using UnityEngine;
using System.Collections;

public class InputManagerScript : MonoBehaviour
{

    [SerializeField]
    private GameManagerScript _gameManager;

    [SerializeField]
    Camera _gameCamera;

    [SerializeField]
    Collider _groundCollider;

    [SerializeField]
    float _groundDistance;

    //[SerializeField]
    //NetworkView _networkView;

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
                    _gameManager.WantToMove(0, hitInfo.point);
                }
          }
    }
}



