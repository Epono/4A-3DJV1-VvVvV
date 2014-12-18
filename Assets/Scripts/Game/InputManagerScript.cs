using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputManagerScript : MonoBehaviour
{
    //Singletonisation
    public static InputManagerScript currentInputManagerScript;

    [SerializeField]
    private GameManagerScript _gameManager;

    [SerializeField]
    Camera _gameCamera;

    [SerializeField]
    Collider _groundCollider;

    [SerializeField]
    Button _moveButton;

    [SerializeField]
    Button _collectButton;

    // [SerializeField]
    // LineRenderer _lineMovement;

    [SerializeField]
    Button _endTurnButton;


    [SerializeField]
    GUI _menuJoueur;

    void Awake()
    {
        if (currentInputManagerScript == null)
        {
            DontDestroyOnLoad(gameObject);
            currentInputManagerScript = this;
        }
        else if (currentInputManagerScript != null)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _gameManager.ExecuteTurnActionT();
        }

        if (Input.GetMouseButtonUp(0))
        {
            var ray = _gameCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo;

            if (_groundCollider.Raycast(ray, out hitInfo, float.MaxValue))
            {
                Debug.Log("MoveToLocation :");
                CharacterActionMove moveAction = new CharacterActionMove(hitInfo.point);
                _gameManager.AddActionInList(moveAction);
            }


            /*
                var ray = _gameCamera.ScreenPointToRay(Input.mousePosition);

                RaycastHit hitInfo;
                Debug.Log("J'ai cliqué !");
                if (_groundCollider.Raycast(ray, out hitInfo, float.MaxValue))
                {
                    //_gameManager.addAction();
                   
                    Debug.Log("MoveToLocation :");

                }
             * */
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

        }


        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Vous souhaitez vous déplacer, cliqué dans la direction désiré");

        }
    }


    void OnGUI()
    {
        if (GUI.Button(new Rect(-300f, 50f, 100f, 30f), "Move"))
        {
            Debug.Log("Bouton move");
        }
    }
}



