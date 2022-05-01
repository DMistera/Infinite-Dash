using UnityEngine;

public class VineMesh : MonoBehaviour {

    public Sprite[] sprites;

    private SpriteRenderer spriteRenderer;

    public void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Start() {
        spriteRenderer.sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
    }
}

