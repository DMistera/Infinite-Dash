using System.Collections;
using UnityEngine;


public class GameCamera : MonoBehaviour {

    public static GameCamera Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<GameCamera>();
            }
            return instance;
        }
    }
    private static GameCamera instance;

    public float offsetX = 5f;
    public float thresholdY = 5f;

    // Use this for initialization
    void Start() {
    }

    void Update() {
    }

    public void UpdateForPlayer(Player player) {
        UpdateX(player);
        UpdateY(player);
    }

    private void UpdateX(Player player) {
        Vector3 position = transform.position;
        position.x = player.transform.position.x + offsetX;
        transform.position = position;
    }

    private void UpdateY(Player player) {
        Vector3 position = transform.position;
        if(player.transform.position.y > position.y + thresholdY) {
            position.y = player.transform.position.y - thresholdY;
        }
        if (player.transform.position.y < position.y - thresholdY) {
            position.y = player.transform.position.y + thresholdY;
        }
        transform.position = position;
    }
}
