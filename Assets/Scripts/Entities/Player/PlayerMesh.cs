using System.Collections;
using UnityEngine;

public class PlayerMesh : MonoBehaviour {

    public float rotationSpeed = 5.0f;
    IEnumerator flyCourotine;
    IEnumerator landCourotine;
    bool landCourotineLock = false;

    MeshRenderer meshRenderer;

    // Use this for initialization
    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        flyCourotine = Fly();
        landCourotine = Land();
    }

    IEnumerator Fly() {
        while (true) {
            transform.Rotate(rotationSpeed * Time.deltaTime * -Vector3.forward);
            yield return null;
        }
    }

    IEnumerator Land() {
        landCourotineLock = true;
        float deltaTime = 0.01f;
        float angle = transform.localEulerAngles.z;
        float target = Utils.Math.Round(angle, 90f);
        float duration = 0.07f;
        for (float t = 0f; t < duration; t += deltaTime) {
            float progress = t / duration;
            float v = (1 - progress) * angle + progress * target;
            transform.eulerAngles = new Vector3(0, 0, v);
            yield return new WaitForSeconds(deltaTime);
        }
        landCourotineLock = false;
    }

    public void StartRotation() {
        StopCoroutine(landCourotine);
        landCourotineLock = false;
        StartCoroutine(flyCourotine);
    }

    public void StopRotation() {
        StopCoroutine(flyCourotine);
        if(!landCourotineLock) {
            landCourotine = Land();
            StartCoroutine(landCourotine);
        }
    }

    public void Die() {
        float duration = 0.2f;
        LeanTween.alpha(gameObject, 0f, duration);
        LeanTween.scale(gameObject, new Vector3(2f, 2f, 2f), duration);
    }
}
