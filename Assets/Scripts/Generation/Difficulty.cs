using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Difficulty {
    protected readonly Dictionary<DifficultyType, float> map = new Dictionary<DifficultyType, float>();


    public float Get(DifficultyType type) {
        return map[type];
    }

    public void Set(DifficultyType type, float v) {
        //Debug.Log($"Change from {map[type]} to {v} on {type}");
        map[type] = Mathf.Clamp(v, 0f, 1f);
    }

    public Difficulty() {
    }

    public Difficulty(Difficulty other) {
        map = new Dictionary<DifficultyType, float>(other.map);
    }

    public Difficulty(string json) {
        map = JsonConvert.DeserializeObject<Dictionary<DifficultyType, float>>(json);
    }

    public float Difference(Difficulty other) {
        float sum = 0;
        foreach (var key in map.Keys) {
            sum += Mathf.Abs(map[key] - other.map[key]);
        }
        return sum* sum;
    }

    public Difficulty MakeSimilar(float range) {
        Difficulty difficulty = new Difficulty();
        foreach (var key in map.Keys) {
            float v = UnityEngine.Random.Range(map[key] - range, map[key] + range);
            difficulty.map[key] = Mathf.Clamp(v, 0f, 1f);
        }
        return difficulty;
    }

    public float Rank() {
        float sum = 0;
        float count = 0;
        foreach (float v in map.Values) {
            sum += v;
            count++;
        }
        return sum / count;
    }

    public string Title() {
        return RankProvider.Instance.GetTitle(Rank());
    }

    public override string ToString() {
        return JsonConvert.SerializeObject(map);
    }

    public static Difficulty Random() {
        Difficulty result = new Difficulty();
        foreach (DifficultyType type in Enum.GetValues(typeof(DifficultyType))) {
            result.map.Add(type, UnityEngine.Random.value);
        }
        return result;
    }

    public static Difficulty Initial(float initialValue) {
        Difficulty result = new Difficulty();
        foreach (DifficultyType type in Enum.GetValues(typeof(DifficultyType))) {
            result.map.Add(type, initialValue);
        }
        return result;
    }

    public static Difficulty operator * (Difficulty d1, Difficulty d2) {
        Difficulty result = new Difficulty();
        foreach (DifficultyType type in Enum.GetValues(typeof(DifficultyType))) {
            result.map.Add(type, d1.map[type] * d2.map[type]);
        }
        return result;
    }
}
