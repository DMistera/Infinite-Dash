using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PlayerSkillLabel : MonoBehaviour {

    private TextMeshProUGUI text;

    public string prefix = "Poziom umiejętności";
    public bool showDifference;

    void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Use this for initialization
    void Start() {
        string s = $"{prefix}: {Rank():0.000}";
        if (showDifference) {
            float diff = Diff();
            if (diff > 0) {
                s += $"(<color=yellow>+{diff:0.000}</color>)";
            } else {
                s += $"(<color=red>{diff:0.000}</color>)";
            }
        }
        text.text = s;
    }

    private float Rank() {
        return PlayerProfile.Instance.PlayerSkill.Rank();
    }

    private float Diff() {
        PlayerSkill skill = PlayerProfile.Instance.PlayerSkill;
        return skill.Rank() - skill.Previous().Rank();
    }
}
