using System.Collections;
using UnityEngine;


public class MenuEvents : MonoBehaviour {

    public void Play() {
        GameManager.Instance.State = GameState.PLAY;
    }

    public void Test() {
        GameManager.Instance.State = GameState.TEST;
    }
}
