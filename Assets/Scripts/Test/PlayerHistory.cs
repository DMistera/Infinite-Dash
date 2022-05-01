using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerHistory {
    private readonly List<PlayerHistoryEntry> entries = new List<PlayerHistoryEntry>();

    public void AddEntry(PlayerSkill skill, int score) {
        entries.Add(new PlayerHistoryEntry {
            Skill = new PlayerSkill(skill),
            Score = score
        });
    }

    public string ToCSV() {
        List<string> result = new List<string> {
            PlayerHistoryEntry.Header()
        };
        foreach (PlayerHistoryEntry entry in entries) {
            result.Add(entry.ToRow());
        }
        return string.Join("\n", result.ToArray());
    }

    public PlayerHistoryEntry Last() {
        if(entries.Count > 0) {
            return entries[entries.Count - 1];
        }
        else {
            return null;
        }
    }
}

