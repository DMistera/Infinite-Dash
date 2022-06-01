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
    int counter = 0;
    int counterAll = 0;
    bool firstStep = false;
    Vector3 lastTrapPosition;
    bool activateFlag = false;

    public override void ActionEnter(SimulationState state) {
        if (state.CurrentAction.Type == PlayerActionType.FALL) {
            firstStep = true;
            counterAll++;
            activateFlag = UnityEngine.Random.value < state.Difficulty.Get(DifficultyType.TRAP_FREQUENCY);
            if (activateFlag) {
                counter++;
            }
        }
    }

    public override void ActionExit(SimulationState state) {
        if (activateFlag && state.CurrentAction.Type == PlayerActionType.FALL) {
            SpawnChain(state);
            activateFlag = false;
        }
    }

    public override void Step(SimulationState state) {
        if(activateFlag && state.CurrentAction.Type == PlayerActionType.FALL) {
            SpawnTrap(state);
            if (firstStep) {
                SpawnChain(state);
            }
            firstStep = false;
        }
    }

    private void SpawnTrap(SimulationState state) {
        Vector3 v = state.Player.transform.localPosition;
        v.y -= state.ActionDeltaY;
        v.y += 2;
        Entity spike = SpawnEntity(state.Chunk, spikePrefab, v);
        spike.transform.rotation = Quaternion.Euler(0, 0, 180f);
        v.y += 1;
        SpawnEntity(state.Chunk, blockPrefab, v);
        lastTrapPosition = v;
    }

    private void SpawnChain(SimulationState state) {
        SpawnEntity(state.Chunk, chainPrefab, lastTrapPosition + new Vector3(0, 1, 0));
    }

    public override void Finally(SimulationState state) {
        state.Difficulty.Set(DifficultyType.TRAP_FREQUENCY, (float)counter / counterAll);
    }
}

