using TMPro;
using UnityEngine;


public class LoadingText : MonoBehaviour {

    public string prefix;
    private TextMeshProUGUI text;

    void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Use this for initialization
    void Start() {
        text.text = prefix;
        ChunkLibrary.Instance.OnLoadStep += (step) => {
            text.text = prefix + " (" + (step + 1) + "/" + ChunkLibrary.Instance.size + ")";
        };
    }
}
