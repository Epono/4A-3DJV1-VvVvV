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
        if (Input.GetMouseButtonUp(0))
        {
            
                var ray = _gameCamera.ScreenPointToRay(Input.mousePosition);

                RaycastHit hitInfo;
                Debug.Log("J'ai cliqué !");
                if (_groundCollider.Raycast(ray, out hitInfo, float.MaxValue))
                {
                    //_gameManager.addAction();
                   
                    Debug.Log("MoveToLocation :");

                }
          }

            if (Input.GetKeyDown(KeyCode.Space))
            {

            }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(-300f,50f,100f,30f), "Move"))
        {
            Debug.Log("Bouton move");
        }
    }
}



