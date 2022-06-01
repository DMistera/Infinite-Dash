using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerHistoryEntry {
    public PlayerSkill EntrySkill { get; set; }
    public PlayerSkill HighestSkill { get; set; }
    public PlayerSkill FinalSkill { get; set; }
    public int Score { get; set; }

    public float[] GetValues() {
        List<float> values = new List<float>();
        foreach (DifficultyType type in Enum.GetValues(typeof(DifficultyType))) {
            values.Add(FinalSkill.Get(type));
        }
        values.Add(EntrySkill.Rank());
        values.Add(HighestSkill.Rank());
        values.Add(FinalSkill.Rank());
        values.Add(Score);
        return values.ToArray();
    }

    public string ToRow() {
        return string.Join(",", GetValues());
    }

    public static string Header() {
        List<string> values = new List<string>();
        foreach (DifficultyType type in Enum.GetValues(typeof(DifficultyType))) {
            values.Add(type.ToString().ToLower());
        }
        values.Add("Entry Rank");
        values.Add("Highest Rank");
        values.Add("Final Rank");
        values.Add("Score");
        return string.Join(",", values.ToArray());
    }
}

