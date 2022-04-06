using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class PlayerActionTimeline {
    public List<PlayerAction> actions;

    public PlayerActionTimeline(List<PlayerAction> actions) {
        this.actions = actions;
    }
    public PlayerActionTimeline(Difficulty difficulty) {
        actions = new List<PlayerAction>();
        actions.Add(new PlayerAction {
            Value = 5,
            Type = PlayerActionType.SLIDE,
        });
        PlayerActionType type = NextType(PlayerActionType.SLIDE); ;
        for (int i = 0; i < 10; i++) {
            actions.Add(new PlayerAction {
                Value = GenerateValue(type),
                Type = type,
            });
            type = NextType(type);
        }
    }

    public static PlayerActionTimeline First() {
        PlayerAction firstAction = new PlayerAction() {
            Type = PlayerActionType.SLIDE,
            Value = 10
        };
        List<PlayerAction> actions = new List<PlayerAction> {
            firstAction
        };
        return new PlayerActionTimeline(actions);
    }

    private int GenerateValue(PlayerActionType type) {
        return type switch {
            PlayerActionType.JUMP => UnityEngine.Random.Range(-2, 3),
            PlayerActionType.DOUBLE_JUMP => UnityEngine.Random.Range(-2, 3),
            PlayerActionType.FALL => UnityEngine.Random.Range(-3, -1),
            PlayerActionType.SLIDE => UnityEngine.Random.Range(1, 5),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private float GetMean(PlayerActionType type) {
        return type switch {
            PlayerActionType.JUMP => 0.3f,
            PlayerActionType.FALL => 0.15f,
            PlayerActionType.SLIDE => 0.2f,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private PlayerActionType NextType(PlayerActionType previousType) {
        return previousType switch {
            PlayerActionType.JUMP => UnityEngine.Random.value < 0.75f ? PlayerActionType.SLIDE : PlayerActionType.DOUBLE_JUMP,
            PlayerActionType.FALL => UnityEngine.Random.value < 0.75f ? PlayerActionType.SLIDE : PlayerActionType.DOUBLE_JUMP,
            PlayerActionType.DOUBLE_JUMP => PlayerActionType.SLIDE,
            PlayerActionType.SLIDE => UnityEngine.Random.value < 0.75f ? PlayerActionType.JUMP : PlayerActionType.FALL,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}

