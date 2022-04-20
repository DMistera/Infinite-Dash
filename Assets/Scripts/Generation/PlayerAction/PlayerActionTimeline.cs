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
        actions.Add(new SlideAction {
            Length = 5,
            Type = PlayerActionType.SLIDE,
        });
        PlayerActionType type = NextType(PlayerActionType.SLIDE); ;
        for (int i = 0; i < 10; i++) {
            actions.Add(GenerateNextAction(type));
            type = NextType(type);
        }
    }

    public static PlayerActionTimeline First() {
        PlayerAction firstAction = new SlideAction() {
            Type = PlayerActionType.SLIDE,
            Length = 10
        };
        List<PlayerAction> actions = new List<PlayerAction> {
            firstAction
        };
        return new PlayerActionTimeline(actions);
    }

    private PlayerAction GenerateNextAction(PlayerActionType type) {
        switch (type) {
            case PlayerActionType.SLIDE:
                return new SlideAction {
                    Length = GenerateLength(),
                    Type = type
                };
            case PlayerActionType.JUMP:
            case PlayerActionType.DOUBLE_JUMP:
            case PlayerActionType.FALL:
                return new AirAction {
                    LevelDifference = GenerateLevelDifference(type),
                    Type = type
                };
            default:
                throw new NotImplementedException();
        }
    }

    private int GenerateLevelDifference(PlayerActionType type) {
        return type switch {
            PlayerActionType.JUMP => UnityEngine.Random.Range(0, 3),
            PlayerActionType.DOUBLE_JUMP => UnityEngine.Random.Range(-1, 3),
            PlayerActionType.FALL => UnityEngine.Random.Range(-3, 0),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private int GenerateLength() {
        return UnityEngine.Random.Range(1, 6);
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

