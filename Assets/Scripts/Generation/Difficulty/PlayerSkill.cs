using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerSkill : Difficulty {

    private List<PlayerSkill> history = new List<PlayerSkill>();

    public Action<Difficulty, float, Difficulty> OnUpdate;

    public PlayerSkill(string json) : base(json) {
    }

    public PlayerSkill(PlayerSkill other) : base(other) {
    }

    public PlayerSkill(Difficulty diff) : base(diff) {
    }

    public PlayerSkill() : base(Initial(0f)) {
    }

    public PlayerSkill First() {
        return history[0];
    }

    public PlayerSkill Last() {
        return history[history.Count - 1];
    }

    private void Update(Difficulty difficulty, float score, Difficulty mask, float kFactor = 0.05f) {
        OnUpdate?.Invoke(difficulty, score, mask);
        history.Add(new PlayerSkill(this));
        foreach (DifficultyType type in Enum.GetValues(typeof(DifficultyType))) {
            float ra = map[type];
            float rb = difficulty.Get(type);
            float ea = 1 / (1 + Mathf.Pow(Mathf.Exp(1), -1.3489f + 1.9544f * (rb - ra)));
            //float ea = 1 / (1 + Mathf.Pow(10, (rb - ra) / 0.5f));
            map[type] += mask.Get(type) * kFactor * (score - ea);
            map[type] = Mathf.Clamp(map[type], 0f, 1f);
        }
    }

    public void IncreaseTo(Difficulty difficulty) {
        Update(difficulty, 1f, Initial(1f));
    }

    public void DecreaseTo(Difficulty difficulty, Difficulty cause) {
        Update(difficulty, 0f, cause);
    }

    public void Update() {

    }

    public void Save() {
        PlayerProfile.Instance.PlayerSkill = this;
    }
}

