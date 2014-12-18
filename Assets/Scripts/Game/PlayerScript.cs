using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript
{

    [SerializeField]
    NavMeshAgent _agent;

    [SerializeField]
    Transform _transform;

    [SerializeField]
    int _actionPoints;

    [SerializeField]
    ArrayList actionList;

    void Start()
    {
        actionList = new ArrayList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {

    }
/*
    public void ExecuteAction(string actionName)
    {

    }
 */

    public void ExecuteActionT(CharacterAction action)
    {
        /*
        if (this.transform.position != action.getLocation())
        {
            Debug.Log(gameObject.name + "Execute action :" + action.GetActionName());
            _agent.SetDestination(action.getLocation());
            Debug.Log("Mon_AgentPATH:" + _agent.path);

            Debug.Log("JE VAIS A LA POSITION :" + action.getLocation());
        }
        //_agent.ResetPath();
        // throw new System.NotImplementedException();
       */
    }
}
