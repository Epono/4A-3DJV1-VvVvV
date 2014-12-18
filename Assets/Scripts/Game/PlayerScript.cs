using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour {

    [SerializeField]
    NavMeshAgent _agent;

    [SerializeField]
    Transform _transform;

    [SerializeField]
    int _actionPoints;

    [SerializeField]
    public List<CharacterAction> actionList;

    void Start() {
        actionList = new List<CharacterAction>();
    }

    void Update() {

    }

    void FixedUpdate() {

    }

    public void AddActionInList(CharacterAction currentAction) {
        actionList.Add(currentAction);
    }

    public void ExecuteActionT(CharacterAction action) {
        if(this.transform.position != action.getLocation()) {
            Debug.Log(gameObject.name + "Execute action :" + action.GetActionName());
            _agent.SetDestination(action.getLocation());
            Debug.Log("Mon_AgentPATH:" + _agent.path);

            Debug.Log("JE VAIS A LA POSITION :" + action.getLocation());
        }
        //_agent.ResetPath();
        // throw new System.NotImplementedException();
    }
}
