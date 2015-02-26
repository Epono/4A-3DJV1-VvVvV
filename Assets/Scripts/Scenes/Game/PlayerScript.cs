using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour {

    [SerializeField]
    GameObject _playerGameObject;

    [SerializeField]
    NavMeshAgent _agent;

    [SerializeField]
    Transform _transform;

    [SerializeField]
    NetworkPlayer _networkPlayer;

    public NetworkPlayer NetworkPlayer {
        get { return _networkPlayer; }
        set { _networkPlayer = value; }
    }

    string primaryKey;
    public string PrimaryKey {
        get { return primaryKey; }
        set { primaryKey = value; }
    }

    [SerializeField]
    bool isConnected = false;

    public bool IsConnected {
        get { return isConnected; }
        set { isConnected = value; }
    }

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

    [SerializeField]
    int index;

    public int Index {
        get { return index; }
        set { index = value; }
    }

    List<CharacterAction> _actionsList = new List<CharacterAction>();

    public List<CharacterAction> ActionsList {
        get { return _actionsList; }
        set { _actionsList = value; }
    }

    CharacterAction currentAction;

    public CharacterAction CurrentAction {
        get { return currentAction; }
        set { currentAction = value; }
    }

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