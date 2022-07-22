using System.Collections;
using UnityEngine;


public class MenuEvents : MonoBehaviour {

    public void Play() {
        GameManager.Instance.State = GameState.PLAY;
    }

    public void Export() {
        PlayerProfile.Instance.PlayerHistory.Export();
    }

    public void Test() {
        GameManager.Instance.State = GameState.TEST;
    }

    public void Help() {
        GameManager.Instance.State = GameState.HELP;
    }

    public void Quit() {
        Application.Quit();
    }
}
