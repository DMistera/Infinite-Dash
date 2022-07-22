using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PlayerSkillLabel : MonoBehaviour {

    private TextMeshProUGUI text;

    public string prefix = "Poziom umiejętności";
    public bool showDifference;

    private static float MULTIPLIER = 1000;

    void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Use this for initialization
    void Start() {
        string s = $"{prefix}: {Title()} ({Points():0}/{MaxPoints():0})\n";
        if (showDifference) {
            float diff = Diff();
            if (diff > 0) {
                s += $"Zdobyte punkty: <color=yellow>+{diff:0}</color> pkt\n";
            } else {
                s += $"Zdobyte punkty: <color=red>{diff:0}</color> pkt\n";
            }
            s += $"Do następnej rangi potrzeba: {TillNext():0} pkt\n";
        }
        text.text = s;
    }

    private string Title() {
        return PlayerProfile.Instance.PlayerSkill.Title();
    }

    private float Diff() {
        PlayerSkill skill = PlayerProfile.Instance.PlayerSkill;
        return (skill.Rank() - skill.First().Rank()) * MULTIPLIER;
    }

    private float TillNext() {
        return RankProvider.Instance.GetTillNext(PlayerProfile.Instance.PlayerSkill.Rank()) * MULTIPLIER;
    }

    private float Points() {
        return RankProvider.Instance.GetOverMin(PlayerProfile.Instance.PlayerSkill.Rank()) * MULTIPLIER;
    }

    private float MaxPoints() {
        return RankProvider.Instance.GetRange(PlayerProfile.Instance.PlayerSkill.Rank()) * MULTIPLIER;
    }
}
