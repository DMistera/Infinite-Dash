using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SpikePolicy : ChunkGenerationPolicy {

    public Spike spikePrefab;
    public Solid solidPrefab;

    private List<Entity> spikes = new List<Entity>();
    bool activateFlag = false;

    public override void ActionEnter(SimulationState state) {
        spikes.Clear();
        activateFlag = UnityEngine.Random.value < 0.4f;
    }

    public override void ActionExit(SimulationState state) {
        if (activateFlag) {
            int len = Mathf.RoundToInt(spikes.Count * state.Difficulty.Get(DifficultyType.SPIKE_LENGTH));
            int startIndex = UnityEngine.Random.Range(0, spikes.Count - 1 - len);
            int endIndex = startIndex + len;
            int i = 0;
            foreach (Entity e in spikes) {
                if (i < startIndex || i >= endIndex) {
                    state.Chunk.Grid.Destroy(e);
                }
                i++;
            }
        }
    }

    public override void Step(SimulationState state) {
        if (activateFlag) {
            if (state.CurrentAction is AirAction airAction) {
                if (airAction.LevelDifference >= 0) {
                    SpawnSpike(state);
                    SpawnSolid(state);
                }
            }
        }
    }

    public override void TriggerEnter(Chunk chunk, Player player, Collider2D collider2D) {
        Entity entity = collider2D.GetComponent<Entity>();
        if (entity != null && entity.OriginPolicy == this) {
            Destroy(collider2D.gameObject);
        }
    }

    private void SpawnSpike(SimulationState state) {
        Vector3 v = state.Player.transform.localPosition;
        v.y -= state.ActionDeltaY;
        if (state.Chunk.Grid.IsEmpty(v)) {
            Entity spike = SpawnEntity(state.Chunk, spikePrefab, v);
            spikes.Add(spike);
        }
    }

    private void SpawnSolid(SimulationState state) {
        Vector3 v = state.Player.transform.localPosition;
        v.y -= state.ActionDeltaY + 1;
        if (state.Chunk.Grid.IsEmpty(v)) {
            SpawnEntity(state.Chunk, solidPrefab, v);
        }
    }
}

