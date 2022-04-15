using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SawbladeMesh : MonoBehaviour {

    public float rotationSpeed;
    private SpriteRenderer spriteRenderer;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Update() {
        transform.Rotate(new Vector3(0, 0, -rotationSpeed * Time.deltaTime));
    }

    public void SetSprite(Sprite sprite) {
        spriteRenderer.sprite = sprite;
    }
}

