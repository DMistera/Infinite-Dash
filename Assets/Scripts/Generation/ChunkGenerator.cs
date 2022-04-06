using System;
using System.Collections;
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
    public Solid solidPrefab;
    public Bomb bombPrefab;
    public GameObject markerPrefab;
    public Chunk chunkPrefab;
    public bool mark = false;

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
        Simulation simulation = new Simulation(chunk.gameObject, playerPrefab, markerPrefab, mark);
        simulation.OnStep += (state) => {
            if (state.CurrentAction.Type == PlayerActionType.SLIDE) {
                SpawnSolid(chunk, state.Player);
            }
            if (state.CurrentAction.Type == PlayerActionType.JUMP || state.CurrentAction.Type == PlayerActionType.FALL || state.CurrentAction.Type == PlayerActionType.DOUBLE_JUMP) {
                float distance = 2.3f - difficulty.Get(DifficultyType.BOMB_SPREAD) * 1.5f;
                if (UnityEngine.Random.value < difficulty.Get(DifficultyType.BOMB_DENSITY)) {
                    SpawnBomb(chunk, state.Player, distance);
                }
                if (UnityEngine.Random.value < difficulty.Get(DifficultyType.BOMB_DENSITY)) {
                    SpawnBomb(chunk, state.Player, -distance);
                }
            }
        };
        simulation.OnActionEnter += (state) => {
            if (state.CurrentAction.Type == PlayerActionType.SLIDE) {
                SpawnSolid(chunk, state.Player);
            }
        };
        simulation.OnEnd += (state) => {
            chunk.EndPosition = SnapToGrid(state.Player.transform.localPosition);
            chunk.Trigger.UpdateShape(chunk.EndPosition.x);
            chunk.Simulation = simulation;
            CleanUpOutsideBounds(chunk);
            simulation.Repeat(); // clean up bombs
            chunk.gameObject.SetActive(false);
            chunk.Ready = true;
            callback?.Invoke(chunk);
        };
        simulation.Init();
        if(async) {
            StartCoroutine(simulation.RunAsync(playerActionTimeline));
        }
        else {
            IEnumerator e = simulation.RunAsync(playerActionTimeline);
            while (e.MoveNext()) { }
        }
        return chunk;
    }

    private void CleanUpOutsideBounds(Chunk chunk) {

        foreach (Transform transform in transform) {
            if (!chunk.Trigger.Collider.bounds.Contains(transform.localPosition)) {
                Destroy(transform.gameObject);
            }
        }
    }

    private void SpawnSolid(Chunk chunk, Player player) {
        Solid solid = Instantiate(solidPrefab, chunk.transform);
        Vector3 v = player.transform.localPosition;
        v.y -= Constants.GRID_SIZE;
        chunk.Grid.Set(solid.gameObject, v);
    }

    private void SpawnBomb(Chunk chunk, Player player, float offset) {
        Vector2 shift = player.GetComponent<Rigidbody2D>().velocity;
        shift.Normalize();
        shift = Utils.Math.RotateVector(shift, Mathf.PI / 2);
        shift *= offset;
        Vector3 pos = player.transform.localPosition + new Vector3(shift.x, shift.y, 0f);
        if(chunk.Grid.IsEmpty(pos)) {
            Bomb bomb = Instantiate(bombPrefab, chunk.transform);
            chunk.Grid.Set(bomb.gameObject, pos);
        }
    }

    private Vector3 SnapToGrid(Vector3 v) {
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        return v;
    }
}
