using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LobbyManagerScript : MonoBehaviour
{
    //Singletonisation
    public static LobbyManagerScript currentLobbyManagerScript;

    [SerializeField]
    private Text[] _playersText;

    [SerializeField]
    Button _exitLobbyButton;

    [SerializeField]
    Button _startGame;

    [SerializeField]
    NetworkView _networkView;

    [SerializeField]
    string _gameScene;

    [SerializeField]
    string _mainMenuScene;

    void Awake()
    {
        if (currentLobbyManagerScript == null)
        {
            DontDestroyOnLoad(gameObject);
            currentLobbyManagerScript = this;
        }
        else if (currentLobbyManagerScript != null)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _startGame.onClick.AddListener(() => { Application.LoadLevel(2); });
        _exitLobbyButton.onClick.AddListener(() => { exitLobby(); });
        if (NetworkManagerScript.currentNetworkManagerScript._isServer)
        {
            Debug.Log("Server created, waiting for players ...");
        }
        else
        {
            Debug.Log("Lobby joined, waiting for other players ...");
        }

        
    }

   
    public void exitLobby()
    {
        if (NetworkManagerScript.currentNetworkManagerScript._isServer)
        {
            // Close server and quit
            Debug.Log("Closing server and quitting ...");
            Network.Disconnect();
            Application.Quit();
        }
        else
        {
            //Disconnect from server and go back to MainMenuScene
            Debug.Log("Quitting lobby and going back to MainMenu ...");
            Network.Disconnect();
            SceneStateManager.currentStateManager.setNewSceneState(SceneStateManager.sceneState.MainMenu);
            Application.LoadLevel(_mainMenuScene);
        }
    }


    // Server-only method
    public void serverPlayerJoined(NetworkPlayer player)
    {
        PersistentPlayersScript.currentPersistentPlayersScript.playerConnected(player);
        serverRefreshPlayersText();

        if (PersistentPlayersScript.currentPersistentPlayersScript.getPlayers().Count == NetworkManagerScript.currentNetworkManagerScript._maxNumberOfConnections)
        {
            StartCoroutine(wait3s());
        }
    }

    public IEnumerator wait3s()
    {
        Debug.Log("Lobby full, launching the game in 3 ...");
        yield return new WaitForSeconds(1);
        Debug.Log("2 ...");
        yield return new WaitForSeconds(1);
        Debug.Log("1 ...");
        yield return new WaitForSeconds(1);
        Debug.Log("Starting !");
        _networkView.RPC("clientLaunchGameScene", RPCMode.All);
        Application.LoadLevel(_gameScene);
    }



    // Server-only method
    public void serverPlayerDisconnected(NetworkPlayer player)
    {
        PersistentPlayersScript.currentPersistentPlayersScript.playerDisconnected(player);
        serverRefreshPlayersText();
    }

    // Server-only method
    public void serverRefreshPlayersText()
    {
        for (int i = 0; i < PersistentPlayersScript.currentPersistentPlayersScript.getPlayers().Count; i++)
        {
            _playersText[i].text = PersistentPlayersScript.currentPersistentPlayersScript.getPlayers()[i].guid;
        }
    }

    //Client-only method
    [RPC]
    public void clientLaunchGameScene()
    {
        Network.SetSendingEnabled(0, false);

        // We need to stop receiving because first the level must be loaded first.
        // Once the level is loaded, rpc's and other state update attached to objects in the level are allowed to fire
        Network.isMessageQueueRunning = false;

        // All network views loaded from a level will get a prefix into their NetworkViewID.
        // This will prevent old updates from clients leaking into a newly created scene.
        SceneStateManager.currentStateManager.setNewSceneState(SceneStateManager.sceneState.Game);
        Application.LoadLevel(_gameScene);
        // yield;
        // yield;

        // Allow receiving data again
        Network.isMessageQueueRunning = true;
        // Now the level has been loaded and we can start sending out data to clients
        Network.SetSendingEnabled(0, true);
    }

}
