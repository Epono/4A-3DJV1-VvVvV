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

   // [SerializeField]
   // LineRenderer _lineMovement;

    

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

               
           }

            if (Input.GetKeyDown(KeyCode.Space))
            {
               _gameManager.ExecuteTurnActionT();
            }

            if(Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("Vous souhaitez vous déplacer, cliqué dans la direction désiré");

            }
            
         
    }
}



