using System.Collections;
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

    public void Update() {
    }

    private PlayerSkill InitializePlayerSkill() {
        Difficulty defaultSkill = Difficulty.Initial(0f);
        string skillJson = PlayerPrefs.GetString(PlayerPrefsKey.PLAYER_SKILL.ToString(), defaultSkill.ToString());
        return new PlayerSkill(skillJson);
    }

    private void SavePlayerSkill(PlayerSkill playerSkill) {
        PlayerPrefs.SetString(PlayerPrefsKey.PLAYER_SKILL.ToString(), playerSkill.ToString());
    }
}
