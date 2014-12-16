using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenuManagerScript : MonoBehaviour
{
    //Singletonisation
    public static MainMenuManagerScript currentMainMenuManagerScript;

    [SerializeField]
    Button _playButton;

    [SerializeField]
    Button _aboutButton;

    [SerializeField]
    Button _howToPlayButton;

    [SerializeField]
    Button _exitGameButton;

    [SerializeField]
    string _gameScene;

    [SerializeField]
    string _lobbyScene;

    void Awake()
    {
        if (currentMainMenuManagerScript == null)
        {
            DontDestroyOnLoad(gameObject);
            currentMainMenuManagerScript = this;
        }
        else if (currentMainMenuManagerScript != null)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("Starting the game (" + (NetworkManagerScript.currentNetworkManagerScript._isServer ? "Server mode" : "Client mode") + "), waiting for a player input on a button");

        //Specifies that the Application should be running when in background (mandatory if multiple instances)
        Application.runInBackground = true;

        initScene();
    }

    public void initScene()
    {
        if (NetworkManagerScript.currentNetworkManagerScript._isServer)
        {
            NetworkManagerScript.currentNetworkManagerScript.runServer();
        }
        else
        {
            _playButton.onClick.AddListener(() => { NetworkManagerScript.currentNetworkManagerScript.TryToConnectToServer(); });
        }

        _aboutButton.onClick.AddListener(() => { displayAboutInformations(); });
        _howToPlayButton.onClick.AddListener(() => { displayHowToPlayInformations(); });
        _exitGameButton.onClick.AddListener(() => { exitGame(); });
    }


    public void Update()
    {

    }

    //TODO
    void displayAboutInformations()
    {
        Debug.Log("Display about informations");
    }

    //TODO
    void displayHowToPlayInformations()
    {
        Debug.Log("Display howToPlay informations");
    }

    void exitGame()
    {
        Debug.Log("Exiting the game ...");

        //Doesn't work in Editor mode
        Application.Quit();
    }

    public void onServerInitialized()
    {
        SceneStateManager.currentStateManager.setNewSceneState(SceneStateManager.sceneState.Lobby);
        Application.LoadLevel(_lobbyScene);
    }


    public void failedToConnect(NetworkConnectionError error)
    {
        Debug.Log("Couldn't connect to the server : " + error);
        //Display an error message, and asks if the player wants to retry to connect
    }

    public void onConnectedToServer()
    {
        Debug.Log("Joining the Lobby");

        Network.SetSendingEnabled(0, false);
        Network.isMessageQueueRunning = false;
        SceneStateManager.currentStateManager.setNewSceneState(SceneStateManager.sceneState.Lobby);

        Application.LoadLevel(_lobbyScene);

        Network.isMessageQueueRunning = true;
        Network.SetSendingEnabled(0, true);
    }

    public void disconnectedFromServer(NetworkDisconnection networkDisconnection)
    {
        if (NetworkManagerScript.currentNetworkManagerScript._isServer)
        {
            //  _playButtonText.text = "Run Server";
            Debug.Log("Server disconnection successful : " + networkDisconnection);
        }
        else
        {
            Debug.Log("Disconnected from the server : " + networkDisconnection);
        }
    }
}