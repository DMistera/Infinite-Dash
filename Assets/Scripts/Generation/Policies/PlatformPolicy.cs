using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlatformPolicy : ChunkGenerationPolicy {

    public Solid platformPrefab;
    public Solid blockPrefab;

    public override void ActionEnter(SimulationState state) {
        if (state.CurrentAction.Type == PlayerActionType.SLIDE) {
            SpawnSolid(state.Chunk, state.Player, blockPrefab);
            if(state.PreviousAction != null) {
                SpawnExtension(state);
            }
        }
    }

    public override void ActionExit(SimulationState state) {
    }

    public override void Step(SimulationState state) {
        if (state.CurrentAction.Type == PlayerActionType.SLIDE) {
            SpawnSolid(state.Chunk, state.Player, blockPrefab);
        }
    }

    public override void TriggerEnter(Chunk chunk, Player player, Collider2D collider2D) {
        
    }

    private void SpawnExtension(SimulationState state) {
        float max = (1f - state.Difficulty.Get(DifficultyType.PLATFORM_LENGTH)) * 4f;
        int length = Mathf.RoundToInt(UnityEngine.Random.Range(0f, max));

        for (int i = 0; i < length; i++) {
            Solid solid = Instantiate(platformPrefab, state.Chunk.transform);
            Vector3 v = state.Player.transform.localPosition;
            v.y -= Constants.GRID_SIZE;
            v.x -= i + 1;
            state.Chunk.Grid.Set(solid, v);
        }
    }

    private void SpawnSolid(Chunk chunk, Player player, Solid solidPrefab) {
        Solid solid = Instantiate(solidPrefab, chunk.transform);
        Vector3 v = player.transform.localPosition;
        v.y -= Constants.GRID_SIZE;
        chunk.Grid.Set(solid, v);
    }
}

