using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


public class GameManagerScript : MonoBehaviour
{

    //Singletonisation
    public static GameManagerScript currentGameManagerScript;
    
    [SerializeField]
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

    //Liste qui contiendras les actions à executer a chaque tour
    List<string> _listActionName = new List<string>();

    //test avec un dictionnaire
    Dictionary<string, Object> _dicoActionName = new Dictionary<string, Object>();

    void Awake()
    {
        if (currentGameManagerScript == null)
        {
            DontDestroyOnLoad(gameObject);
            currentGameManagerScript = this;
        }
        else if (currentGameManagerScript != null)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        if(NetworkManagerScript.currentNetworkManagerScript._isServer) {
            //Debug
            PersistentPlayersScript.currentPersistentPlayersScript.displayNetworkPlayers();
            //_networkView.RPC("TellPlayerWhoHeIs", RPCMode.Others);

            /*
            List<NetworkPlayer> l = PersistentPlayersScript.currentPersistentPlayersScript.getPlayers();
            List<GameObject> ll = new List<GameObject>();

            for(int i = 0; i < l.Count; i++) {
                GameObject t = (GameObject) Network.Instantiate(_playerPrefab, new Vector3(10 * (i-1), 2, -70), new Quaternion(), 0);
                t.GetComponent<ClassicPlayerScript>().playerId = 1 + i;
               // t.name = "Player" + i;
                ll.Add(t);
            }

            //for(int i = 0; i < l.Count; i++) {
            //}
             */

            /*
            int i = -10;
            foreach(NetworkPlayer n in PersistentPlayersScript.currentPersistentPlayersScript.getPlayers()) {
                //Select a random SpawnPoint
                //Compute the associated rotation (useless ?) 
                //Network.Instantiate(prefab, RANDOM_SPAWN_POINT, ROTATION_EN_FONCTION_DU_SPAWN, group?);
                GameObject t = (GameObject) Network.Instantiate(_playerPrefab, new Vector3(i, 2, -70), new Quaternion(), 0);
                t.GetComponent<Renderer>().material = _otherPlayerMaterial;
                i += 10;
            }
             */
        } else {
            foreach(GameObject go in _playersGameObject) {
                if(!go.name.Equals(int.Parse(Network.player.ToString()))) {
                    go.GetComponent<Renderer>().material = _otherPlayerMaterial;
                }
            }
        }
    }

    [RPC]
    void TellPlayerWhoHeIs() {
        /*
        BinaryFormatter bf = new BinaryFormatter(); //Create a formatter
        MemoryStream ins = new MemoryStream(System.Convert.FromBase64String(data)); //Create an input stream from the string
        //Read back the data
        List<GameObject> lll = (List<GameObject>)bf.Deserialize(ins);

        List<NetworkPlayer> l = PersistentPlayersScript.currentPersistentPlayersScript.getPlayers();
        for(int i = 0; i < l.Count; i++) {
            if(i+1 != playerId) {
                lll[i].GetComponent<Renderer>().material = _otherPlayerMaterial;
            }
        }

        Network.player.i
       */
        //Find("Player" + int.Parse(Network.player.ToString()));
        /*
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach(GameObject go in allObjects) {
            if(go.name.StartsWith("Player")) {
                ClassicPlayerScript cps = go.GetComponent<ClassicPlayerScript>();
                Debug.Log((int.Parse(Network.player.ToString())) + " - " + cps.playerId);
                if(cps.playerId != int.Parse(Network.player.ToString())) {
                    go.GetComponent<Renderer>().material = _otherPlayerMaterial;
                }
            }
        }
        */

        foreach(GameObject go in _playersGameObject) {
            if(!go.name.Equals(int.Parse(Network.player.ToString()))) {
                go.GetComponent<Renderer>().material = _otherPlayerMaterial;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if(allCoinsCollected || timesUp) {
            // End the game
        //}
    }

    //J'EN SUIS LA
    public void ExecuteTurnAction(Vector3 pos)
    {
        Debug.Log("On rentre dans ExecuteTurnAction()");
       // for (int i = 0; i < 3; i++ )
        //{
        Debug.Log(_listActionName[0]);
        ExecuteActionOfPlayer(0, _listActionName, pos);
        //}
        
           
    }

    //Liste d'action du tour d'un joueur
    public void AddActionInActionList(string actionName)
    {
        _listActionName.Add(actionName);
        Debug.Log("L'action a bien été ajouté a la liste");
    }


    public void ExecuteActionOfPlayer(int player, List<string> actionNameList, Vector3 pos)
    {
        Debug.Log("On rentre dans ExecuteActionOfPlayer()");
        for (int i = 0; i < actionNameList.Count; i++)
        {
            //_playersScript[player].ExecuteAction(actionNameList[i], pos);
        }
    }
}
