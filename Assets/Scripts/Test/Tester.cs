using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Tester : MonoBehaviour {

    public Track track;
    public int iterations = 100;
    public string path;
    public DifficultyEntry[] initialDifficulty;
    public bool updateSkill = true;

    private int iteration = 1;
    private readonly PlayerHistory history = new PlayerHistory();
    private readonly EloMatchCollection eloMatchCollection = new EloMatchCollection();

    // Use this for initialization
    void Start() {
        track.OnReset += HandleTrackReset;
    }

    private void HandleTrackReset() {
        PlayerHistoryEntry last = history.Last();
        if (last != null) {
            if (updateSkill) {
                track.Player.Skill = new PlayerSkill(last.FinalSkill);
            }
            else {
                track.Player.Skill = new PlayerSkill(new Difficulty(initialDifficulty));
            }
        }
        else {
            track.Player.Skill = new PlayerSkill(new Difficulty(initialDifficulty));
        }
        track.Player.OnDeath += () => {
            history.AddEntry(track.Player.Skill, track.Player.Score);
            if (iteration == iterations) {
                SaveTestResult();
                Debug.Log($"Test completed. Results saved in {path}");
                GameManager.Instance.State = GameState.MENU;
            } else {
                iteration++;
                Debug.Log($"Testing: {iteration - 1}/{iterations}");
            }
        };
        track.Player.Skill.OnUpdate += (Difficulty diff, float score, Difficulty mask) => {
            eloMatchCollection.AddMatches(track.Player.Skill, diff, score, mask);
        };
    }

    void SaveTestResult() {
        string p = path + "TestResult" + DateTime.Now.ToString().Replace("/", "-").Replace(":", "-").Replace(" ", "");
        File.WriteAllText(p + ".csv", history.ToCSV());
        File.WriteAllText(p + "elo" + ".csv", eloMatchCollection.ToCSV());
    }
}
