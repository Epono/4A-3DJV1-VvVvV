using UnityEngine;
using System.Collections;

public class ClassicPlayerScript : APlayerScript
{

    [SerializeField]
    NavMeshAgent _agent;

    [SerializeField]
    Transform[] _ballsTransform;


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
    #endregion
}
