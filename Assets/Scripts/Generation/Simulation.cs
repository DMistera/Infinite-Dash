using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Simulation {

    private readonly GameObject parent;
    private readonly Player playerPrefab;
    private readonly GameObject markerPrefab;
    public static readonly float DELTA_TIME = 0.005f;

    public Action<SimulationState> OnActionEnter;
    public Action<SimulationState> OnActionExit;
    public Action<SimulationState> OnStep;
    public Action<SimulationState> OnEnd;

    public System.Collections.Generic.List<float> JumpPositions { get; private set; } = new System.Collections.Generic.List<float>();
    public float Duration { get; private set; }
    private readonly bool mark;

    public Simulation(GameObject parent, Player playerPrefab, GameObject markerPrefab, bool mark) {
        this.parent = parent;
        this.playerPrefab = playerPrefab;
        this.markerPrefab = markerPrefab;
        this.mark = mark;
    }

    private Player player;

    public void Init() {
        player = GameObject.Instantiate(playerPrefab, parent.transform);
        player.transform.localPosition = Vector3.zero;
        player.transform.rotation = Quaternion.identity;
        player.ActiveChunk = parent.GetComponent<Chunk>();
    }

    public IEnumerator RunAsync(PlayerActionTimeline playerActionTimeline) {
        PhysicsScene2D physicsScene2D = GetPhysicsScene2D();
        Physics2D.simulationMode = SimulationMode2D.Script;
        float stepTime = 1f / player.forwardVelocity;
        float stepTimeAccumulator = 0;
        float timeAccumulator = 0;
        SimulationState state = new SimulationState {
            Player = player,
        };
        foreach (PlayerAction action in playerActionTimeline.actions) {
            state.Steps = 0;
            state.CurrentAction = action;
            state.ActionDeltaY = 0;
            OnActionEnter?.Invoke(state);
            float originalY = player.transform.localPosition.y;
            if (action.Type == PlayerActionType.JUMP || action.Type == PlayerActionType.DOUBLE_JUMP) {
                player.OnSpace();
                JumpPositions.Add(player.transform.localPosition.x);
            }
            while (!ActionExitCondition(action, state)) {
                state.ActionDeltaY = player.transform.localPosition.y - originalY;
                player.SetVelocityX(player.forwardVelocity);
                if (stepTimeAccumulator >= stepTime) {
                    OnStep?.Invoke(state);
                    stepTimeAccumulator -= stepTime;
                    state.Steps++;
                }
                if (mark) {
                    GameObject marker = GameObject.Instantiate(markerPrefab, parent.transform);
                    marker.GetComponent<MeshRenderer>().material.color = Color.green;
                    marker.transform.localPosition = player.transform.localPosition;
                }
                Physics2D.simulationMode = SimulationMode2D.Script;
                if (!physicsScene2D.Simulate(DELTA_TIME)) {
                    throw new Exception("Simulation count not run!");
                }
                stepTimeAccumulator += DELTA_TIME;
                timeAccumulator += DELTA_TIME;
                yield return null;
            }
            OnActionExit?.Invoke(state);
            state.PreviousAction = action;
        }
        Duration = timeAccumulator;
        GameObject.Destroy(player.gameObject);
        OnEnd?.Invoke(state);
    }

    public void Repeat() {
        PhysicsScene2D physicsScene2D = GetPhysicsScene2D();
        Physics2D.simulationMode = SimulationMode2D.Script;
        Player player = GameObject.Instantiate(playerPrefab, parent.transform);
        player.transform.localPosition = Vector3.zero;
        player.transform.rotation = Quaternion.identity;
        player.ActiveChunk = parent.GetComponent<Chunk>();
        BoxCollider2D collider = player.GetComponent<BoxCollider2D>();
        collider.size *= 1.1f;
        float lastX = 0f;
        for (float t = 0; t < Duration; t += DELTA_TIME) {
            player.SetVelocityX(player.forwardVelocity);
            foreach (float x in JumpPositions) {
                if (x > lastX && x <= player.transform.localPosition.x) {
                    player.OnSpace();
                }
            }
            lastX = player.transform.localPosition.x;
            if(mark) {
                GameObject marker = GameObject.Instantiate(markerPrefab, parent.transform);
                marker.transform.localPosition = player.transform.localPosition;
            }
            physicsScene2D.Simulate(DELTA_TIME);
        }
        GameObject.Destroy(player.gameObject);
    }

    private bool ActionExitCondition(PlayerAction action, SimulationState state) {
        switch (action.Type) {
            case PlayerActionType.JUMP:
            case PlayerActionType.DOUBLE_JUMP:
                return state.Player.GetComponent<Rigidbody2D>().velocity.y < 0f && state.ActionDeltaY < action.Value;
            case PlayerActionType.FALL:
                return state.ActionDeltaY < action.Value;
            case PlayerActionType.SLIDE:
                return state.Steps >= action.Value;
            default:
                throw new ArgumentOutOfRangeException();
        };
    }

    private PhysicsScene2D GetPhysicsScene2D() {
        return UnityEngine.SceneManagement.SceneManager.GetSceneByName("Global").GetPhysicsScene2D();
    }
}

