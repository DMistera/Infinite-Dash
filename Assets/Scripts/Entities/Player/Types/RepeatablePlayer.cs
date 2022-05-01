using System.Collections;
using UnityEngine;


public class RepeatablePlayer : MonoBehaviour {

    private Player player;

    // Use this for initialization
    void Start() {
        player = GetComponent<Player>();
        player.OnDeath += () => {
            Track track = GetComponentInParent<Track>();
            track.Reset();
        };
    }
}
