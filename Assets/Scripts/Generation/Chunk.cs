using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class Chunk : MonoBehaviour {

    public Vector3 EndPosition { get; set; }

    public Action<Player> OnPlayerEnter;
    public Action<Player> OnPlayerLeave;
    public ChunkSolution Solution { get; set; }
    public Difficulty Difficulty { get; set; }
    public float Duration { get; set; }

    public ChunkTrigger Trigger { get; private set; }
    public Grid Grid { get; private set; }
    public bool Ready { get; set; } = false;
    public bool Ranked { get; set; } = true;
    public bool TestPassed { get; set; } = false;

    public Chunk Clone(Transform parent) {
        Chunk chunkClone = Instantiate(this, parent);
        foreach (Transform child in chunkClone.transform) {
            Entity entity = child.GetComponent<Entity>();
            if (entity != null) {
                Destroy(child.gameObject);
            }
        }
        foreach (Transform child in transform) {
            Entity entity = child.GetComponent<Entity>();
            if (entity != null) {
                entity.Clone(chunkClone.transform);
            }
        }
        chunkClone.Solution = Solution;
        chunkClone.Difficulty = Difficulty;
        chunkClone.Ready = Ready;
        chunkClone.Ranked = Ranked;
        return chunkClone;
    }

    public void Awake() {
        Trigger = GetComponentInChildren<ChunkTrigger>();
        Grid = GetComponent<Grid>();
    }

    // Use this for initialization
    public void Start() {
        OnPlayerEnter += (player) => {
            player.SetActiveChunk(this);
        };
        OnPlayerLeave += (player) => {
            player.HandleLeaveChunk(this);
        };
    }
}
