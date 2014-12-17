using UnityEngine;
using System.Collections;

public class ClassicPlayerScript : APlayerScript
{

    [SerializeField]
    NavMeshAgent _agent;

    [SerializeField]
    Transform[] _ballsTransform;

    [SerializeField]
    LineRenderer _lineMovement;

    [SerializeField]
    Rigidbody[] _ballsRigidBody;

    [SerializeField]
    Transform _transform;

    [SerializeField]
    float _shootDistance;

    [SerializeField]
    float _shootImpulse;

    float _squareShootDistance;
    // Use this for initialization

    void Start()
    {
        _squareShootDistance = Mathf.Pow(_shootDistance, 2);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        //Affichage d'un trait pour symboliser le déplacement

        if(Input.GetKeyDown(KeyCode.A))
        {
         //   Debug.Log("x" + Input.mousePosition.x);
        //    _lineMovement.SetPosition(1, new Vector3(Input.mousePosition.x-_transform.position.x, 0, Input.mousePosition.y));
           //  Input.mousePosition.z);
            //Debug.Log(Input.mousePresent.)

          //  _lineMovement.SetWidth(this._transform.position.x, Input.mousePosition.x);
        }
        
    }
    #region implemented abstract members of APayerScript

    public override void TryToMove(Vector3 pos)
    {
        Debug.Log(gameObject.name + "TRY TO MOVE TO : " + pos);
        _agent.SetDestination(pos);
    }

    public override void TryToShoot()
    {
        Debug.Log(gameObject.name + " shoots");
        for (var i = 0; i < _ballsTransform.Length; i++)
        {
            var b = _ballsTransform[i];
            var brigid = _ballsRigidBody[i];

            var distanceVector = b.position - _transform.position;

            if (distanceVector.sqrMagnitude <= _squareShootDistance)
            {
                brigid.AddForce(distanceVector.normalized * _shootImpulse, ForceMode.Impulse);
            }
        }
    }

    public override void ExecuteAction(string actionName, Vector3 pos)
    {
        Debug.Log(gameObject.name + "Execute action :" + actionName);
        _agent.SetDestination(pos);

       // throw new System.NotImplementedException();
    }
    #endregion
}
