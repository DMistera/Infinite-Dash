using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ChunkTrigger : MonoBehaviour {


    private Chunk chunk;
    public BoxCollider2D Collider { get; private set; }

    public void Awake() {
        Collider = GetComponent<BoxCollider2D>();
        chunk = GetComponentInParent<Chunk>();
    }

    public void OnTriggerEnter2D(Collider2D collider2D) {
        if(chunk.Ready) {
            Player player = collider2D.GetComponent<Player>();
            if (player != null) {
                chunk.OnPlayerEnter?.Invoke(player);
            }
        }
    }
    public void OnTriggerExit2D(Collider2D collider2D) {
        if (chunk.Ready) {
            Player player = collider2D.GetComponent<Player>();
            if (player != null) {
                chunk.OnPlayerLeave?.Invoke(player);
            }
        }
    }

    public void UpdateShape(float width) {
        Collider.size = new Vector2(width, 2000f);
        Collider.offset = new Vector2(width / 2f, 0f);
    }
}
