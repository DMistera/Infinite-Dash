using System.Collections;
using UnityEngine;


public class PlayPlayer : MonoBehaviour {

    private Player player;

    // Use this for initialization
    void Start() {
        player = GetComponent<Player>();
        player.OnDeath += () => {
            PlayerProfile.Instance.PlayerHistory.AddEntry(player.Skill, player.Score);
            PlayerProfile.Instance.SavePlayerHistory();
            GameManager.Instance.State = GameState.GAME_OVER;
        };
    }
}
