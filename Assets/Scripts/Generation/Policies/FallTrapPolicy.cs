using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FallTrapPolicy : ChunkGenerationPolicy {

    public Spike spikePrefab;
    public Solid blockPrefab;
    public Entity chainPrefab;

    public override void ActionEnter(SimulationState state) {
        if (state.CurrentAction.Type == PlayerActionType.FALL) {
            SpawnTrap(state);
            SpawnChain(state);
        }
    }

    public override void ActionExit(SimulationState state) {
        if (state.CurrentAction.Type == PlayerActionType.FALL) {
            SpawnTrap(state);
            SpawnChain(state);
        }
    }

    public override void Step(SimulationState state) {
        if(state.CurrentAction.Type == PlayerActionType.FALL) {
            SpawnTrap(state);
        }
    }

    public override void TriggerEnter(Chunk chunk, Player player, Collider2D collider2D) {
        
    }

    private void SpawnTrap(SimulationState state) {
        Vector3 v = state.Player.transform.localPosition;
        v.y -= state.ActionDeltaY;
        v.y += 2;
        Entity spike = SpawnEntity(state.Chunk, spikePrefab, v);
        spike.transform.rotation = Quaternion.Euler(0, 0, 180f);
        v.y += 1;
        SpawnEntity(state.Chunk, blockPrefab, v);
    }

    private void SpawnChain(SimulationState state) {
        Vector3 v = state.Player.transform.localPosition;
        v.y -= state.ActionDeltaY;
        v.y += 4;
        SpawnEntity(state.Chunk, chainPrefab, v);
    }
}

