using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputManagerScript : MonoBehaviour {

    //public static InputManagerScript currentInputManagerScript;

    [SerializeField]
    GameManagerScript _gameManager;

    [SerializeField]
    PlayerScript[] _playerScript;

    [SerializeField]
    Camera _gameCamera;

    [SerializeField]
    Collider _groundCollider;

    [SerializeField]
    NetworkView _networkView;

    // [SerializeField]
    // LineRenderer _lineMovement;

    [SerializeField]
    Button _collectCoinsButton;

    [SerializeField]
    Button _addWayPointButton;

    [SerializeField]
    Button _finishTurnButton;

    [SerializeField]
    Button _cancelButton;

    Vector3 clickPoint = Vector3.zero;

    //void Awake() {
    //    if(currentInputManagerScript == null) {
    //        DontDestroyOnLoad(gameObject);
    //        currentInputManagerScript = this;
    //    } else if(currentInputManagerScript != null) {
    //        Destroy(gameObject);
    //    }
    //}

    // Use this for initialization
    void Start() {
        _collectCoinsButton.onClick.AddListener(() => {
            //WantsToCollectCoins();
            _networkView.RPC("WantsToCollectCoins", RPCMode.Server, Network.player);
        });
        _addWayPointButton.onClick.AddListener(() => {
            //WantsToAddWayPoint();
            _networkView.RPC("WantsToAddWayPoint", RPCMode.Server, Network.player, clickPoint);
        });
        _finishTurnButton.onClick.AddListener(() => {
            //WantsToFinishTurn(); 
            _networkView.RPC("WantsToFinishTurn", RPCMode.Server, Network.player);
        });
        _cancelButton.onClick.AddListener(() => {
            CancelClick();
        });
    }

    // Update is called once per frame
    void Update() {

        if(Network.isClient) {
            // End Turn
            if(Input.GetKeyDown(KeyCode.Space)) {
                //WantsToFinishTurn();
                _networkView.RPC("WantsToAddWayPoint", RPCMode.Server, Network.player);
            }

            if(Input.GetKeyDown(KeyCode.Return)) {
                //WantsToFinishTurn();
                _networkView.RPC("WantsToFinishTurn", RPCMode.Server, Network.player);
            }

            if(Input.GetKeyDown(KeyCode.A)) {
                _networkView.RPC("WantsToCollectCoins", RPCMode.Server, Network.player);
            }

            // Initialize clickPoint (PC)
            if(Input.GetMouseButtonUp(0)) {
                var ray = _gameCamera.ScreenPointToRay(Input.mousePosition);

                RaycastHit hitInfo;

                if(_groundCollider.Raycast(ray, out hitInfo, float.MaxValue)) {
                    clickPoint = hitInfo.point;
                }
            }

            // Initialize clickPoint (Android)
            if(Input.touchCount > 0) {
                Touch touch = Input.GetTouch(0);

                if(touch.phase == TouchPhase.Ended) {
                    var ray = _gameCamera.ScreenPointToRay(touch.position);

                    RaycastHit hitInfo;

                    if(_groundCollider.Raycast(ray, out hitInfo, float.MaxValue)) {
                        clickPoint = hitInfo.point;
                    }
                }
            }
        }
    }

    //[RPC]
    //void WantsToFinishTurn() {
    //    //_networkView.RPC("WantsToFinishTurn", RPCMode.Server, Network.player);
    //    //_gameManager.WantsToFinishTurn(Network.player);
    //    _gameManager._networkView.RPC("WantsToFinishTurn", RPCMode.Server, Network.player);
    //}

    //[RPC]
    //void WantsToAddWayPoint() {
    //    if(clickPoint != Vector3.zero) {
    //        //_networkView.RPC("WantsToAddWayPoint", RPCMode.Server, Network.player, clickPoint);
    //        //_gameManager.WantsToAddWayPoint(Network.player, clickPoint);
    //        _gameManager._networkView.RPC("WantsToAddWayPoint", RPCMode.Server, Network.player, clickPoint);
    //    }
    //}

    //[RPC]
    //void WantsToCollectCoins() {
    //    //_networkView.RPC("WantsToCollectCoins", RPCMode.Server, Network.player);
    //    _gameManager.WantsToCollectCoins(Network.player);
    //    //_gameManager._networkView.RPC("WantsToCollectCoins", RPCMode.Server, Network.player);
    //}

    //void ExecuteActions() {
    //    Debug.Log("Player " + gameObject.name + " wants to execute actions");
    //    _networkView.RPC("WantsToFinishTurn", RPCMode.Server, Network.player);
    //}

    void CancelClick() {
        clickPoint = Vector3.zero;
    }
}