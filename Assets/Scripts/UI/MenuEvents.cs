using System.Collections;
using UnityEngine;


public class MenuEvents : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Play() {
        GameManager.Instance.State = GameState.PLAY;
    }
}
