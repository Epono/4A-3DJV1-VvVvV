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
    public NetworkPlayer _networkPlayer;
    //

    string primaryKey;

    [SerializeField]
    bool isConnected = false;

    public bool IsConnected {
        get { return isConnected; }
        set { isConnected = value; }
    }

    public string PrimaryKey {
        get { return primaryKey; }
        set { primaryKey = value; }
    }

    [SerializeField]
    List<CharacterAction> _actionsList = new List<CharacterAction>();

    string playerName;
    public string PlayerName {
        get { return playerName; }
        set { playerName = value; }
    }

    int actionPoints;
    public int ActionPoints {
        get { return actionPoints; }
        set { actionPoints = value; }
    }

    int score;
    public int Score {
        get { return score; }
        set { score = value; }
    }

    CharacterAction currentAction;

    void Start() {
        score = 0;
        playerName = _playerGameObject.name;
        primaryKey = _networkPlayer.ipAddress + ":" + _networkPlayer.port;
        isConnected = true;
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

    public void AddActionInList(CharacterAction currentAction) {
        _actionsList.Add(currentAction);
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
        currentAction.Execute();
    }

    public bool HasMoreActions() {
        return _actionsList.Count > 0;
    }

    public void ClearActionsList() {
        _actionsList.Clear();
        currentAction = null;
    }

    public NavMeshAgent GetAgent() {
        return _agent;
    }

    public void StopMove() {
        _agent.Stop();
    }

    public CharacterAction GetCurrentAction() {
        return currentAction;
    }
}