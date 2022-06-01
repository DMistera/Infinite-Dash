using System.Collections;
using UnityEngine;


public class Cloud : ParallaxEntity {

    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;

    public void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    public override void Start() {
        base.Start();

        parallaxEffect = UnityEngine.Random.Range(0.3f, 0.98f);
        float size = (1f - parallaxEffect) * 3f;
        transform.localScale = new Vector3(size, size, size);
        spriteRenderer.sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
        Color c = spriteRenderer.color;
        c.a = (1f - parallaxEffect) * 0.2f;
        spriteRenderer.color = c;
    }
}
