using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Grid : MonoBehaviour {

    private Dictionary<Vector2Int, Entity> map = new Dictionary<Vector2Int, Entity>();

    public Entity Get(Vector2Int position) {
        return map[position];
    }

    public Entity Get(Vector3 position3D) {
        position3D = SnapToGrid(position3D);
        return Get(ToVector2Int(position3D));
    }

    public Vector2Int GetPosition(Entity entity) {
        return ToVector2Int(entity.transform.localPosition);
    }

    public void Set(Entity entity) {
        Set(entity, entity.transform.localPosition);
    }

    public void Set(Entity entity, Vector3 position3D) {
        position3D = SnapToGrid(position3D);
        Set(entity, ToVector2Int(position3D));
    }

    public void Set(Entity entity, Vector2Int position) {
        if (map.ContainsKey(position)) {
            GameObject.Destroy(map[position].gameObject);
            map.Remove(position);
        }
        map.Remove(GetPosition(entity));
        entity.transform.parent = transform;
        entity.transform.localPosition = ToVector3(position);
        map[position] = entity;
    }

    public void Destroy(Entity entity) {
        Vector2Int position = ToVector2Int(entity.transform.localPosition);
        if (map.ContainsKey(position)) {
            GameObject.Destroy(map[position].gameObject);
            map.Remove(position);
        }
    }

    public bool IsEmpty(Vector3 position) {
        try {
            Get(position);
            return false;
        }
        catch (KeyNotFoundException) {
            return true;
        }
    }

    public void MoveToEmpty(Entity entity, Vector3 step) {
        Vector2Int position = GetPosition(entity);
        if (!map.Remove(position)) {            
            throw new Exception("Object is not in the grid");
        }
        Set(entity, FindEmpty(entity.transform.localPosition, step));
    }

    public void ForEach(Action<Entity> action) {
        foreach (Entity entity in map.Values) {
            action.Invoke(entity);
        }
    }

    // TODO
    public void Move(Entity entity, Vector2Int shift) {
        Vector2Int position = GetPosition(entity);
        if(!map.Remove(position)) {
            throw new Exception("Object is not in the grid");
        }
        map[position + shift] = entity;
    }

    private Vector2Int FindEmpty(Vector3 centre, Vector3 step) {
        Vector2Int original = ToVector2Int(centre);
        while (true) {
            Vector2Int position = ToVector2Int(centre);
            if (!map.ContainsKey(position) && position != original) {
                return position;
            }
            centre += step;
        }
    }

    private Vector2Int ToVector2Int(Vector3 v) {
        return new Vector2Int(Convert.ToInt32(v.x), Convert.ToInt32(v.y));
    }

    private Vector3 ToVector3(Vector2Int v) {
        return new Vector3(v.x, v.y, 0f);
    }

    public static Vector3 SnapToGrid(Vector3 v) {
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        return v;
    }
}

