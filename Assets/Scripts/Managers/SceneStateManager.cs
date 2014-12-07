using UnityEngine;
using System.Collections;

public class SceneStateManager : MonoBehaviour
{
    public static SceneStateManager currentStateManager;

    private static sceneState _currentSceneState;

    public enum sceneState
    {
        MainMenu = 0,
        Lobby = 1,
        Game = 2,
    };

    void Awake()
    {
        if (currentStateManager == null)
        {
            DontDestroyOnLoad(gameObject);
            currentStateManager = this;
        }
        else if (currentStateManager != null)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        _currentSceneState = sceneState.MainMenu;
    }

    public sceneState getCurrentSceneState()
    {
        return _currentSceneState;
    }

    public void setNewSceneState(sceneState newState)
    {
        _currentSceneState = newState;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
