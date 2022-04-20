using System.Collections;
using UnityEngine;


public class CloudPolicy : ChunkGenerationPolicy {

    public Cloud cloudPrefab;
    public float cloudDensity = 0.1f;

    public override void ActionEnter(SimulationState state) {
        if(UnityEngine.Random.value < cloudDensity) {
            SpawnCloud(state);
        }
    }

    public override void ActionExit(SimulationState state) {
        
    }

    public override void Step(SimulationState state) {
        
    }

    public override void TriggerEnter(Chunk chunk, Player player, Collider2D collider2D) {
        
    }

    private void SpawnCloud(SimulationState state) {
        Vector3 v = state.Player.transform.localPosition;
        v.y += UnityEngine.Random.Range(0f, 10f);
        v.z = -2;
        SpawnEntity(state.Chunk, cloudPrefab, v);
    }
}
