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
    public bool TestPassed { get; set; } = false;

    public Chunk Clone(Transform parent) {
        Chunk chunkClone = Instantiate(this, parent);
        chunkClone.Solution = Solution;
        chunkClone.Difficulty = Difficulty;
        chunkClone.Ready = Ready;
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
            player.Score++;
            player.Skill.IncreaseTo(Difficulty);
            if (player.saveSkillChanges) {
                player.Skill.Save();
            }
        };
    }
}
