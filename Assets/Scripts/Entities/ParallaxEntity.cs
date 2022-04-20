using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ParallaxEntity : Entity {
    public float parallaxEffect = 1f;

    private new Camera camera;
    private Vector3 startPosition;

    public virtual void Start() {
        camera = Camera.main;
        startPosition = transform.position;
        GameCamera.Instance.OnUpdatePosition += UpdatePosition;
    }

    public void OnDestroy() {
        GameCamera.Instance.OnUpdatePosition -= UpdatePosition;
    }

    private void UpdatePosition() {
        float distX = GameCamera.Instance.transform.position.x * parallaxEffect;
        float distY = GameCamera.Instance.transform.position.y * parallaxEffect;
        transform.position = new Vector3(startPosition.x + distX, startPosition.y + distY, transform.position.z);
    }
}

