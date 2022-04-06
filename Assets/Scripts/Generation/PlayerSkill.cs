using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerSkill : Difficulty {

    private readonly float INCREASE_RATE = 0.5f;
    private readonly float DECREASE_RATE = 1f;

    public PlayerSkill(string json) : base(json) {
    }

    public PlayerSkill(PlayerSkill other) : base(other) {
    }

    public void IncreaseTo(Difficulty difficulty) {
        foreach (DifficultyType type in Enum.GetValues(typeof(DifficultyType))) {
            float v = difficulty.Get(type);
            if(v > map[type]) {
                float diff = v - map[type];
                map[type] += diff * INCREASE_RATE;
            }
        }
        Save();
    }

    public void DecreaseTo(Difficulty difficulty, Difficulty cause) {
        foreach (DifficultyType type in Enum.GetValues(typeof(DifficultyType))) {
            float v = difficulty.Get(type);
            if (v < map[type]) {
                float diff = (map[type] - v) * cause.Get(type);
                map[type] -= diff * DECREASE_RATE;
            }
        }
        Save();
    }

    public void Update() {

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

    public void Save() {
        PlayerProfile.Instance.PlayerSkill = this;
    }
}

