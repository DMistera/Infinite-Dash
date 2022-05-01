﻿using System.Collections;
using System.IO;
using UnityEngine;

public class Tester : MonoBehaviour {

    public Track track;
    public int iterations = 100;
    public string path;
    private int iteration = 1;
    private PlayerHistory history = new PlayerHistory();

    // Use this for initialization
    void Start() {
        track.OnReset += HandleTrackReset;
    }

    private void HandleTrackReset() {
        PlayerHistoryEntry last = history.Last();
        if (last != null) {
            track.Player.Skill = last.Skill;
        }
        else {
            track.Player.Skill = new PlayerSkill(Difficulty.Initial(0f));
        }
        track.Player.OnDeath += () => {
            history.AddEntry(track.Player.Skill, track.Player.Score);
            if (iteration == iterations) {
                SaveTestResult();
                Debug.Log($"Test completed. Results saved in {path}");
                GameManager.Instance.State = GameState.MENU;
            } else {
                iteration++;
                Debug.Log($"Testing: {iteration}/{iterations}");
            }
        };
    }

    void SaveTestResult() {
        File.WriteAllText(path + "TestResult.csv", history.ToCSV());
    }
}
