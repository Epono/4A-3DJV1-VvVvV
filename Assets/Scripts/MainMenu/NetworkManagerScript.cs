using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NetworkManagerScript : MonoBehaviour
{
    //Singleton
    public static NetworkManagerScript currentNetworkManagerScript;

    //To ease the configuration
    [SerializeField]
    int _serverPort;

    [SerializeField]
    string _serverAdress;

    [SerializeField]
    public int _maxNumberOfConnections;

    [SerializeField]
    public NetworkView _networkView;

    [SerializeField]
    public bool _isServer;

    //[SerializeField]
    // MainMenuManagerScript _mainMenuManagerScript;

    // [SerializeField]
    // PersistentPlayersScript _persistentPlayersScript;

    //[SerializeField]
    //string _lobbyScene;

    private bool _isServerRunning = false;

    void Awake()
    {
        if (currentNetworkManagerScript == null)
        {
            DontDestroyOnLoad(gameObject);
            currentNetworkManagerScript = this;
        }
        else if (currentNetworkManagerScript != null)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Server (Runs the server)
    public void runServer()
    {
        if (!_isServerRunning)
        {
            Debug.Log("Trying to initialize the server ...");
            Network.InitializeSecurity();
            Network.InitializeServer(_maxNumberOfConnections, _serverPort, true);
        }
        else
        {
            Debug.Log("Closing the server ...");
            Network.Disconnect();
        }
    }

    //Server (server finished initializing, launch lobbyScene)
    void OnServerInitialized()
    {
        _isServerRunning = true;
        MainMenuManagerScript.currentMainMenuManagerScript.onServerInitialized();
    }

    //Server (a player connected)
    void OnPlayerConnected(NetworkPlayer player)
    {
        switch (SceneStateManager.currentStateManager.getCurrentSceneState())
        {
            case SceneStateManager.sceneState.MainMenu:
                // Shouldn't happen (not connected while in MainMenu)
                break;
            case SceneStateManager.sceneState.Lobby:
                // Add the player to the persistentPlayersScript
                Debug.Log("Player [" + player.ipAddress + "] connected");
                LobbyManagerScript.currentLobbyManagerScript.serverPlayerJoined(player);
                break;
            case SceneStateManager.sceneState.Game:
                // If it's a reconnection, add him to the game, shouldn't happen otherwise
                break;
        }
    }

    //Server (a player disconnected)
    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("Player disconnected : " + player.guid);
        //LobbyManagerScript.currentLobbyManagerScript.playerDisconnected(player);
        switch (SceneStateManager.currentStateManager.getCurrentSceneState())
        {
            case SceneStateManager.sceneState.MainMenu:
                // Shouldn't happen (not connected while in MainMenu)
                break;
            case SceneStateManager.sceneState.Lobby:
                // Wait (a bit) for him to reconnect ?
                LobbyManagerScript.currentLobbyManagerScript.serverPlayerDisconnected(player);
                break;
            case SceneStateManager.sceneState.Game:
                // Wait (a bit longer) for him to reconnect ?
                break;
        }
    }

    //Client (tries to connect to the server)
    public void TryToConnectToServer()
    {
        Debug.Log("Trying to connect to the server ...");
        Network.Connect(_serverAdress, _serverPort);
    }

    //Client
    void OnFailedToConnect(NetworkConnectionError error)
    {
        //LobbyManagerScript.currentLobbyManagerScript.failedToConnect(error);
        switch (SceneStateManager.currentStateManager.getCurrentSceneState())
        {
            case SceneStateManager.sceneState.MainMenu:
                // Couldn't connect, display error message and retry
                break;
            case SceneStateManager.sceneState.Lobby:
                // shouldn't happen (can't connect during (local) Lobby state)
                break;
            case SceneStateManager.sceneState.Game:
                // shouldn't happen (can't connect during (local) Game state)
                break;
        }
    }

    //Client
    void OnConnectedToServer()
    {
        switch (SceneStateManager.currentStateManager.getCurrentSceneState())
        {
            case SceneStateManager.sceneState.MainMenu:
                // Go to Lobby
                Debug.Log("Connection to the server successful !");
                MainMenuManagerScript.currentMainMenuManagerScript.onConnectedToServer();
                break;
            case SceneStateManager.sceneState.Lobby:
                // Shouldn't happen
                break;
            case SceneStateManager.sceneState.Game:
                //Shouldn't happen
                break;
        }
    }

    //Server (gets called when the server finishes closing => normal) + Client (gets called when disconnected from server => abnormal)
    void OnDisconnectedFromServer(NetworkDisconnection networkDisconnection)
    {
        if (_isServer)
        {
            Debug.Log("Server closed");
            _isServerRunning = false;
        }
        else
        {
            Debug.Log("Disconnected from server");
        }

        switch (SceneStateManager.currentStateManager.getCurrentSceneState())
        {
            case SceneStateManager.sceneState.MainMenu:
                // Shouldn't happen (not connected while in MainMenu)
                break;
            case SceneStateManager.sceneState.Lobby:
                // Try to reconnect/evicts from the lobby after a while
                break;
            case SceneStateManager.sceneState.Game:
                // Try to reconnect/evicts from the game after a while
                break;
        }
    }
}
