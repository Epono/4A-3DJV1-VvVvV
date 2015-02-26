using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NetworkManagerScript : MonoBehaviour {

    public static NetworkManagerScript currentNetworkManagerScript;

    [SerializeField]
    int _serverPort;

    public int ServerPort {
        get { return _serverPort; }
        set { _serverPort = value; }
    }

    [SerializeField]
    int _maxNumberOfConnections;

    public int MaxNumberOfConnections {
        get { return _maxNumberOfConnections; }
        set { _maxNumberOfConnections = value; }
    }

    [SerializeField]
    NetworkView _networkView;

    public NetworkView NetworkView {
        get { return _networkView; }
        set { _networkView = value; }
    }

    [SerializeField]
    bool _isServer;

    public bool IsServer {
        get { return _isServer; }
        set { _isServer = value; }
    }

    // Updated when needed
    private MainMenuManagerScript mainMenuManagerScript;

    public MainMenuManagerScript MainMenuManagerScript {
        get { return mainMenuManagerScript; }
        set { mainMenuManagerScript = value; }
    }
    private LobbyManagerScript lobbyManagerScript;

    public LobbyManagerScript LobbyManagerScript {
        get { return lobbyManagerScript; }
        set { lobbyManagerScript = value; }
    }
    private GameManagerScript gameManagerScript;

    public GameManagerScript GameManagerScript {
        get { return gameManagerScript; }
        set { gameManagerScript = value; }
    }

    private bool isServerRunning = false;

    float intervalLookingForServers = 1f;
    float currentIntervalLookingForServers;

    //Lists of servers
    HostData[] hostDatas;

    //Current (or last if deconnected) server visited
    //HostData currentHostData;

    void Awake() {
        if(currentNetworkManagerScript == null) {
            DontDestroyOnLoad(gameObject);
            currentNetworkManagerScript = this;
        } else if(currentNetworkManagerScript != null) {
            Destroy(gameObject);
        }
    }

    void Start() {
        currentIntervalLookingForServers = intervalLookingForServers;
        if(!NetworkManagerScript.currentNetworkManagerScript._isServer) {
            LookForServers();
        }
    }

    void Update() {
        if(!_isServer && SceneStateManager.currentStateManager.getCurrentSceneState() == SceneStateManager.sceneState.MainMenu) {
            currentIntervalLookingForServers -= Time.deltaTime;
            if(currentIntervalLookingForServers < 0) {
                currentIntervalLookingForServers = intervalLookingForServers;
                LookForServers();
            }
        }
    }

    public void LookForServers() {
        MasterServer.RequestHostList("fr.esgi.VvVvV");
        hostDatas = MasterServer.PollHostList();
        if(hostDatas.Length != 0) {
            int i = 0;
            while(i < hostDatas.Length) {
                Debug.Log("Game name : " + hostDatas[i].gameName);
                i++;
            }
        } else {
            Debug.Log("No server available");
        }
    }

    //Server (Runs the server)
    public void runServer() {
        if(!isServerRunning) {
            Debug.Log("Trying to initialize the server ...");
            Network.InitializeSecurity();
            Network.InitializeServer(_maxNumberOfConnections, _serverPort, !Network.HavePublicAddress());
            MasterServer.RegisterHost("fr.esgi.VvVvV", "gameName", "This is a comment");
        } else {
            Debug.Log("Closing the server ...");
            Network.Disconnect();
        }
    }

    //Server (server finished initializing, launch lobbyScene)
    void OnServerInitialized() {
        Debug.Log("Server finished Initializing !");
        isServerRunning = true;
        SceneStateManager.currentStateManager.loadLevel(SceneStateManager.sceneState.Lobby);
    }

    //Server (a player connected)
    void OnPlayerConnected(NetworkPlayer player) {
        Debug.Log("Player [" + player.ipAddress + ":" + player.port + "] connected");
        switch(SceneStateManager.currentStateManager.getCurrentSceneState()) {
            case SceneStateManager.sceneState.Lobby:
                lobbyManagerScript.serverPlayerConnected(player);
                break;
            case SceneStateManager.sceneState.Game:
                // If it's a reconnection, add him to the game, shouldn't happen otherwise
                gameManagerScript.serverPlayerConnected(player);
                break;
            default:
                Debug.Log("OnPlayerConnected with wrong currentSceneState : " + SceneStateManager.currentStateManager.getCurrentSceneState());
                break;
        }
    }

    //Server (a player disconnected)
    void OnPlayerDisconnected(NetworkPlayer player) {
        Debug.Log("Player [" + player.ipAddress + ":" + player.port + "] disconnected");
        switch(SceneStateManager.currentStateManager.getCurrentSceneState()) {
            case SceneStateManager.sceneState.Lobby:
                lobbyManagerScript.serverPlayerDisconnected(player);
                break;
            case SceneStateManager.sceneState.Game:
                gameManagerScript.serverPlayerDisconnected(player);
                break;
            default:
                Debug.Log("OnPlayerDisconnected with wrong currentSceneState : " + SceneStateManager.currentStateManager.getCurrentSceneState());
                break;
        }
    }

    //Client (tries to connect to the server)
    public void TryToConnectToServer() {
        Debug.Log("Trying to connect to the server ...");
        if(hostDatas.Length > 0) {
            Network.Connect(hostDatas[0]);
        }
    }

    //Client
    void OnFailedToConnect(NetworkConnectionError error) {
        // TODO: display error message and retry
    }

    //Client
    void OnConnectedToServer() {
        //currentHostData = hostDatas[0];
    }

    //Server (gets called when the server finishes closing => normal) 
    //Client (gets called when disconnected from server => abnormal)
    void OnDisconnectedFromServer(NetworkDisconnection networkDisconnection) {
        if(_isServer) {
            Debug.Log("Server closed");
            isServerRunning = false;
        } else {
            Debug.Log("Disconnected from server");
            switch(SceneStateManager.currentStateManager.getCurrentSceneState()) {
                case SceneStateManager.sceneState.Lobby:
                    // TODO: Go back to Mainenu
                    break;
                case SceneStateManager.sceneState.Game:
                    // TODO: Try to reconnect/go back to MainMenu after a while
                    break;
                default:
                    Debug.Log("OnDisconnectedFromServer with wrong currentSceneState : " + SceneStateManager.currentStateManager.getCurrentSceneState());
                    break;
            }
        }
    }

    [RPC]
    public void LoadLobby() {
        Debug.Log("RPC received, joining lobby");
        SceneStateManager.currentStateManager.loadLevel(SceneStateManager.sceneState.Lobby);
    }

    //Client-only method
    [RPC]
    public void ClientLaunchGameScene() {
        SceneStateManager.currentStateManager.loadLevel(SceneStateManager.sceneState.Game);
    }
}
