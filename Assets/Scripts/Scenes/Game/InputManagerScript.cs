using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InputManagerScript : MonoBehaviour {

    public static InputManagerScript currentInputManagerScript;

    GameManagerScript gameManagerScript;

    public GameManagerScript GameManagerScript {
        get { return gameManagerScript; }
        set { gameManagerScript = value; }
    }

    [SerializeField]
    HelpersScript _helpersScript;

    [SerializeField]
    PlayerScript[] _playerScript;

    [SerializeField]
    Camera _gameCamera;

    [SerializeField]
    Collider _groundCollider;

    [SerializeField]
    NetworkView _networkView;

    [SerializeField]
    Button _collectCoinsButton;

    [SerializeField]
    Button _addWayPointButton;

    [SerializeField]
    Button _finishTurnButton;

    [SerializeField]
    Button _cancelButton;

    Vector3 clickPoint = Vector3.zero;

    public Vector3 ClickPoint {
        get { return clickPoint; }
        set { clickPoint = value; }
    }

    void Start() {
        currentInputManagerScript = this;

        _collectCoinsButton.onClick.AddListener(() => {
            _networkView.RPC("WantsToCollectCoins", RPCMode.Server, Network.player);
        });
        _addWayPointButton.onClick.AddListener(() => {
            _networkView.RPC("WantsToAddWayPoint", RPCMode.Server, Network.player, clickPoint);
            _helpersScript.AddWaypoint(clickPoint);
            CancelClick();
        });
        _finishTurnButton.onClick.AddListener(() => {
            _networkView.RPC("WantsToFinishTurn", RPCMode.Server, Network.player);
        });
        _cancelButton.onClick.AddListener(() => {
            CancelClick();
        });
    }

    // Update is called once per frame
    void Update() {
        if(Network.isClient/* && gameManagerScript.IsPlanificating*/) {

            if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) {
                _networkView.RPC("WantsToAddWayPoint", RPCMode.Server, Network.player, clickPoint);
                _helpersScript.AddWaypoint(clickPoint);
                CancelClick();
            }

            if(Input.GetKeyDown(KeyCode.Return)) {
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
                    _helpersScript.ClickPointChanged();
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
                        _helpersScript.ClickPointChanged();
                    }
                }
            }
        }
    }

    void CancelClick() {
        clickPoint = Vector3.zero;
        _helpersScript.ClickPointChanged();
    }
}