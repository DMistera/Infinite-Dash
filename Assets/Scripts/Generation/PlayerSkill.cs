using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerSkill : Difficulty {

    private readonly float INCREASE_RATE = 0.5f;
    private readonly float DECREASE_RATE = 1f;
    private List<PlayerSkill> history = new List<PlayerSkill>();

    public PlayerSkill(string json) : base(json) {
    }

    public PlayerSkill(PlayerSkill other) : base(other) {
    }

    public PlayerSkill(Difficulty diff) : base(diff) {
    }

    public PlayerSkill Previous() {
        return history[history.Count - 1];
    }

    public void IncreaseTo(Difficulty difficulty) {
        history.Add(new PlayerSkill(this));
        foreach (DifficultyType type in Enum.GetValues(typeof(DifficultyType))) {
            float v = difficulty.Get(type);
            if(v > map[type]) {
                float diff = v - map[type];
                map[type] += diff * INCREASE_RATE;
            }
        }
    }

    public void DecreaseTo(Difficulty difficulty, Difficulty cause) {
        history.Add(new PlayerSkill(this));
        foreach (DifficultyType type in Enum.GetValues(typeof(DifficultyType))) {
            float v = difficulty.Get(type);
            if (v < map[type]) {
                float diff = (map[type] - v) * cause.Get(type);
                map[type] -= diff * DECREASE_RATE;
            }
        }
    }

    public void Update() {

    }

    public void Save() {
        PlayerProfile.Instance.PlayerSkill = this;
    }
}

