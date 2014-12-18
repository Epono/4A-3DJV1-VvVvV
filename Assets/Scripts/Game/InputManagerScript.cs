using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputManagerScript : MonoBehaviour {
    //Singletonisation
    public static InputManagerScript currentInputManagerScript;

    [SerializeField]
    private GameManagerScript _gameManager;

    //[SerializeField]
    //private PlayerScript _playerScript;

    [SerializeField]
    Camera _gameCamera;

    [SerializeField]
    Collider _groundCollider;

    [SerializeField]
    Button _moveButton;

    [SerializeField]
    Button _collectButton;

    [SerializeField]
    NetworkView _networkView;

    // [SerializeField]
    // LineRenderer _lineMovement;

    [SerializeField]
    Button _endTurnButton;


    [SerializeField]
    GUI _menuJoueur;

    void Awake() {
        if(currentInputManagerScript == null) {
            DontDestroyOnLoad(gameObject);
            currentInputManagerScript = this;
        } else if(currentInputManagerScript != null) {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        if(Network.isClient) {
            //if(GUI BOUTTON MOVE)
            //{
            //    print("Choisir un point de déplacement :");

            //    if(Input.GetMouseButtonUp(0))
            //    {
            //        var ray = _gameCamera.ScreenPointToRay(Input.mousePosition);
            //        RaycastHit hitInfo;
            //        if(_groundCollider.Raycast(ray, out hitInfo, float.MaxValue))
            //        {
            //            CharacterActionMove moveAction = new CharacterActionMove(hitInfo.point));
            //        }
            //    }


            //}

            //if(GUI BOUTTON PICK)
            //{
            //    //Recupère les pièces dans une zone
            //}

            //if(GUI BOUTTON END TURN)
            //{
            //   // EXECUTE LES METHODE D'ACTION;
            //}

            if(Input.GetKeyDown(KeyCode.Space)) {
                // _gameManager.ExecuteTurnActionT();
                Debug.Log("Player " + gameObject.name + " wants to execute actions");
                _networkView.RPC("WantsToExecute", RPCMode.Server, Network.player);
            }

            if(Input.GetMouseButtonUp(0)) {
                var ray = _gameCamera.ScreenPointToRay(Input.mousePosition);

                RaycastHit hitInfo;

                if(_groundCollider.Raycast(ray, out hitInfo, float.MaxValue)) {
                    //Debug.Log("MoveToLocation :");
                    Debug.Log("Player " + gameObject.name + " wants to add a new waypoint : " + hitInfo.point);
                    //CharacterActionMove moveAction = new CharacterActionMove(hitInfo.point);
                    //_gameManager.AddActionInList(moveAction);
                    //_playerScript.AddActionInList(moveAction);
                    _networkView.RPC("WantsToAddWayPoint", RPCMode.Server, Network.player, hitInfo.point);
                }
            }

            if(Input.GetKeyDown(KeyCode.A)) {
                Debug.Log("Vous souhaitez vous déplacer, cliqué dans la direction désiré");
            }
        }
    }

    void OnGUI() {
        if(GUI.Button(new Rect(-300f, 50f, 100f, 30f), "Move")) {
            Debug.Log("Bouton move");
        }
    }
}



