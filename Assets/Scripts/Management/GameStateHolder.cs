using System;
using System.Collections;
using UnityEngine;

public class GameStateHolder : MonoBehaviour {

    public static GameStateHolder Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<GameStateHolder>();
            }
            return instance;
        }
    }
    private static GameStateHolder instance;

    public GameState State { 
        get {
            return state;
        } 
        set {
            if (state != value) {
                OnExit?.Invoke(state);
                state = value;
                OnEnter?.Invoke(state);
            }
        } 
    }
    private GameState state = GameState.LOADING;

    public Action<GameState> OnEnter;
    public Action<GameState> OnExit;


}
