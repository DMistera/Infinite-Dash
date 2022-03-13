using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Simulation {

    private readonly GameObject parent;
    private readonly Player playerPrefab;
    private readonly float deltaTime = 0.001f;

    public Action<SimulationState> OnActionEnter;
    public Action<SimulationState> OnActionExit;
    public Action<SimulationState> OnStep;
    public Action<SimulationState> OnEnd;

    public List<float> JumpTimes { get; private set; } = new List<float>();

    public Simulation(GameObject parent, Player playerPrefab) {
        this.parent = parent;
        this.playerPrefab = playerPrefab;
    }

    public void Run(PlayerActionTimeline playerActionTimeline) {
        Physics2D.simulationMode = SimulationMode2D.Script;
        Player player = GameObject.Instantiate(playerPrefab, parent.transform);
        player.transform.position = Vector3.zero;
        player.transform.rotation = Quaternion.identity;
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
            float originalY = player.transform.position.y;
            if (action.Type == PlayerActionType.JUMP) {
                player.OnSpace();
                JumpTimes.Add(timeAccumulator);
            }
            while (!ActionExitCondition(action, state)) {
                state.ActionDeltaY = player.transform.position.y - originalY;
                player.SetVelocityX(player.forwardVelocity);
                if (stepTimeAccumulator >= stepTime) {
                    OnStep?.Invoke(state);
                    stepTimeAccumulator -= stepTime;
                    state.Steps++;
                }
                Physics2D.Simulate(deltaTime);
                stepTimeAccumulator += deltaTime;
                timeAccumulator += deltaTime;
            }
            OnActionExit?.Invoke(state);
            state.PreviousAction = action;
        }
        OnEnd?.Invoke(state);
        Physics2D.simulationMode = SimulationMode2D.Update;
        GameObject.Destroy(player.gameObject);
    }

    private bool ActionExitCondition(PlayerAction action, SimulationState state) {
        return action.Type switch {
            PlayerActionType.JUMP => state.Player.GetComponent<Rigidbody2D>().velocity.y < 0f && state.ActionDeltaY < action.Value,
            PlayerActionType.FALL => state.ActionDeltaY < action.Value,
            PlayerActionType.SLIDE => state.Steps >= action.Value,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}

