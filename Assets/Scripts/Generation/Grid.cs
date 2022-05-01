using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Grid : MonoBehaviour {

    private Dictionary<Vector3Int, Entity> map = new Dictionary<Vector3Int, Entity>();

    public Entity Get(Vector3Int position) {
        return map[position];
    }

    public Entity Get(Vector3 position3D) {
        position3D = SnapToGrid(position3D);
        return Get(ToVector3Int(position3D));
    }

    public Vector3Int GetPosition(Entity entity) {
        return ToVector3Int(entity.transform.localPosition);
    }

    public void Set(Entity entity) {
        Set(entity, entity.transform.localPosition);
    }

    public void Set(Entity entity, Vector3 position3D) {
        position3D = SnapToGrid(position3D);
        Set(entity, ToVector3Int(position3D));
    }

    public void Set(Entity entity, Vector3Int position) {
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
        Vector3Int position = ToVector3Int(entity.transform.localPosition);
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
        Vector3Int position = GetPosition(entity);
        if (!map.Remove(position)) {            
            throw new Exception("Object is not in the grid");
        }
        Set(entity, FindEmpty(entity.transform.localPosition, step));
    }

    public void ForEach(Action<Vector3Int, Entity> action) {
        foreach (KeyValuePair<Vector3Int, Entity> entity in map) {
            action.Invoke(entity.Key, entity.Value);
        }
    }

    // TODO
    public void Move(Entity entity, Vector3Int shift) {
        Vector3Int position = GetPosition(entity);
        if(!map.Remove(position)) {
            throw new Exception("Object is not in the grid");
        }
        map[position + shift] = entity;
    }

    private Vector3Int FindEmpty(Vector3 centre, Vector3 step) {
        Vector3Int original = ToVector3Int(centre);
        while (true) {
            Vector3Int position = ToVector3Int(centre);
            if (!map.ContainsKey(position) && position != original) {
                return position;
            }
            centre += step;
        }
    }

    private Vector3Int ToVector3Int(Vector3 v) {
        return new Vector3Int(Convert.ToInt32(v.x), Convert.ToInt32(v.y), Convert.ToInt32(v.z));
    }

    private Vector3 ToVector3(Vector3Int v) {
        return new Vector3(v.x, v.y, v.z);
    }

    public static Vector3 SnapToGrid(Vector3 v) {
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        return v;
    }
}

