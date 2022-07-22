using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI {
    public class ButtonDecorator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        public GameObject decorationPrefab;

        private GameObject leftDecoration;
        private GameObject rightDecoration;

        private RectTransform rectTransform;
        private float farDist;
        private float closeDist;

        private readonly float time = 0.2f;
        private readonly LeanTweenType easing = LeanTweenType.easeOutCubic;

        // Use this for initialization
        void Start() {
            rectTransform = GetComponent<RectTransform>();
            farDist = rectTransform.sizeDelta.x / 2f + 30f;
            closeDist = rectTransform.sizeDelta.x / 2f;



            leftDecoration = Instantiate(decorationPrefab, transform);
            leftDecoration.transform.localPosition = new Vector3(-farDist, 0, 0);

            rightDecoration = Instantiate(decorationPrefab, transform);
            rightDecoration.transform.localPosition = new Vector3(farDist, 0, 0);
        }

        public void OnPointerEnter(PointerEventData data) {
            leftDecoration.GetComponent<RectTransform>().LeanMoveX(-closeDist, time).setEase(easing);
            rightDecoration.GetComponent<RectTransform>().LeanMoveX(closeDist, time).setEase(easing);
        }

        public void OnPointerExit(PointerEventData data) {
            leftDecoration.GetComponent<RectTransform>().LeanMoveX(-farDist, time).setEase(easing);
            rightDecoration.GetComponent<RectTransform>().LeanMoveX(farDist, time).setEase(easing);
        }

    }
}