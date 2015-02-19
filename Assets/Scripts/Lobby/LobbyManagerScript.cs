using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LobbyManagerScript : MonoBehaviour {

    [SerializeField]
    private Text[] _playersText;

    [SerializeField]
    Button _exitLobbyButton;

    [SerializeField]
    Button _startGame;

    [SerializeField]
    NetworkView _networkView;

    void Start() {
        NetworkManagerScript.currentNetworkManagerScript._lobbyManagerScript = this;

        _startGame.onClick.AddListener(() => { SceneStateManager.currentStateManager.loadLevel(SceneStateManager.sceneState.Game); });

        _exitLobbyButton.onClick.AddListener(() => { exitLobby(); });

        if(NetworkManagerScript.currentNetworkManagerScript._isServer) {
            Debug.Log("Server created, waiting for players ...");
        } else {
            Debug.Log("Lobby joined, waiting for other players ...");
        }
    }

    public void exitLobby() {
        if(NetworkManagerScript.currentNetworkManagerScript._isServer) {
            // Close server and quit
            Debug.Log("Closing server and quitting ...");

            //Temp
            SceneStateManager.currentStateManager.loadLevel(SceneStateManager.sceneState.MainMenu);

            //Normalement
            //Network.Disconnect();
            //Application.Quit();
        } else {
            //Disconnect from server and go back to MainMenuScene
            Debug.Log("Quitting lobby and going back to MainMenu ...");
            Network.Disconnect();

            SceneStateManager.currentStateManager.loadLevel(SceneStateManager.sceneState.MainMenu);
        }
    }


    // Server-only method
    public void serverPlayerJoined(NetworkPlayer player) {
        PersistentPlayersScript.currentPersistentPlayersScript.playerConnected(player);

        NetworkManagerScript.currentNetworkManagerScript._networkView.RPC("LoadLobby", player);

        List<NetworkPlayer> players = PersistentPlayersScript.currentPersistentPlayersScript.getPlayers();
        for(int i = 0; i < players.Count; i++) {
            _playersText[i].text = players[i].guid;

            if(players[i] != player) {
                serverRefreshPlayersText(players[i]);
            }
        }

        StartCoroutine(InitPlayersText(player));

        if(PersistentPlayersScript.currentPersistentPlayersScript.getPlayers().Count == NetworkManagerScript.currentNetworkManagerScript._maxNumberOfConnections) {
            StartCoroutine(wait3s());
        }
    }

    public IEnumerator InitPlayersText(NetworkPlayer player) {
        yield return new WaitForSeconds(1);
        serverRefreshPlayersText(player);
    }



    public IEnumerator wait3s() {
        Debug.Log("Lobby full, launching the game in 3 ...");
        //TODO: RPC pour dire le temps qu'il reste
        yield return new WaitForSeconds(1);
        //TODO: RPC pour dire le temps qu'il reste
        Debug.Log("2 ...");
        yield return new WaitForSeconds(1);
        //TODO: RPC pour dire le temps qu'il reste
        Debug.Log("1 ...");
        yield return new WaitForSeconds(1);
        Debug.Log("Starting !");
        _networkView.RPC("clientLaunchGameScene", RPCMode.OthersBuffered);

        SceneStateManager.currentStateManager.loadLevel(SceneStateManager.sceneState.Game);
    }

    // Server-only method
    public void serverPlayerDisconnected(NetworkPlayer player) {
        PersistentPlayersScript.currentPersistentPlayersScript.playerDisconnected(player);
        serverRefreshPlayersText(player);
    }

    // Server-only method
    public void serverRefreshPlayersText(NetworkPlayer player) {
        List<NetworkPlayer> players = PersistentPlayersScript.currentPersistentPlayersScript.getPlayers();
        for(int i = 0; i < PersistentPlayersScript.currentPersistentPlayersScript.getPlayers().Count; i++) {
            _networkView.RPC("UpdateLobbyPlayers", player, i, players[i].guid);
        }
    }

    //Server-only method
    [RPC]
    public void UpdateLobbyPlayers(int index, string playerNameAtIndex) {
        _playersText[index].text = playerNameAtIndex;
    }

    //Client-only method
    [RPC]
    public void clientLaunchGameScene() {
        SceneStateManager.currentStateManager.loadLevel(SceneStateManager.sceneState.Game);
    }

    //Client-only method
    [RPC]
    public void clientLaunchGameOverScene() {
        SceneStateManager.currentStateManager.loadLevel(SceneStateManager.sceneState.GameOver);
    }
}
