using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour {

    [SerializeField]
    GameObject _playerGameObject;

    // initialisé au Start ?
    [SerializeField]
    NavMeshAgent _agent;

    [SerializeField]
    Transform _transform;

    [SerializeField]
    NetworkPlayer _networkPlayer;
    //

    [SerializeField]
    List<CharacterAction> _actionsList = new List<CharacterAction>();

    string playerName;
    int actionPoints;
    int score;

    CharacterAction currentAction;

    void Start() {
        score = 0;
        playerName = _playerGameObject.name;
    }

    void Update() {
        if(currentAction != null) {
            if(currentAction.IsFinished()) {
                currentAction = null;
            } else {
                currentAction.CheckIfFinished();
            }
        }
    }

    void FixedUpdate() {

    }

    public void AddActionInList(CharacterAction currentAction) {
        _actionsList.Add(currentAction);
        Debug.Log("Ajout action");
    }

    public void IncrementScore() {
        score++;
    }

    public int GetScore() {
        return score;
    }

    public void ExecuteNextAction() {
        currentAction = _actionsList[0];
        _actionsList.RemoveAt(0);
        Debug.Log("Executing action : " + currentAction);
        currentAction.Execute();
        Debug.Log("After executing action : " + currentAction);
    }

    public bool HasMoreActions() {
        return _actionsList.Count > 0;
    }

    public void ClearActionsList() {
        _actionsList.Clear();
        currentAction = null;
        Debug.Log(_actionsList.Count);
    }

    public NavMeshAgent GetAgent() {
        return _agent;
    }

    public void StopMove() {
        _agent.Stop();
        Debug.Log("Agent stopped !");
    }

    public CharacterAction GetCurrentAction() {
        return currentAction;
    }
}