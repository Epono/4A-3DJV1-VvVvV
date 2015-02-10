using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


public class GameManagerScript : MonoBehaviour {

    [SerializeField]
    SmartActionManagerScript _smartActionManager;

    [SerializeField]
    PlayerScript[] _playersScript;

    [SerializeField]
    GameObject[] _playersGameObject;

    [SerializeField]
    Material _otherPlayerMaterial;

    [SerializeField]
    NetworkView _networkView;

    [SerializeField]
    Text _textTurnTimeRemaining;

    [SerializeField]
    Text _textGameTimeRemaining;


    //[SerializeField]
    //GUIText Timer;

    //TODO : Utilisation d'une variable pour le choix de la durée d'une partie
    float timeleft = 60.0f * 3;
    //[SerializeField]
    //TextMesh timer;

    //List<CharacterAction> _maList = new List<CharacterAction>();

    [SerializeField]
    float _turnDuration = 10.0f;

    float currentTurnTimeRemaining = 10.0f;

    [SerializeField]
    float _simulationDuration = 5.0f;

    float currentSimulationTimeRemaining = 5.0f;

    bool isPlaying = false;

    void Start() {
        NetworkManagerScript.currentNetworkManagerScript._gameManagerScript = this;

        PersistentPlayersScript.currentPersistentPlayersScript.displayNetworkPlayers();

        //if(networkmanagerscript.currentnetworkmanagerscript._isserver) {
        //    _networkview.rpc("tellplayerwhoheis", rpcmode.others);
        //}
    }

    void Update() {
        timeleft -= Time.deltaTime;
        _textGameTimeRemaining.text = "Game : " + timeleft.ToString("F0") + " s";

        if(isPlaying) {
            currentSimulationTimeRemaining -= Time.deltaTime;
            _textTurnTimeRemaining.text = "Simu : " + currentSimulationTimeRemaining.ToString("F1") + " s";
        } else {
            currentTurnTimeRemaining -= Time.deltaTime;
            _textTurnTimeRemaining.text = "Tour : " + currentTurnTimeRemaining.ToString("F1") + " s";
        }


        if(timeleft < 0) {
            Application.LoadLevel("GameOver");
        } else {
            if(isPlaying) {
                if(currentSimulationTimeRemaining < 0) {
                    isPlaying = false;
                    currentSimulationTimeRemaining = _simulationDuration;
                    // Reprendre
                }
            } else {
                if(currentTurnTimeRemaining < 0) {
                    isPlaying = true;
                    currentTurnTimeRemaining = _turnDuration;
                    // Lancer simulation
                    // ExecuteActions();
                }
            }
        }
    }

    [RPC]
    void TellPlayerWhoHeIs() {
        foreach(GameObject go in _playersGameObject) {
            if(!go.name.Equals(int.Parse(Network.player.ToString()))) {
                go.GetComponent<Renderer>().material = _otherPlayerMaterial;
            }
        }
    }

    //Execution des actions d'un joueur
    /*
    public void ExecuteTurnActionT() {
        Debug.Log("On rentre dans ExecuteTurnActionT()");

        ExecuteActionOfPlayerT(0, _maList);
    }
    */


    //Liste d'action du tour d'un joueur
    //public void AddActionInActionList(string actionName)
    //{
    //    _listActionName.Add(actionName);
    //    Debug.Log("L'action a bien été ajouté a la liste");
    //}

    //public void AddActionInDico(CharacterActionMove action)
    //{
    //    //_dicoActionName.Add(action.getActionName(), action);
    //    _maList.Add(action);
    //    Debug.Log("L'action a bien été ajouté au dico");
    //}

    /*
    public void AddActionInList(CharacterAction currentAction) {
        _maList.Add(currentAction);
    }
     */

    //public void ExecuteActionOfPlayer(int player, List<string> actionNameList, Vector3 pos)
    //{
    //    Debug.Log("On rentre dans ExecuteActionOfPlayer()");
    //    for (int i = 0; i < actionNameList.Count; i++)
    //    {
    //        _playersScript[player].ExecuteAction(actionNameList[i], pos);
    //    }
    //}

    [RPC]
    public void WantsToExecute(NetworkPlayer player) {
        if(Network.isServer) {
            _networkView.RPC("WantsToExecute", RPCMode.Others, player);
        }
        var playerId = int.Parse(player.ToString());

        ExecuteActionOfPlayerT(playerId - 1, _playersScript[playerId - 1].actionList);
    }

    [RPC]
    public void WantsToAddWayPoint(NetworkPlayer player, Vector3 point) {
        if(Network.isServer) {
            _networkView.RPC("WantsToAddWayPoint", RPCMode.Server, player, point);
        }
        var playerId = int.Parse(player.ToString());

        //ExecuteActionOfPlayerT(playerId - 1, _playersScript[playerId - 1].actionList);
        _playersScript[playerId - 1].AddActionInList(new CharacterActionMove(point));
    }

    public void ExecuteActionOfPlayerT(int player, List<CharacterAction> maList) {
        Debug.Log("On rentre dans ExecuteActionOfPlayerT()");
        //  maList.ConvertAll(CharacterActionMove);
        // for(int i = 0; i < maList.Count; i++) {
        if(maList.Count != 0) {


            Debug.Log("Taille de la liste :" + maList.Count);




            //_playersScript[player].ExecuteActionT(dico["MoveToLocation"]);
            _playersScript[player].ExecuteActionT(maList[0]);

            //_playersScript[player].ExecuteAction(actionNameList[i], pos);

            // }
        }
    }

    [RPC]
    public void HasToStop(NetworkPlayer player) {
        if(Network.isServer) {
            _networkView.RPC("HasToStop", RPCMode.Others, player);
        }
        var playerId = int.Parse(player.ToString());

        ExecuteActionOfPlayerT(playerId - 1, _playersScript[playerId - 1].actionList);
        _playersScript[playerId - 1]._agent.Stop();
    }
}
