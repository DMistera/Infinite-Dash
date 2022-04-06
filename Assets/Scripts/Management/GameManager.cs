 using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }
    private static GameManager instance;

    public GameState State { 
        get {
            return state;
        } 
        set {
            if (state != value) {
                OnStateExit?.Invoke(state);
                state = value;
                OnStateEnter?.Invoke(state);
            }
        } 
    }
    private GameState state = GameState.LOADING;

    public Action<GameState> OnStateEnter;
    public Action<GameState> OnStateExit;

}
