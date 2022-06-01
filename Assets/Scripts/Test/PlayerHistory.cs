using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerHistory {
    private readonly List<PlayerHistoryEntry> entries = new List<PlayerHistoryEntry>();

    public void AddEntry(PlayerSkill skill, int score) {
        entries.Add(new PlayerHistoryEntry {
            EntrySkill = new PlayerSkill(skill.First()),
            HighestSkill = new PlayerSkill(skill.Last()),
            FinalSkill = new PlayerSkill(skill),
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

