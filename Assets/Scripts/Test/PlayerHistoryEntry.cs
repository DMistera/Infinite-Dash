using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerHistoryEntry {
    public PlayerSkill Skill { get; set; }
    public int Score;

    public float[] GetValues() {
        List<float> values = new List<float>();
        foreach (DifficultyType type in Enum.GetValues(typeof(DifficultyType))) {
            values.Add(Skill.Get(type));
        }
        values.Add(Skill.Rank());
        values.Add(Score);
        return values.ToArray();
    }

    public string ToRow() {
        return string.Join(",", GetValues());
    }

    public static string Header() {
        List<string> values = new List<string>();
        foreach (DifficultyType type in Enum.GetValues(typeof(DifficultyType))) {
            values.Add(type.ToString().ToLowerInvariant());
        }
        values.Add("Rank");
        values.Add("Score");
        return string.Join(",", values.ToArray());
    }
}

