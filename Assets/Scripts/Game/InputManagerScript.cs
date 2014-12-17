using UnityEngine;
using System.Collections;

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
    float _groundDistance;

    private RaycastHit onTest;

    // MES AJOUTS
    //Le manager intelligent

    [SerializeField]
    LineRenderer _lineMovement;

    

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

                if (_groundCollider.Raycast(ray, out hitInfo, _groundDistance))
                {
                   // _gameManager.WantToMove(0, hitInfo.point);
                   
                    Debug.Log("MoveToLocation :");
                    _gameManager.AddActionInActionList("MoveToLocation");

                    onTest = hitInfo;
                    Debug.Log("Initialisation du raycast :");
                }

               
          }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _gameManager.ExecuteTurnAction(onTest.point);
            }

         
    }
}



