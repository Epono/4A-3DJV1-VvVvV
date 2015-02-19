using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


public class GameManagerScript : MonoBehaviour {

    [SerializeField]
    PlayerScript[] _playersScript;

    [SerializeField]
    GameObject[] _playersGameObject;

    [SerializeField]
    GameObject[] _coins;

    [SerializeField]
    Material _playerMaterial;

    [SerializeField]
    public NetworkView _networkView;

    [SerializeField]
    Text _textTurnTimeRemaining;

    [SerializeField]
    Text _textGameTimeRemaining;

    [SerializeField]
    Text _textScore;

    [SerializeField]
    float _gameDuration = 180.0f;
    float currentGameTimeRemaining = 180.0f;

    [SerializeField]
    float _turnDuration = 10.0f;
    float currentTurnTimeRemaining = 10.0f;

    [SerializeField]
    float _simulationDuration = 5.0f;
    float currentSimulationTimeRemaining = 5.0f;

    bool isPlaying = false;

    bool[] playerWantsToFinishTurn = new bool[3];
    bool finishTurn;

    float intervalBetweenRPCs = 0.1f;
    float currentIntervalBetweenRPCs = 0.1f;

    void Start() {
        if(NetworkManagerScript.currentNetworkManagerScript._isServer) {
            NetworkManagerScript.currentNetworkManagerScript._gameManagerScript = this;

            PersistentPlayersScript.currentPersistentPlayersScript.displayNetworkPlayers();

            for(int i = 0; i < 3; i++) {
                _playersScript[i]._networkPlayer = PersistentPlayersScript.currentPersistentPlayersScript.getPlayers()[i];
                _networkView.RPC("TellPlayerWhoHeIs", _playersScript[i]._networkPlayer, i + 1);
            }
        }
    }

    void Update() {
        if(NetworkManagerScript.currentNetworkManagerScript._isServer) {
            currentGameTimeRemaining -= Time.deltaTime;
            _textGameTimeRemaining.text = "Jeu : " + currentGameTimeRemaining.ToString("F0") + "s";

            if(currentGameTimeRemaining < 0 || _coins.Length == 0) {
                Application.LoadLevel("GameOver");
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
            for(int i = 0; i < _playersScript.Length; i++) {
                _playersScript[i].ClearActionsList();
                playerWantsToFinishTurn[i] = false;
                _playersScript[i].StopMove();
            }
            // Reprend la planif au prochain update
        } else {
            // Executer actions
            for(int i = 0; i < _playersScript.Length; i++) {
                if(_playersScript[i].HasMoreActions() && _playersScript[i].GetCurrentAction() == null) {
                    Debug.Log("On execute une action !");
                    _playersScript[i].ExecuteNextAction();
                }
            }
        }

        currentIntervalBetweenRPCs -= Time.deltaTime;
        if(currentIntervalBetweenRPCs < 0) {
            currentIntervalBetweenRPCs = intervalBetweenRPCs;
            foreach(PlayerScript playerScript in _playersScript) {
                if(PersistentPlayersScript.currentPersistentPlayersScript.getPlayers().Contains(playerScript._networkPlayer)) {
                    _networkView.RPC("UpdatePlayersDuringSimulation", playerScript._networkPlayer, currentGameTimeRemaining, currentSimulationTimeRemaining, playerScript.GetScore());
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
            currentIntervalBetweenRPCs = intervalBetweenRPCs;
            foreach(PlayerScript playerScript in _playersScript) {
                _networkView.RPC("UpdatePlayersDuringPlanification", playerScript._networkPlayer, currentGameTimeRemaining, currentTurnTimeRemaining, playerScript.GetScore());
            }
        }
    }

    //Finish turn
    [RPC]
    public void WantsToFinishTurn(NetworkPlayer player) {
        if(!isPlaying) {
            if(NetworkManagerScript.currentNetworkManagerScript._isServer) {
                _networkView.RPC("WantsToFinishTurn", RPCMode.Others, player);
            }
            var playerId = int.Parse(player.ToString());

            playerWantsToFinishTurn[playerId - 1] = true;

            finishTurn = true;
            for(int i = 0; i < _playersScript.Length; i++) {
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
            if(NetworkManagerScript.currentNetworkManagerScript._isServer) {
                _networkView.RPC("WantsToAddWayPoint", RPCMode.Others, player, point);
            }
            var playerId = int.Parse(player.ToString());

            Debug.Log(playerId);

            _playersScript[playerId - 1].AddActionInList(new CharacterActionMove(point, _playersScript[playerId - 1].GetAgent()));
        }
    }

    [RPC]
    public void WantsToCollectCoins(NetworkPlayer player) {
        if(!isPlaying) {
            if(NetworkManagerScript.currentNetworkManagerScript._isServer) {
                _networkView.RPC("WantsToCollectCoins", RPCMode.Others, player);
            }
            var playerId = int.Parse(player.ToString());

            _playersScript[playerId - 1].AddActionInList(new CharacterActionCollectCoins());
        }
    }

    [RPC]
    public void UpdatePlayersDuringPlanification(float newCurrentGameTimeRemaining, float newCurrentTurnTimeRemaining, int newScore) {
        _textGameTimeRemaining.text = "Jeu : " + newCurrentGameTimeRemaining.ToString("F0") + "s";
        _textTurnTimeRemaining.text = "Tour : " + newCurrentTurnTimeRemaining.ToString("F1") + " s";
        _textScore.text = "Simu : " + newScore.ToString("F0") + "s";
    }

    [RPC]
    public void UpdatePlayersDuringSimulation(float newCurrentGameTimeRemaining, float newCurrentSimulationTimeRemaining, int newScore) {
        _textGameTimeRemaining.text = "Jeu : " + newCurrentGameTimeRemaining.ToString("F0") + "s";
        _textTurnTimeRemaining.text = "Simu : " + newCurrentSimulationTimeRemaining.ToString("F1") + "s";
        _textScore.text = "Simu : " + newScore.ToString("F0") + "s";
    }

    [RPC]
    void TellPlayerWhoHeIs(int index) {
        foreach(GameObject go in _playersGameObject) {
            if(go.name.Equals("Player" + index)) {
                go.GetComponent<Renderer>().material = _playerMaterial;
            }
        }
    }
}