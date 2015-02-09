using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    [SerializeField]
    GameObject _playerGameObject;

    [SerializeField]
    int _score = 0;

    [SerializeField]
    NetworkPlayer _networkPlayer;

    [SerializeField]
    string _playerName = "player0";

	void Start () {
        _score = 0;
        _playerName = "player0";
	}
	
	void Update () {
	
	}

    public void IncrementScore() {
        _score++;
    }

    public int GetScore() {
        return _score;
    }
}
