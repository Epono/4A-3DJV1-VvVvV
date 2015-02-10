using UnityEngine;
using System.Collections;

public class CharacterActionMove : CharacterAction {

    Vector3 _location;
    NavMeshAgent _agent;

    public CharacterActionMove(Vector3 locationChoose, NavMeshAgent agent) {
        actionPoint = 1;
        actionName = "MoveToLocation";
        _location = locationChoose;
        _agent = agent;
    }

    public override void Execute() {
        Debug.Log("L'agent " + _agent.gameObject.name + " se deplace vers " + _location);
        _agent.SetDestination(_location);
    }

    public Vector3 getLocation() {
        return _location;
    }

    public override void CheckIfFinished() {
        if(!_agent.pathPending) {
            if(_agent.remainingDistance <= _agent.stoppingDistance) {
                if(!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f) {
                    isFinished = true;
                }
            }
        }
    }
}
