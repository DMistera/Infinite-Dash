using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SimpleFileBrowser;

public class PlayerHistory {
    private readonly List<PlayerHistoryEntry> entries = new List<PlayerHistoryEntry>();

    public void AddEntry(PlayerSkill skill, int score) {
        entries.Add(new PlayerHistoryEntry {
            EntrySkillRank = new PlayerSkill(skill.First()).Rank(),
            HighestSkillRank = new PlayerSkill(skill.Last()).Rank(),
            FinalSkill = new PlayerSkill(skill),
            Score = score
        });
    }

    public void AddEntriesFromCSV(string csv) {
        foreach(string line in csv.Split('\n').Skip(1)) {
            entries.Add(new PlayerHistoryEntry(line));
        }
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

    public void Export() {
        FileBrowser.ShowSaveDialog((string[] paths) => {
            File.WriteAllText(paths[0], ToCSV());
        }, null, FileBrowser.PickMode.Files, title: "Zapisz wyniki", initialFilename: "wyniki.csv", allowMultiSelection: false);
        //string path = EditorUtility.SaveFilePanel("Zapisz wyniki", "", "wyniki.csv", "csv");
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

