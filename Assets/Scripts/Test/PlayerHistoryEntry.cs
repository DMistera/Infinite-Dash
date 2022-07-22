using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerHistoryEntry {
    public float EntrySkillRank { get; set; }
    public float HighestSkillRank { get; set; }
    public PlayerSkill FinalSkill { get; set; }
    public int Score { get; set; }

    public PlayerHistoryEntry() {
    }

    public PlayerHistoryEntry(string line) { 
        float[] values = line.Split(',').Select(x => {
            float p = float.Parse(x);
            if(float.IsNaN(p)) {
                return 0;
            }
            return p;
        }).ToArray();
        FinalSkill = new PlayerSkill();
        int i = 0;
        foreach (DifficultyType type in Enum.GetValues(typeof(DifficultyType))) {
            FinalSkill.Set(type, values[i]);
            i++;
        }
        EntrySkillRank = values[i++];
        HighestSkillRank = values[i++];
        Score = (int)values[++i];
    }

    public float[] GetValues() {
        List<float> values = new List<float>();
        foreach (DifficultyType type in Enum.GetValues(typeof(DifficultyType))) {
            values.Add(FinalSkill.Get(type));
        }
        values.Add(EntrySkillRank);
        values.Add(HighestSkillRank);
        values.Add(FinalSkill.Rank());
        values.Add(Score);
        return values.ToArray();
    }

    public string ToRow() {
        NumberFormatInfo nfi = new NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        string[] str = GetValues().Select(x => x.ToString(nfi)).ToArray();
        return string.Join(",", str);
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

