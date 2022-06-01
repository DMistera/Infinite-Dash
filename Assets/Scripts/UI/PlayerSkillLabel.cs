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
        string s = $"{prefix}: {Title()}\n";
        if (showDifference) {
            float diff = Diff()*1000;
            if (diff > 0) {
                s += $"Zdobyte punkty: <color=yellow>+{diff:0}</color> pkt\n";
            } else {
                s += $"Zdobyte punkty: <color=red>{diff:0}</color> pkt\n";
            }
            float tillNext = TillNext() * 1000;
            s += $"Do następnej rangi potrzeba: {tillNext:0} pkt\n";
        }
        text.text = s;
    }

    private string Title() {
        return PlayerProfile.Instance.PlayerSkill.Title();
    }

    private float Diff() {
        PlayerSkill skill = PlayerProfile.Instance.PlayerSkill;
        return skill.Rank() - skill.First().Rank();
    }

    private float TillNext() {
        return RankProvider.Instance.GetTillNext(PlayerProfile.Instance.PlayerSkill.Rank());
    }
}
