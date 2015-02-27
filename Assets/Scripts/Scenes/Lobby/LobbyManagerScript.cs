using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LobbyManagerScript : MonoBehaviour {

    [SerializeField]
    Text[] _playersText;

    Dictionary<NetworkPlayer, Text> playersTextFromNetworkPlayerAddress = new Dictionary<NetworkPlayer, Text>();
    Dictionary<Text, bool> playerTextIsFilled = new Dictionary<Text, bool>();

    [SerializeField]
    Button _exitLobbyButton;

    [SerializeField]
    Button _startGame;

    [SerializeField]
    NetworkView _networkView;

    [SerializeField]
    Text _textCountdown;

    bool isRunningCoroutineCountdownBeforeLaunchingGame = false;

    void Start() {
        NetworkManagerScript.currentNetworkManagerScript.LobbyManagerScript = this;

        foreach(Text text in _playersText) {
            playerTextIsFilled.Add(text, false);
        }

        _startGame.onClick.AddListener(() => { SceneStateManager.currentStateManager.loadLevel(SceneStateManager.sceneState.Game); });

        _exitLobbyButton.onClick.AddListener(() => { exitLobby(); });

        if(NetworkManagerScript.currentNetworkManagerScript.IsServer) {
            Debug.Log("Server created, waiting for players ...");
        } else {
            Debug.Log("Lobby joined, waiting for other players ...");
        }
    }

    public void exitLobby() {
        if(NetworkManagerScript.currentNetworkManagerScript.IsServer) {
            Debug.Log("Closing server and quitting ...");
            Network.Disconnect();
            Application.Quit();
        } else {
            Debug.Log("Quitting lobby and going back to MainMenu ...");
            Network.Disconnect();
            SceneStateManager.currentStateManager.loadLevel(SceneStateManager.sceneState.MainMenu);
        }
    }

    // Server-only method
    public void serverPlayerConnected(NetworkPlayer player) {
        PersistentNetworkPlayersScript.currentPersistentNetworkPlayersScript.Players.Add(player);
        Debug.Log(PersistentNetworkPlayersScript.currentPersistentNetworkPlayersScript.Players.Count);

        NetworkManagerScript.currentNetworkManagerScript.NetworkView.RPC("LoadLobby", player);

        foreach(Text text in _playersText) {
            if(!playerTextIsFilled[text]) {
                playerTextIsFilled[text] = true;
                playersTextFromNetworkPlayerAddress.Add(player, text);
                text.text = Utils.NetworkPlayerToFormattedAddress(player);
                break;
            }
        }

        foreach(NetworkPlayer networkPlayer in PersistentNetworkPlayersScript.currentPersistentNetworkPlayersScript.Players) {
            if(networkPlayer != player) {
                serverRefreshPlayersTextForSpecifiedPlayer(networkPlayer);
            }
        }

        StartCoroutine(InitPlayersText(player));

        if(PersistentNetworkPlayersScript.currentPersistentNetworkPlayersScript.Players.Count == NetworkManagerScript.currentNetworkManagerScript.MaxNumberOfConnections) {
            StartCoroutine("CountdownBeforeLaunchingGame");
        }
    }

    public IEnumerator InitPlayersText(NetworkPlayer player) {
        yield return new WaitForSeconds(1);
        serverRefreshPlayersTextForSpecifiedPlayer(player);
    }

    // Server-only method
    public void serverPlayerDisconnected(NetworkPlayer player) {
        if(isRunningCoroutineCountdownBeforeLaunchingGame) {
            StopCoroutine("CountdownBeforeLaunchingGame");
            _networkView.RPC("LaunchingPostponed", RPCMode.Others);
        }

        PersistentNetworkPlayersScript.currentPersistentNetworkPlayersScript.Players.Remove(player);
        Debug.Log(PersistentNetworkPlayersScript.currentPersistentNetworkPlayersScript.Players.Count);

        playersTextFromNetworkPlayerAddress[player].text = "Waiting for player ...";
        playerTextIsFilled[playersTextFromNetworkPlayerAddress[player]] = false;

        foreach(NetworkPlayer networkPlayer in PersistentNetworkPlayersScript.currentPersistentNetworkPlayersScript.Players) {
            serverRefreshPlayersTextForSpecifiedPlayer(networkPlayer);
        }
    }

    // Server-only method
    public void serverRefreshPlayersTextForSpecifiedPlayer(NetworkPlayer player) {
        for(int i = 0; i < _playersText.Length; i++) {
            _networkView.RPC("UpdateLobbyPlayers", player, i, _playersText[i].text);
        }
    }

    //Client-only method
    [RPC]
    public void UpdateLobbyPlayers(int index, string playerNameAtIndex) {
        _playersText[index].text = playerNameAtIndex;
    }

    public IEnumerator CountdownBeforeLaunchingGame() {
        isRunningCoroutineCountdownBeforeLaunchingGame = true;
        Debug.Log("Lobby full, launching the game in 3 ...");
        _networkView.RPC("UpdateTimeRemainingBeforeLaunchingGame", RPCMode.Others, 3);
        yield return new WaitForSeconds(1);
        _networkView.RPC("UpdateTimeRemainingBeforeLaunchingGame", RPCMode.Others, 2);
        Debug.Log("2 ...");
        yield return new WaitForSeconds(1);
        _networkView.RPC("UpdateTimeRemainingBeforeLaunchingGame", RPCMode.Others, 1);
        Debug.Log("1 ...");
        yield return new WaitForSeconds(1);
        Debug.Log("Starting !");
        _networkView.RPC("clientLaunchGameScene", RPCMode.Others);

        SceneStateManager.currentStateManager.loadLevel(SceneStateManager.sceneState.Game);
        isRunningCoroutineCountdownBeforeLaunchingGame = false;
    }

    [RPC]
    public void UpdateTimeRemainingBeforeLaunchingGame(int secondsRemaining) {
        StopCoroutine("ShowMessage");
        StartCoroutine("ShowMessage", secondsRemaining);
    }

    IEnumerator ShowMessage(int secondsRemaining) {
        _textCountdown.enabled = true;
        _textCountdown.text = "Début dans " + secondsRemaining.ToString("F0") + " ...";
        yield return new WaitForSeconds(1);
        _textCountdown.enabled = false;
        _textCountdown.text = "";
    }

    [RPC]
    public void LaunchingPostponed() {
        StopCoroutine("ShowMessage");
        StartCoroutine(ShowMessage2());
    }

    IEnumerator ShowMessage2() {
        _textCountdown.enabled = true;
        _textCountdown.text = "Joueur déco, attendre";
        yield return new WaitForSeconds(3);
        _textCountdown.enabled = false;
        _textCountdown.text = "";
    }


    //Client-only method
    [RPC]
    public void clientLaunchGameScene() {
        SceneStateManager.currentStateManager.loadLevel(SceneStateManager.sceneState.Game);
    }
}
