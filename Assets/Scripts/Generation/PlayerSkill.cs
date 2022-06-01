using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerSkill : Difficulty {

    private readonly float STEEPNESS = 10f;
    private readonly float LOOSE_K_FACTOR = 0.05f;
    private readonly float WIN_K_FACTOR = 0.01f;
    private readonly float DIFF = 0.5f;

    private List<PlayerSkill> history = new List<PlayerSkill>();

    public Action<Difficulty, float, Difficulty> OnUpdate;

    public PlayerSkill(string json) : base(json) {
    }

    public PlayerSkill(PlayerSkill other) : base(other) {
    }

    public PlayerSkill(Difficulty diff) : base(diff) {
    }

    public PlayerSkill First() {
        return history[0];
    }

    public PlayerSkill Last() {
        return history[history.Count - 1];
    }

    private void Update(Difficulty difficulty, float score, Difficulty mask, float kFactor) {
        OnUpdate?.Invoke(difficulty, score, mask);
        history.Add(new PlayerSkill(this));
        foreach (DifficultyType type in Enum.GetValues(typeof(DifficultyType))) {
            float ra = map[type];
            float rb = difficulty.Get(type);
            float ea = 1 / (1 + Mathf.Pow(STEEPNESS, (rb - ra) / DIFF));
            map[type] += mask.Get(type) * kFactor  * (score - ea);
            map[type] = Mathf.Clamp(map[type], 0f, 1f);
        }
    }

    public void IncreaseTo(Difficulty difficulty) {
        Update(difficulty, 1f, Initial(1f), WIN_K_FACTOR);
    }

    public void DecreaseTo(Difficulty difficulty, Difficulty cause) {
        Update(difficulty, 0f, cause, LOOSE_K_FACTOR);
    }

    public void Update() {

    }

    public void Save() {
        PlayerProfile.Instance.PlayerSkill = this;
    }
}

