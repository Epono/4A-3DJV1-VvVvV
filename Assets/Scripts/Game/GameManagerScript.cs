using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


public class GameManagerScript : MonoBehaviour {

    //Singletonisation
    public static GameManagerScript currentGameManagerScript;

    SmartActionManagerScript _smartActionManager;

    [SerializeField]
    private PlayerScript[] _playersScript;

    [SerializeField]
    private GameObject[] _playersGameObject;

    /*
    [SerializeField]
    public Object _playerPrefab;
    */

    [SerializeField]
    public Material _otherPlayerMaterial;


    [SerializeField]
    public NetworkView _networkView;


    //[SerializeField]
    //GUIText Timer;

    //TODO : Utilisation d'une variable pour le choix de la durée d'une partie
    float timeleft = 60.0f * 5;
    //[SerializeField]
    //TextMesh timer;

    //List<CharacterAction> _maList = new List<CharacterAction>();


    void Awake() {
        if(currentGameManagerScript == null) {
            DontDestroyOnLoad(gameObject);
            currentGameManagerScript = this;
        } else if(currentGameManagerScript != null) {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start() {
        PersistentPlayersScript.currentPersistentPlayersScript.displayNetworkPlayers();



    }

    // Update is called once per frame
    void Update() {
        // Debug.Log("Fin de la partie dans :" + timeleft);
        timeleft -= Time.deltaTime;
        if(timeleft < 0) {
            Debug.Log("GameOver");
            Application.Quit();
        }
    }

    void FixedUpdate() {
        //guiText.text = Time.timeSinceLevelLoad.ToString();
        //  Timer.text = Time.timeSinceLevelLoad.ToString();
        //  timer.text = Time.timeSinceLevelLoad.ToString();
    }

    /*
    public void WantToMove(int player, Vector3 pos) {
        if(NetworkManagerScript.currentNetworkManagerScript._isServer) {
            //Debug
            PersistentPlayersScript.currentPersistentPlayersScript.displayNetworkPlayers();
            //_networkView.RPC("TellPlayerWhoHeIs", RPCMode.Others);


        } else {
            foreach(GameObject go in _playersGameObject) {
                if(!go.name.Equals(int.Parse(Network.player.ToString()))) {
                    go.GetComponent<Renderer>().material = _otherPlayerMaterial;
                }
            }
        }
    }
    */

    [RPC]
    void TellPlayerWhoHeIs() {
        foreach(GameObject go in _playersGameObject) {
            if(!go.name.Equals(int.Parse(Network.player.ToString()))) {
                go.GetComponent<Renderer>().material = _otherPlayerMaterial;
            }
        }
    }

    // Update is called once per frame



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
        for(int i = 0; i < maList.Count; i++) {

            Debug.Log("Taille de la liste :" + maList.Count);
            // Debug.Log("type de ma putain de liste :" + maList[i].GetType());
            // Debug.Log("??? de ma putain de liste :" + maList[i]);
            // Debug.Log("??? de ma putain de liste :" + maList);



            //_playersScript[player].ExecuteActionT(dico["MoveToLocation"]);
            _playersScript[player].ExecuteActionT(maList[i]);

            //_playersScript[player].ExecuteAction(actionNameList[i], pos);

        }
    }
}
