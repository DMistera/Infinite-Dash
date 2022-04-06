using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Grid : MonoBehaviour {

    private Dictionary<Vector2Int, GameObject> map = new Dictionary<Vector2Int,GameObject>();

    public GameObject Get(Vector2Int position) {
        return map[position];
    }

    public GameObject Get(Vector3 position3D) {
        position3D = SnapToGrid(position3D);
        return Get(ToVector2Int(position3D));
    }

    public Vector2Int GetPosition(GameObject gameObject) {
        return ToVector2Int(gameObject.transform.localPosition);
    }

    public void Set(GameObject gameObject) {
        Set(gameObject, gameObject.transform.localPosition);
    }

    public void Set(GameObject gameObject, Vector3 position3D) {
        position3D = SnapToGrid(position3D);
        Set(gameObject, ToVector2Int(position3D));
    }

    public void Set(GameObject gameObject, Vector2Int position) {
        if (map.ContainsKey(position)) {
            GameObject.Destroy(map[position]);
            map.Remove(position);
        }
        map.Remove(GetPosition(gameObject));
        gameObject.transform.parent = transform;
        gameObject.transform.localPosition = ToVector3(position);
        map[position] = gameObject;
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

    public void MoveToEmpty(GameObject gameObject, Vector3 step) {
        Vector2Int position = GetPosition(gameObject);
        if (!map.Remove(position)) {            
            throw new Exception("Object is not in the grid");
        }
        Set(gameObject, FindEmpty(gameObject.transform.localPosition, step));
    }

    // TODO
    public void Move(GameObject gameObject, Vector2Int shift) {
        Vector2Int position = GetPosition(gameObject);
        if(!map.Remove(position)) {
            throw new Exception("Object is not in the grid");
        }
        map[position + shift] = gameObject;
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

    private Vector3 SnapToGrid(Vector3 v) {
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        return v;
    }
}

