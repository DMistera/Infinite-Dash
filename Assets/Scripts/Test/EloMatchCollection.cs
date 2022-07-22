using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EloMatchCollection {

    private readonly List<EloMatch> matches = new List<EloMatch>();
    private static readonly float RANGE = 2f;
    private static readonly int POINT_COUNT = 40;
    private static readonly DifficultyType[] BANNED_TYPES = new DifficultyType[] { DifficultyType.TRAP_FREQUENCY };

    public void AddMatches(PlayerSkill skill, Difficulty difficulty, float score, Difficulty mask) {
        foreach (DifficultyType type in Enum.GetValues(typeof(DifficultyType))) {
            if (!BANNED_TYPES.Contains(type) && mask.Get(type) > 0.5f) {
                float diff = difficulty.Get(type) - skill.Get(type);
                matches.Add(new EloMatch {
                    Difference = diff,
                    Score = score,
                });
            }
        }
    }

    private List<Vector2> CreateChart() {
        float step = RANGE / POINT_COUNT;
        List < Vector2 > result = new List<Vector2> ();
        for (float x = -RANGE/2f; x < RANGE/2f; x += step) {
            float sum = 0;
            float count = 0;
            foreach (EloMatch match in matches) {
                if (match.Difference < x + step / 2f && match.Difference > x - step / 2f) {
                    sum += match.Score;
                    count++;
                }
            }
            if(count > 0) {
                result.Add(new Vector2(x, sum / count));
            }
        }
        return result;
    }

    /*public string ToCSV() {
        List<string> result = new List<string>();
        foreach (Vector2 v in CreateChart()) {
            result.Add(v.x + "," + v.y);
        }
        return string.Join("\n", result.ToArray());
    }*/

    public string ToCSV() {
        List<string> result = new List<string>();
        foreach (EloMatch match in matches) {
            result.Add(match.ToString());
        }
        return string.Join("\n", result.ToArray());
    }
}

