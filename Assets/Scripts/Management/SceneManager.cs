using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {

    public SceneDescriptor[] sceneDescriptors;

    private readonly List<SceneDescriptor> loadedSceneDescriptors = new List<SceneDescriptor>();

    // Use this for initialization
    void Start() {
        GameManager.Instance.OnStateEnter += (state) => {

            ClearOldScenes(state);

            foreach (SceneDescriptor sceneDescriptor in sceneDescriptors) {
                if (Array.Exists(sceneDescriptor.gameStates, s => s == state) && !loadedSceneDescriptors.Contains(sceneDescriptor)) {
                    Load(sceneDescriptor);
                }
            }
        };

        Load(sceneDescriptors[0]);
    }

    private void ClearOldScenes(GameState state) {
        List<SceneDescriptor> toRemove = new List<SceneDescriptor>();
        foreach (SceneDescriptor sceneDescriptor in loadedSceneDescriptors) {
            if (!Array.Exists(sceneDescriptor.gameStates, s => s == state)) {
                toRemove.Add(sceneDescriptor);
            }
        }

        foreach (SceneDescriptor sceneDescriptor in toRemove) {
            Unload(sceneDescriptor);
            loadedSceneDescriptors.Remove(sceneDescriptor);
        }
    }

    private void Load(SceneDescriptor descriptor) {
        UnityEngine.SceneManagement.LoadSceneParameters parameters = new UnityEngine.SceneManagement.LoadSceneParameters(UnityEngine.SceneManagement.LoadSceneMode.Additive, UnityEngine.SceneManagement.LocalPhysicsMode.Physics2D);
        UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.LoadScene(descriptor.sceneName, parameters);
        loadedSceneDescriptors.Add(descriptor);
    }

    private void Unload(SceneDescriptor descriptor) {
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(descriptor.sceneName);
    }
}
