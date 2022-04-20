using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class ChainPolicy : ChunkGenerationPolicy {

    public Entity chainPrefab;

    private bool activeFlag;
    public override void ActionEnter(SimulationState state) {
        if(state.CurrentAction.Type == PlayerActionType.SLIDE) {
            int length = (state.CurrentAction as SlideAction).Length;
            if (length >= 5) {
                activeFlag = true;
            }
        }
    }

    public override void ActionExit(SimulationState state) {
        activeFlag = false;
    }

    public override void Step(SimulationState state) {
        if (activeFlag) {
            int length = (state.CurrentAction as SlideAction).Length;
            int steps = state.Steps;
            if (steps == 0 || steps == length - 1) {
                SpawnChain(state);
            }
        }
    }

    public override void TriggerEnter(Chunk chunk, Player player, Collider2D collider2D) {
        
    }

    private void SpawnChain(SimulationState state) {
        Vector3 v = state.Player.transform.localPosition;
        v.y -= state.ActionDeltaY;
        v.z = -1;
        SpawnEntity(state.Chunk, chainPrefab, v);
    }
}

