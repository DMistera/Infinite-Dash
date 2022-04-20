using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Vine : Entity {

    public Sprite[] sprites;

    private SpriteRenderer spriteRenderer;

    public void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Start() {
        spriteRenderer.sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
    }
}

