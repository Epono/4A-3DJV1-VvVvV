using UnityEngine;
using System.Collections;

public class SceneStateManager : MonoBehaviour {

    public static SceneStateManager currentStateManager;

    [SerializeField]
    string _mainMenuSceneName = "MainMenu";
    [SerializeField]
    string _lobbySceneName = "Lobby";
    [SerializeField]
    string _gameSceneName = "Game";

    public enum sceneState {
        MainMenu = 0,
        Lobby = 1,
        Game = 2,
    };

    private static sceneState _currentSceneState;

    void Awake() {
        if(currentStateManager == null) {
            DontDestroyOnLoad(gameObject);
            currentStateManager = this;
        } else if(currentStateManager != null) {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start() {
        _currentSceneState = sceneState.MainMenu;
    }

    public sceneState getCurrentSceneState() {
        return _currentSceneState;
    }

    public void loadLevel(sceneState newState) {
        _currentSceneState = newState;
        switch(newState) {
            case sceneState.MainMenu:
                Application.LoadLevel(_mainMenuSceneName);
                break;
            case sceneState.Lobby:
                Application.LoadLevel(_lobbySceneName);
                break;
            case sceneState.Game:
                Application.LoadLevel(_gameSceneName);
                break;
        }
    }
}
