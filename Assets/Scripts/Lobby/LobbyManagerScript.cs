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

    void Update()
    {

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

        //_playersText[PersistentPlayersScript.currentPersistentPlayersScript.getPlayers().Count - 1].text = player.guid;
        serverRefreshPlayersText();

        if (PersistentPlayersScript.currentPersistentPlayersScript.getPlayers().Count == NetworkManagerScript.currentNetworkManagerScript._maxNumberOfConnections)
        {
            Debug.Log("Lobby full, launching the game ...");
            NetworkManagerScript.currentNetworkManagerScript._networkView.RPC("launchGameScene", RPCMode.OthersBuffered);
            Application.LoadLevel(_gameScene);
        }
    }

    // Server-only method
    public void serverPlayerDisconnected(NetworkPlayer player)
    {
        PersistentPlayersScript.currentPersistentPlayersScript.playerDisconnected(player);

        //_playersText[PersistentPlayersScript.currentPersistentPlayersScript.getPlayers().Count - 1].text = player.guid;
        serverRefreshPlayersText();

        if (PersistentPlayersScript.currentPersistentPlayersScript.getPlayers().Count == NetworkManagerScript.currentNetworkManagerScript._maxNumberOfConnections)
        {
            Debug.Log("Lobby full, launching the game ...");
            NetworkManagerScript.currentNetworkManagerScript._networkView.RPC("launchGameScene", RPCMode.OthersBuffered);
            Application.LoadLevel(_gameScene);
        }
    }

    // Server-only method
    public void serverRefreshPlayersText()
    {
        for (int i = 0; i < PersistentPlayersScript.currentPersistentPlayersScript.getPlayers().Count; i++)
        {
            _playersText[i].text = PersistentPlayersScript.currentPersistentPlayersScript.getPlayers()[i].guid;
        }
        //_playersText[PersistentPlayersScript.currentPersistentPlayersScript.getPlayers().Count - 1].text = player.guid;

    }

    //Client-only method
    public void clientLaunchGameScene()
    {
        Application.LoadLevel(_gameScene);
    }

}
