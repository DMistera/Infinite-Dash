using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class RankProvider : MonoBehaviour {

    public static RankProvider Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<RankProvider>();
            }
            return instance;
        }
    }
    private static RankProvider instance;

    public TextAsset rankData;

    private List<RankDataEntry> entries = new List<RankDataEntry>();

    // Use this for initialization
    void Start() {
        entries = JsonConvert.DeserializeObject<List<RankDataEntry>>(rankData.text);
    }

    public string GetTitle(float rank) {
        string result = entries[0].Title;
        foreach (RankDataEntry entry in entries) {
            if(entry.Min < rank) {
                result = entry.Title;
            }
            else {
                return result;
            }
        }
        return result;
    }

    public float GetTillNext(float rank) {
        foreach (RankDataEntry entry in entries) {
            if (entry.Min > rank) {
                return entry.Min - rank;
            }
        }
        throw new Exception("Rank title not found!");
    }

    private class RankDataEntry {
        public float Min { get; set; }
        public string Title { get; set; }
    }
}
