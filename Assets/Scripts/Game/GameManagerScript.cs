using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class GameManagerScript : MonoBehaviour {

    public static GameManagerScript currentGameManagerScript;

    [SerializeField]
    List<PlayerScript> _playerScript;

    Dictionary<NetworkPlayer, PlayerScript> playerScriptFromNetworkPlayer = new Dictionary<NetworkPlayer, PlayerScript>();

    [SerializeField]
    GameObject[] _playersGameObject;

    [SerializeField]
    List<GameObject> _coins;

    public List<GameObject> Coins {
        get { return _coins; }
        set { _coins = value; }
    }

    [SerializeField]
    Material _playerMaterial;

    [SerializeField]
    NetworkView _networkView;

    public NetworkView NetworkView {
        get { return _networkView; }
        set { _networkView = value; }
    }

    [SerializeField]
    Text _textTurnTimeRemaining;

    [SerializeField]
    Text _textGameTimeRemaining;

    [SerializeField]
    Text _textScore;

    [SerializeField]
    float _gameDuration = 180.0f;
    float currentGameTimeRemaining;

    [SerializeField]
    float _turnDuration = 10.0f;
    float currentTurnTimeRemaining;

    [SerializeField]
    float _simulationDuration = 5.0f;
    float currentSimulationTimeRemaining;

    bool isPlaying = false;

    bool[] playerWantsToFinishTurn = new bool[NetworkManagerScript.currentNetworkManagerScript.MaxNumberOfConnections];
    bool finishTurn;

    [SerializeField]
    float _intervalBetweenRPCs = 0.1f;
    float currentIntervalBetweenRPCs;

    GameObject currentPlayerGameObject;

    public GameObject CurrentPlayerGameObject {
        get { return currentPlayerGameObject; }
        set { currentPlayerGameObject = value; }
    }

    void Start() {
        currentGameManagerScript = this;

        currentGameTimeRemaining = _gameDuration;
        currentSimulationTimeRemaining = _simulationDuration;
        currentTurnTimeRemaining = _turnDuration;
        currentIntervalBetweenRPCs = _intervalBetweenRPCs;

        if(NetworkManagerScript.currentNetworkManagerScript.IsServer) {
            NetworkManagerScript.currentNetworkManagerScript.GameManagerScript = this;

            PersistentNetworkPlayersScript.currentPersistentNetworkPlayersScript.displayNetworkPlayers();

            for(int i = 0; i < NetworkManagerScript.currentNetworkManagerScript.MaxNumberOfConnections; i++) {
                PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript[i].NetworkPlayer = PersistentNetworkPlayersScript.currentPersistentNetworkPlayersScript.Players[i];
                playerScriptFromNetworkPlayer.Add(PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript[i].NetworkPlayer, PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript[i]);
                _networkView.RPC("TellPlayerWhoHeIs", PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript[i].NetworkPlayer, i + 1);
            }
        }
    }

    void Update() {
        if(NetworkManagerScript.currentNetworkManagerScript.IsServer) {
            currentGameTimeRemaining -= Time.deltaTime;
            _textGameTimeRemaining.text = "Jeu : " + currentGameTimeRemaining.ToString("F0") + "s";

            if(currentGameTimeRemaining < 0 || _coins.Count == 0) {
                //TODO: Gérer la victoire/defaite
                //Application.LoadLevel("GameOver");
            } else {
                if(isPlaying) {
                    UpdateDuringSimulation();
                } else {
                    UpdateDuringPlanification();
                }
            }
        }
    }

    void UpdateDuringSimulation() {
        currentSimulationTimeRemaining -= Time.deltaTime;
        _textTurnTimeRemaining.text = "Simu : " + currentSimulationTimeRemaining.ToString("F1") + "s";

        if(currentSimulationTimeRemaining < 0) {
            isPlaying = false;
            currentSimulationTimeRemaining = _simulationDuration;
            for(int i = 0; i < PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript.Count; i++) {
                PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript[i].ClearActionsList();
                playerWantsToFinishTurn[i] = false;
                PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript[i].StopMove();
            }
            // Reprend la planif au prochain update
        } else {
            // Executer actions
            for(int i = 0; i < PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript.Count; i++) {
                if(PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript[i].HasMoreActions() && PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript[i].GetCurrentAction() == null) {
                    PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript[i].ExecuteNextAction();
                }
            }
        }

        currentIntervalBetweenRPCs -= Time.deltaTime;
        if(currentIntervalBetweenRPCs < 0) {
            currentIntervalBetweenRPCs = _intervalBetweenRPCs;
            foreach(PlayerScript playerScript in PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript) {
                if(playerScript.IsConnected) {
                    _networkView.RPC("UpdatePlayersDuringSimulation", playerScript.NetworkPlayer, currentGameTimeRemaining, currentSimulationTimeRemaining, playerScript.GetScore());
                }
            }
        }
    }

    void UpdateDuringPlanification() {
        currentTurnTimeRemaining -= Time.deltaTime;
        _textTurnTimeRemaining.text = "Tour : " + currentTurnTimeRemaining.ToString("F1") + " s";

        if(currentTurnTimeRemaining < 0 || finishTurn) {
            isPlaying = true;
            finishTurn = false;
            currentTurnTimeRemaining = _turnDuration;
            // Lance la simu au prochain update
        }

        currentIntervalBetweenRPCs -= Time.deltaTime;
        if(currentIntervalBetweenRPCs < 0) {
            currentIntervalBetweenRPCs = _intervalBetweenRPCs;
            foreach(PlayerScript playerScript in PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript) {
                if(playerScript.IsConnected) {
                    _networkView.RPC("UpdatePlayersDuringPlanification", playerScript.NetworkPlayer, currentGameTimeRemaining, currentTurnTimeRemaining, playerScript.GetScore());
                }
            }
        }
    }

    public void serverPlayerConnected(NetworkPlayer player) {
        foreach(PlayerScript playerScript in PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript) {
            if(player.ipAddress.Equals(playerScript.NetworkPlayer.ipAddress) && player.port.Equals(playerScript.NetworkPlayer.port)) {
                playerScript.NetworkPlayer = player;
                playerScript.IsConnected = true;
                NetworkManagerScript.currentNetworkManagerScript.NetworkView.RPC("ClientLaunchGameScene", player);
                break;
            }
        }
    }

    public void serverPlayerDisconnected(NetworkPlayer player) {
        foreach(PlayerScript playerScript in PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript) {
            if(player.ipAddress.Equals(playerScript.NetworkPlayer.ipAddress) && player.port.Equals(playerScript.NetworkPlayer.port)) {
                playerScript.IsConnected = false;
            }
        }
    }

    //Finish turn
    [RPC]
    public void WantsToFinishTurn(NetworkPlayer player) {
        if(!isPlaying) {
            if(NetworkManagerScript.currentNetworkManagerScript.IsServer) {
                _networkView.RPC("WantsToFinishTurn", RPCMode.Others, player);
            }
            var playerId = int.Parse(player.ToString());

            playerWantsToFinishTurn[playerId - 1] = true;

            finishTurn = true;
            for(int i = 0; i < PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript.Count; i++) {
                if(!playerWantsToFinishTurn[i]) {
                    finishTurn = false;
                    break;
                }
            }
        }
    }

    [RPC]
    public void WantsToAddWayPoint(NetworkPlayer player, Vector3 point) {
        if(!isPlaying) {
            if(NetworkManagerScript.currentNetworkManagerScript.IsServer) {
                _networkView.RPC("WantsToAddWayPoint", RPCMode.Others, player, point);
            }
            //PlayerScript currentPlayerScript = PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript[int.Parse(player.ToString()) - 1];
            PlayerScript currentPlayerScript = playerScriptFromNetworkPlayer[player];

            currentPlayerScript.AddActionInList(new CharacterActionMove(currentPlayerScript, point));
        }
    }

    [RPC]
    public void WantsToCollectCoins(NetworkPlayer player) {
        if(!isPlaying) {
            if(NetworkManagerScript.currentNetworkManagerScript.IsServer) {
                _networkView.RPC("WantsToCollectCoins", RPCMode.Others, player);
            }
            //PlayerScript currentPlayerScript = PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript[int.Parse(player.ToString()) - 1];
            PlayerScript currentPlayerScript = playerScriptFromNetworkPlayer[player];

            currentPlayerScript.AddActionInList(new CharacterActionCollectCoins(currentPlayerScript));
        }
    }

    [RPC]
    public void UpdatePlayersDuringPlanification(float newCurrentGameTimeRemaining, float newCurrentTurnTimeRemaining, int newScore) {
        _textGameTimeRemaining.text = "Jeu : " + newCurrentGameTimeRemaining.ToString("F0") + "s";
        _textTurnTimeRemaining.text = "Tour : " + newCurrentTurnTimeRemaining.ToString("F1") + " s";
        _textScore.text = "Score : " + newScore;
    }

    [RPC]
    public void UpdatePlayersDuringSimulation(float newCurrentGameTimeRemaining, float newCurrentSimulationTimeRemaining, int newScore) {
        _textGameTimeRemaining.text = "Jeu : " + newCurrentGameTimeRemaining.ToString("F0") + "s";
        _textTurnTimeRemaining.text = "Simu : " + newCurrentSimulationTimeRemaining.ToString("F1") + "s";
        _textScore.text = "Score : " + newScore;
    }

    [RPC]
    void TellPlayerWhoHeIs(int index) {
        foreach(GameObject go in _playersGameObject) {
            if(go.name.Equals("Player" + index)) {
                go.GetComponent<Renderer>().material = _playerMaterial;
                currentPlayerGameObject = go;
            }
        }
    }
}