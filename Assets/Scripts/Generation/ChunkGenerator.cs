﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChunkGenerator : MonoBehaviour {

    public static ChunkGenerator Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<ChunkGenerator>();
            }
            return instance;
        }
    }
    private static ChunkGenerator instance;

    public Player playerPrefab;
    public GameObject markerPrefab;
    public Chunk chunkPrefab;
    public bool mark = false;
    public ChunkGenerationPolicy[] chunkGenerationPolicies;
    public float deltaTime = 0.005f;

    public Chunk GenerateByDifficulty(Difficulty difficulty) {
        return GenerateByTimeline(new PlayerActionTimeline(difficulty), difficulty);
    }

    public void GenerateByDifficultyAsync(Difficulty difficulty, Action<Chunk> callback) {
        GenerateByTimelineAsync(new PlayerActionTimeline(difficulty), difficulty, callback);
    }

    public Chunk GenerateByTimeline(PlayerActionTimeline playerActionTimeline, Difficulty difficulty) {
        return GenerateByTimeline(playerActionTimeline, difficulty, false);
    }

    public void GenerateByTimelineAsync(PlayerActionTimeline playerActionTimeline, Difficulty difficulty, Action<Chunk> callback) {
        GenerateByTimeline(playerActionTimeline, difficulty, true, callback);
    }

    private Chunk GenerateByTimeline(PlayerActionTimeline playerActionTimeline, Difficulty difficulty, bool async, Action<Chunk> callback = null) {
        Chunk chunk = Instantiate(chunkPrefab, ChunkLibrary.Instance.transform);
        chunk.Difficulty = difficulty;
        if(async) {
            StartCoroutine(RunAsync(chunk, playerActionTimeline, difficulty, callback));
        }
        else {
            IEnumerator e = RunAsync(chunk, playerActionTimeline, difficulty, callback);
            while (e.MoveNext()) { }
        }
        return chunk;
    }

    public IEnumerator RunAsync(Chunk chunk, PlayerActionTimeline playerActionTimeline, Difficulty difficulty, Action<Chunk> callback) {
        Player player = Instantiate(playerPrefab, chunk.transform);
        player.transform.localPosition = Vector3.zero;
        player.transform.rotation = Quaternion.identity;
        player.ActiveChunk = chunk;
        PhysicsScene2D physicsScene2D = GetPhysicsScene2D();
        Physics2D.simulationMode = SimulationMode2D.Script;
        float nextStepX = 0;
        float timeAccumulator = 0;
        SimulationState state = new SimulationState {
            Player = player,
            Chunk = chunk,
            Difficulty = difficulty
        };
        ChunkSolution solution = new ChunkSolution();
        for(int i = 0; i < playerActionTimeline.actions.Count; i++) {
            PlayerAction action = playerActionTimeline.actions[i];
            state.Steps = 0;
            state.CurrentAction = action;
            state.NextAction = i + 1 >= playerActionTimeline.actions.Count ? null : playerActionTimeline.actions[i + 1];
            state.ActionDeltaY = 0;
            foreach (ChunkGenerationPolicy policy in chunkGenerationPolicies) {
                policy.ActionEnter(state);
            }
            float originalY = Mathf.Round(player.transform.localPosition.y);
            if (action.Type == PlayerActionType.JUMP || action.Type == PlayerActionType.DOUBLE_JUMP) {
                player.OnSpace();
                solution.JumpPositions.Add(player.transform.localPosition.x);
            }
            while (!ActionExitCondition(action, state)) {
                state.ActionDeltaY = player.transform.localPosition.y - originalY;
                player.SetVelocityX(player.forwardVelocity);
                if (player.transform.localPosition.x >= nextStepX) {
                    foreach (ChunkGenerationPolicy policy in chunkGenerationPolicies) {
                        policy.Step(state);
                    }
                    nextStepX += 1f;
                    state.Steps++;
                }
                if (mark) {
                    GameObject marker = Instantiate(markerPrefab, chunk.transform);
                    marker.GetComponent<SpriteRenderer>().material.color = Color.green;
                    marker.transform.localPosition = player.transform.localPosition;
                }
                Physics2D.simulationMode = SimulationMode2D.Script;
                if (!physicsScene2D.Simulate(deltaTime)) {
                    throw new Exception("Simulation could not run!");
                }
                timeAccumulator += deltaTime;
                if (timeAccumulator > 15f) {
                    throw new Exception("Failed to generate chunk!");
                }
                yield return null;
            }
            foreach (ChunkGenerationPolicy policy in chunkGenerationPolicies) {
                policy.ActionExit(state);
            }
            state.PreviousAction = action;
        }
        chunk.Duration = timeAccumulator;
        Destroy(player.gameObject);

        chunk.EndPosition = Grid.SnapToGrid(state.Player.transform.localPosition);
        chunk.Trigger.UpdateShape(chunk.EndPosition.x);
        chunk.Solution = solution;
        EnableEntities(chunk);
        CleanOutsideBounds(chunk);
        while(!chunk.TestPassed) {
            chunk.TestPassed = true;
            IEnumerator e = RunTest(chunk);
            while (e.MoveNext()) {
                yield return null;
            }
        }
        CleanOutsideBounds(chunk);
        chunk.gameObject.SetActive(false);
        chunk.Ready = true;

        foreach (ChunkGenerationPolicy policy in chunkGenerationPolicies) {
            policy.Finally(state);
        }
        callback?.Invoke(chunk);
    }

    private IEnumerator RunTest(Chunk chunk) {
        PhysicsScene2D physicsScene2D = GetPhysicsScene2D();
        Physics2D.simulationMode = SimulationMode2D.Script;
        Player player = Instantiate(playerPrefab, chunk.transform);
        player.transform.localPosition = Vector3.zero;
        player.transform.rotation = Quaternion.identity;
        player.ActiveChunk = chunk;
        player.OnTriggerEnter += (Collider2D collider2D) => {
            foreach (ChunkGenerationPolicy policy in chunkGenerationPolicies) {
                policy.TriggerEnter(chunk, player, collider2D);
            }
        };

        //BoxCollider2D collider = player.GetComponent<BoxCollider2D>();
        //collider.size *= 1.5f;
        float lastX = 0f;
        for (float t = 0; t < chunk.Duration; t += deltaTime) {
            player.SetVelocityX(player.forwardVelocity);
            foreach (float x in chunk.Solution.JumpPositions) {
                if (x > lastX && x <= player.transform.localPosition.x) {
                    player.OnSpace();
                }
            }
            lastX = player.transform.localPosition.x;
            if (mark) {
                GameObject marker = Instantiate(markerPrefab, chunk.transform);
                marker.transform.localPosition = player.transform.localPosition;
            }
            physicsScene2D.Simulate(deltaTime);
            yield return null;
        }
        Destroy(player.gameObject);
    }

    private bool ActionExitCondition(PlayerAction action, SimulationState state) {
        switch (action.Type) {
            case PlayerActionType.JUMP:
            case PlayerActionType.DOUBLE_JUMP:
                return state.Player.GetComponent<Rigidbody2D>().velocity.y < 0f && state.ActionDeltaY < ((AirAction)action).LevelDifference;
            case PlayerActionType.FALL:
                return state.ActionDeltaY < ((AirAction)action).LevelDifference;
            case PlayerActionType.SLIDE:
                return state.Steps >= ((SlideAction)action).Length;
            default:
                throw new ArgumentOutOfRangeException();
        };
    }

    private void EnableEntities(Chunk chunk) {
        chunk.Grid.ForEach((key, entity) => {
            entity.gameObject.SetActive(true);
        });
    }

    private void CleanOutsideBounds(Chunk chunk) {
        List<Entity> toDelete = new List<Entity>();
        chunk.Grid.ForEach((key, entity) => {
            if(key.x >= (int)chunk.EndPosition.x || key.x < 0) {
                toDelete.Add(entity);
            }
        });
        foreach (Entity entity in toDelete) {
            chunk.Grid.Destroy(entity);
        }
    }

    /*private void CleanOutsideBounds(Chunk chunk) {
        foreach (Transform t in chunk.transform) {
            if (!chunk.Trigger.Collider.bounds.Contains(t.position)) {
                Destroy(t.gameObject);
            }
        }
    }*/

    private PhysicsScene2D GetPhysicsScene2D() {
        return UnityEngine.SceneManagement.SceneManager.GetSceneByName("Global").GetPhysicsScene2D();
    }
}
