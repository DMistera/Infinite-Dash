using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VinePolicy : ChunkGenerationPolicy {

    public Entity vinePrefab;
    public float vineDensity = 0.1f;

    public override void Step(SimulationState state) {
        if(state.CurrentAction.Type == PlayerActionType.SLIDE) {
            if(UnityEngine.Random.value < vineDensity) {
                SpawnVine(state);
            }
        }
    }

    private void SpawnVine(SimulationState state) {
        Vector3 v = state.Player.transform.localPosition;
        v.z = -1;
        SpawnEntity(state.Chunk, vinePrefab, v);
    }
}
