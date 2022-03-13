using System;
using System.Collections;
using UnityEngine;

public class Chunk : MonoBehaviour {

    public Player playerPrefab;
    public Solid solidPrefab;
    public Bomb bombPrefab;
    public Vector3 EndPosition { get; private set; }

    public Action<Player> OnPlayerEnter;
    public Action<Player> OnPlayerExit;
    public Simulation Simulation { get; private set; }

    private ChunkTrigger trigger;

    public Chunk Clone(Transform parent) {
        Chunk chunkClone = Instantiate(this, parent);
        chunkClone.Simulation = Simulation;
        return chunkClone;
    }

    public void Awake() {
        trigger = GetComponentInChildren<ChunkTrigger>();
    }

    // Use this for initialization
    public void Start() {
        OnPlayerEnter += (player) => {
            player.SetActiveChunk(this);
        };
    }

    public void GenerateByDifficulty(ChunkDifficulty difficulty) {
        PlayerActionTimeline playerActionTimeline = new PlayerActionTimeline(difficulty.Path);
        GenerateByTimeline(playerActionTimeline, difficulty.Obstacles);
    }

    public void GenerateByTimeline(PlayerActionTimeline playerActionTimeline, float difficulty) {
        Simulation = new Simulation(gameObject, playerPrefab);
        Simulation.OnStep += (state) => {
            if (state.CurrentAction.Type == PlayerActionType.SLIDE) {
                SpawnSolid(state.Player);
            }
            if (state.CurrentAction.Type == PlayerActionType.JUMP || state.CurrentAction.Type == PlayerActionType.FALL) {
                float distance = 2.3f - difficulty * 1.5f;
                if (UnityEngine.Random.value < difficulty) {
                    //SpawnBomb(state.Player, distance);
                }
                if (UnityEngine.Random.value < difficulty) {
                    //SpawnBomb(state.Player, -distance);
                }
            }
        };
        Simulation.OnActionEnter += (state) => {
             if (state.CurrentAction.Type == PlayerActionType.SLIDE) {
                 SpawnSolid(state.Player);
             }
        };
        Simulation.OnEnd += (state) => {
            EndPosition = SnapToGrid(state.Player.transform.position);
        };
        Simulation.Run(playerActionTimeline);
        trigger.UpdateShape(EndPosition.x);
        CleanUpOutsideBounds();
        Simulate(playerActionTimeline); // clean up bombs
    }

    private void CleanUpOutsideBounds() {
        
        foreach (Transform transform in transform) {
            if(!trigger.Collider.bounds.Contains(transform.position)) {
                Destroy(transform.gameObject);
            }
        }
    }

    private void Simulate(PlayerActionTimeline playerActionTimeline) {
        Simulation simulation = new Simulation(gameObject, playerPrefab);
        simulation.Run(playerActionTimeline);
    }

    private void SpawnSolid(Player player) {
        Solid solid = Instantiate(solidPrefab, transform);
        Vector3 v = player.transform.position;
        v.y -= Constants.GRID_SIZE;
        solid.transform.position = SnapToGrid(v);
    }

    private void SpawnBomb(Player player, float offset) {
        Vector2 shift = player.GetComponent<Rigidbody2D>().velocity;
        shift.Normalize();
        shift = Utils.Math.RotateVector(shift, Mathf.PI / 2);
        shift *= offset;
        Bomb bomb = Instantiate(bombPrefab, transform);
        bomb.transform.position = SnapToGrid(player.transform.position + new Vector3(shift.x, shift.y, 0f));
    }

    private Vector3 SnapToGrid(Vector3 v) {
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        return v;
    }
}
