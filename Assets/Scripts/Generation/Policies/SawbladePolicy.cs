using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SawbladePolicy : ChunkGenerationPolicy {

    public Sawblade bombPrefab;

    public override void ActionEnter(SimulationState state) {
        
    }

    public override void ActionExit(SimulationState state) {
        
    }

    public override void Step(SimulationState state) {
        if (state.CurrentAction.Type == PlayerActionType.JUMP || state.CurrentAction.Type == PlayerActionType.FALL || state.CurrentAction.Type == PlayerActionType.DOUBLE_JUMP) {
            float distance = 2.3f - state.Difficulty.Get(DifficultyType.BOMB_SPREAD) * 1.5f;
            if (UnityEngine.Random.value < state.Difficulty.Get(DifficultyType.BOMB_DENSITY)) {
                SpawnSawblade(state.Chunk, state.Player, distance);
            }
            if (UnityEngine.Random.value < state.Difficulty.Get(DifficultyType.BOMB_DENSITY)) {
                SpawnSawblade(state.Chunk, state.Player, -distance);
            }
        }
    }

    public override void TriggerEnter(Chunk chunk, Player player, Collider2D collider2D) {
        if (collider2D.GetComponent<Sawblade>() != null) {
            Vector3 step = collider2D.transform.localPosition - player.transform.localPosition;
            chunk.Grid.MoveToEmpty(collider2D.GetComponent<Entity>(), step);
            chunk.TestPassed = false;
        }
    }

    private void SpawnSawblade(Chunk chunk, Player player, float offset) {
        Vector2 shift = player.GetComponent<Rigidbody2D>().velocity;
        shift.Normalize();
        shift = Utils.Math.RotateVector(shift, Mathf.PI / 2);
        shift *= offset;
        Vector3 pos = player.transform.localPosition + new Vector3(shift.x, shift.y, 0f);
        if (chunk.Grid.IsEmpty(pos)) {
            Sawblade sawblade = SpawnEntity(chunk, bombPrefab, pos) as Sawblade;
            sawblade.Init();
        }
    }
}

