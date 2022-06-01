using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class PlayerActionTimeline {
    public List<PlayerAction> actions;

    private readonly static float MAX_SLIDE_LENGTH = 6f;

    public PlayerActionTimeline(List<PlayerAction> actions) {
        this.actions = actions;
    }
    public PlayerActionTimeline(Difficulty difficulty) {
        actions = new List<PlayerAction> {
            new SlideAction {
                Length = 5,
                Type = PlayerActionType.SLIDE,
            }
        };
        PlayerActionType type = NextType(PlayerActionType.SLIDE, difficulty); ;
        for (int i = 0; i < 10; i++) {
            actions.Add(GenerateNextAction(type, difficulty));
            type = NextType(type, difficulty);
        }

        CorrectDoubleJumpDifficulty(difficulty);
        CorrectSlideLengthDifficulty(difficulty);
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

    private void CorrectDoubleJumpDifficulty(Difficulty difficulty) {
        int count = 0;
        int countAll = 0;
        foreach (PlayerAction action in actions) {
            switch (action.Type) {
                case PlayerActionType.JUMP: case PlayerActionType.FALL:
                    countAll++;
                    break;
                case PlayerActionType.DOUBLE_JUMP:
                    count++;
                    break;
            }
        }
        difficulty.Set(DifficultyType.DOUBLE_JUMP_FREQUENCY, (float)count / countAll);
    }

    private void CorrectSlideLengthDifficulty(Difficulty difficulty) {
        int sum = 0;
        int count = 0;
        foreach (PlayerAction action in actions) {
            if(action is SlideAction slideAction) {
                count++;
                sum += slideAction.Length;
            }
        }
        float n = ((MAX_SLIDE_LENGTH + 1) / 2f)  * count - sum;
        float d = (MAX_SLIDE_LENGTH - 1) * count;
        difficulty.Set(DifficultyType.SLIDE_LENGTH, n / d);
    }

    private PlayerAction GenerateNextAction(PlayerActionType type, Difficulty difficulty) {
        switch (type) {
            case PlayerActionType.SLIDE:
                return new SlideAction {
                    Length = GenerateLength(difficulty),
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

    private int GenerateLength(Difficulty difficulty) {
        float f = (1f - difficulty.Get(DifficultyType.SLIDE_LENGTH)) * UnityEngine.Random.Range(1f, MAX_SLIDE_LENGTH - 1f);
        return 1 + Mathf.RoundToInt(f);
    }

    private float GetMean(PlayerActionType type) {
        return type switch {
            PlayerActionType.JUMP => 0.3f,
            PlayerActionType.FALL => 0.15f,
            PlayerActionType.SLIDE => 0.2f,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private PlayerActionType NextType(PlayerActionType previousType, Difficulty difficulty) {
        return previousType switch {
            PlayerActionType.JUMP => UnityEngine.Random.value < difficulty.Get(DifficultyType.DOUBLE_JUMP_FREQUENCY) ? PlayerActionType.DOUBLE_JUMP : PlayerActionType.SLIDE,
            PlayerActionType.FALL => UnityEngine.Random.value < difficulty.Get(DifficultyType.DOUBLE_JUMP_FREQUENCY) ? PlayerActionType.DOUBLE_JUMP : PlayerActionType.SLIDE,
            PlayerActionType.DOUBLE_JUMP => PlayerActionType.SLIDE,
            PlayerActionType.SLIDE => UnityEngine.Random.value < 0.75f ? PlayerActionType.JUMP : PlayerActionType.FALL,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}

