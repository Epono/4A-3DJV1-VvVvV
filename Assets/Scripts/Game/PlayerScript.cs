﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour {

    [SerializeField]
    public NavMeshAgent _agent;

    [SerializeField]
    Transform _transform;

    int _actionPoints;
    public int score = 0;

    [SerializeField]
    public List<CharacterAction> actionList = new List<CharacterAction>();

    void Start() {
        //actionList = 
    }

    void Update() {

    }

    void FixedUpdate() {

    }

    public void AddActionInList(CharacterAction currentAction) {
        actionList.Add(currentAction);
    }

    public void ExecuteActionT(CharacterAction action) {
        //if(this.transform.position != action.getLocation()) {
        Debug.Log(gameObject.name + "Execute action :" + action.GetActionName());
        _agent.SetDestination(action.getLocation());
        actionList.Remove(action);
        Debug.Log("Mon_AgentPATH:" + _agent.path);

        Debug.Log("JE VAIS A LA POSITION :" + action.getLocation());
        //}
        //_agent.ResetPath();
        // throw new System.NotImplementedException();
    }
}