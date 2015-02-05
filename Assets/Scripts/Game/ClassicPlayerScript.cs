using UnityEngine;
using System.Collections;

public class ClassicPlayerScript : MonoBehaviour
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

   // List<CharacterAction> _maList = new List<CharacterAction>();

    [SerializeField]
    public int playerId;


    void Start()
    {
        _squareShootDistance = Mathf.Pow(_shootDistance, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    other.
    //}

    #region implemented abstract members of APayerScript

    public void TryToMove(Vector3 pos)
    {
        Debug.Log(gameObject.name + "TRY TO MOVE TO : " + pos);
        _agent.SetDestination(pos);
    }

    public void TryToShoot()
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


    //public override void ExecuteAction(string actionName, Vector3 pos)
    //{
    //    Debug.Log(gameObject.name + "Execute action :" + actionName);
    //    _agent.SetDestination(pos);

    //   // throw new System.NotImplementedException();
    //}


   // public void ExecuteAction(string actionName, Vector3 pos)

    public void ExecuteActionT(CharacterAction action)
    {
       
        
        
        if(this.transform.position != action.getLocation())
        {
            Debug.Log(gameObject.name + "Execute action :" + action.GetActionName());
            _agent.SetDestination(action.getLocation());
            Debug.Log("Mon_AgentPATH:" + _agent.path);

            Debug.Log("JE VAIS A LA POSITION :" + action.getLocation());
        }
        //_agent.ResetPath();
        // throw new System.NotImplementedException();
    }

    #endregion
}
