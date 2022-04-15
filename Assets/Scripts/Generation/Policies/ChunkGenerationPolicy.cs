using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ChunkGenerationPolicy : MonoBehaviour {
    public abstract void ActionEnter(SimulationState state);
    public abstract void ActionExit(SimulationState state);
    public abstract void Step(SimulationState state);
    public abstract void TriggerEnter(Chunk chunk, Player player, Collider2D collider2D);

    protected Entity SpawnEntity(Chunk chunk, Entity entityPrefab, Vector3 v, bool disable = true) {
        Entity entity = Instantiate(entityPrefab, chunk.transform);
        entity.OriginPolicy = this;
        chunk.Grid.Set(entity, v);
        if (disable) {
            entity.gameObject.SetActive(false);
        }
        return entity;
    }
}
