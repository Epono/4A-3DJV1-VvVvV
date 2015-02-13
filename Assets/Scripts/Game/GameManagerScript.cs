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

    //[SerializeField]
    //Material _otherPlayerMaterial;

    [SerializeField]
    public NetworkView _networkView;

    [SerializeField]
    Text _textTurnTimeRemaining;

    [SerializeField]
    Text _textGameTimeRemaining;

    [SerializeField]
    float _gameDuration = 180.0f;

    [SerializeField]
    float _turnDuration = 10.0f;

    float currentTurnTimeRemaining = 10.0f;

    [SerializeField]
    float _simulationDuration = 5.0f;

    float currentSimulationTimeRemaining = 5.0f;

    bool isPlaying = false;

    bool[] playerWantsToFinishTurn = new bool[3];
    bool finishTurn;

    void Start() {
        NetworkManagerScript.currentNetworkManagerScript._gameManagerScript = this;

        PersistentPlayersScript.currentPersistentPlayersScript.displayNetworkPlayers();

        //if(NetworkManagerScript.currentNetworkManagerScript._isServer) {
        //    _networkView.RPC("TellPlayerWhoHeIs", RPCMode.Others);
        //}
    }

    void Update() {
        if(NetworkManagerScript.currentNetworkManagerScript._isServer) {
            _gameDuration -= Time.deltaTime;
            _textGameTimeRemaining.text = "Jeu : " + _gameDuration.ToString("F0") + "s";

            if(_gameDuration < 0 || _coins.Length == 0) {
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
            // Reprendre la planif
        } else {
            // Executer actions
            for(int i = 0; i < _playersScript.Length; i++) {
                if(_playersScript[i].HasMoreActions() && _playersScript[i].GetCurrentAction() == null) {
                    Debug.Log("On execute une action !");
                    _playersScript[i].ExecuteNextAction();
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

            // Lancer la simu
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

    //[RPC]
    //void TellPlayerWhoHeIs() {
    //    foreach(GameObject go in _playersGameObject) {
    //        if(!go.name.Equals(int.Parse(Network.player.ToString()))) {
    //            go.GetComponent<Renderer>().material = _otherPlayerMaterial;
    //        }
    //    }
    //}
}

// isKinematic pour les players ?
