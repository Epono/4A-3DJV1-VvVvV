using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NetworkManagerScript : MonoBehaviour {

    public static NetworkManagerScript currentNetworkManagerScript;

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

    // Updated when needed
    public MainMenuManagerScript _mainMenuManagerScript;
    public LobbyManagerScript _lobbyManagerScript;
    public GameManagerScript _gameManagerScript;

    private bool isServerRunning = false;

    void Awake() {
        if(currentNetworkManagerScript == null) {
            DontDestroyOnLoad(gameObject);
            currentNetworkManagerScript = this;
        } else if(currentNetworkManagerScript != null) {
            Destroy(gameObject);
        }
    }

    void Update() {
        // Check players connections ?
    }

    //Server (Runs the server)
    public void runServer() {
        if(!isServerRunning) {
            Debug.Log("Trying to initialize the server ...");
            Network.InitializeSecurity();
            Network.InitializeServer(_maxNumberOfConnections, _serverPort, !Network.HavePublicAddress());
        } else {
            Debug.Log("Closing the server ...");
            Network.Disconnect();
        }
    }

    //Server (server finished initializing, launch lobbyScene)
    void OnServerInitialized() {
        isServerRunning = true;
        SceneStateManager.currentStateManager.loadLevel(SceneStateManager.sceneState.Lobby);
    }

    //Server (a player connected)
    void OnPlayerConnected(NetworkPlayer player) {
        switch(SceneStateManager.currentStateManager.getCurrentSceneState()) {
            case SceneStateManager.sceneState.MainMenu:
                // Shouldn't happen (not connected while in MainMenu)
                break;
            case SceneStateManager.sceneState.Lobby:
                // Add the player to the persistentPlayersScript
                Debug.Log("Player [" + player.ipAddress + "] connected");
                _lobbyManagerScript.serverPlayerJoined(player);
                break;
            case SceneStateManager.sceneState.Game:
                // If it's a reconnection, add him to the game, shouldn't happen otherwise
                break;
        }
    }

    //Server (a player disconnected)
    void OnPlayerDisconnected(NetworkPlayer player) {
        Debug.Log("Player disconnected : " + player.guid);
        switch(SceneStateManager.currentStateManager.getCurrentSceneState()) {
            case SceneStateManager.sceneState.MainMenu:
                // Shouldn't happen (not connected while in MainMenu)
                break;
            case SceneStateManager.sceneState.Lobby:
                // Wait (a bit) for him to reconnect ?
                _lobbyManagerScript.serverPlayerDisconnected(player);
                break;
            case SceneStateManager.sceneState.Game:
                // Wait (a bit longer) for him to reconnect ?
                break;
        }
    }

    //Client (tries to connect to the server)
    public void TryToConnectToServer() {
        Debug.Log("Trying to connect to the server ...");
        Network.Connect(_serverAdress, _serverPort);
    }

    //Client
    void OnFailedToConnect(NetworkConnectionError error) {
        switch(SceneStateManager.currentStateManager.getCurrentSceneState()) {
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
    void OnConnectedToServer() {
        switch(SceneStateManager.currentStateManager.getCurrentSceneState()) {
            case SceneStateManager.sceneState.MainMenu:
                // Go to Lobby (handled by server) 
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
    void OnDisconnectedFromServer(NetworkDisconnection networkDisconnection) {
        if(_isServer) {
            Debug.Log("Server closed");
            isServerRunning = false;
        } else {
            Debug.Log("Disconnected from server");
        }

        switch(SceneStateManager.currentStateManager.getCurrentSceneState()) {
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

    [RPC]
    public void LoadLobby() {
        Debug.Log("RPC received, joining lobby");
        SceneStateManager.currentStateManager.loadLevel(SceneStateManager.sceneState.Lobby);
    }
}
