using System.Collections;
using System.Globalization;
using UnityEngine;


public class PlayerProfile : MonoBehaviour {

    public static PlayerProfile Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<PlayerProfile>();
            }
            return instance;
        }
    }
    private static PlayerProfile instance;

    public PlayerHistory PlayerHistory { get; private set; } = new PlayerHistory();

    public PlayerSkill PlayerSkill {
        get {
            if (playerSkill == null) {
                playerSkill = InitializePlayerSkill();
            }
            return playerSkill;
        }
        set {
            SavePlayerSkill(value);
            playerSkill = value;
        }
    }
    private PlayerSkill playerSkill;

    public void Start() {
        string playerHistoryCsv = PlayerPrefs.GetString(PlayerPrefsKey.PLAYER_HISTORY.ToString());
        if(playerHistoryCsv.Length > 0) {
            PlayerHistory.AddEntriesFromCSV(playerHistoryCsv);
        }
    }

    private PlayerSkill InitializePlayerSkill() {
        Difficulty defaultSkill = Difficulty.Initial(0f);
        string skillJson = PlayerPrefs.GetString(PlayerPrefsKey.PLAYER_SKILL.ToString(), defaultSkill.ToString());
        return new PlayerSkill(skillJson);
    }

    private void SavePlayerSkill(PlayerSkill playerSkill) {
        PlayerPrefs.SetString(PlayerPrefsKey.PLAYER_SKILL.ToString(), playerSkill.ToString());
    }

    public void SavePlayerHistory() {
        PlayerPrefs.SetString(PlayerPrefsKey.PLAYER_HISTORY.ToString(), PlayerHistory.ToCSV());
    }
}
