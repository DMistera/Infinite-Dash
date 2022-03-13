using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI {
    public class GameOverEvents : MonoBehaviour {

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void Restart() {
            GameStateHolder.Instance.State = GameState.PLAY;
        }

        public void Menu() {
            GameStateHolder.Instance.State = GameState.MENU;
        }
    }
}