using System.Collections;
using UnityEngine;


public class Sawblade : Hazard {

    public SawbladeVariant[] variants;
    private SawbladeMesh mesh;

    // Use this for initialization
    void Awake() {
        mesh = GetComponentInChildren<SawbladeMesh>();
    }

    public void Init() {
        SawbladeVariant variant = variants[UnityEngine.Random.Range(0, variants.Length)];
        mesh.SetSprite(variant.sprite);
        transform.localScale = new Vector3(variant.size, variant.size, 0);
    }
}
