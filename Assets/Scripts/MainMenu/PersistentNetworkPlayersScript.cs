using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/*
 * NetworkPlayers persistence for MainMenuScene => LobbyScene => GameScene
 * After that, PersistentePlayersScriptScript takes over 
 */
public class PersistentNetworkPlayersScript : MonoBehaviour {
    public static PersistentNetworkPlayersScript currentPersistentNetworkPlayersScript;

    List<NetworkPlayer> _players;

    public List<NetworkPlayer> Players {
        get { return _players; }
        set { _players = value; }
    }

    void Awake() {
        if(currentPersistentNetworkPlayersScript == null) {
            DontDestroyOnLoad(gameObject);
            currentPersistentNetworkPlayersScript = this;
        } else if(currentPersistentNetworkPlayersScript != null) {
            Destroy(gameObject);
        }
    }

    void Start() {
        _players = new List<NetworkPlayer>();
    }

    public void displayNetworkPlayers() {
        for(int i = 0; i < _players.Count; i++) {
            Debug.Log("guid : " + _players[i].guid);
            Debug.Log("ip : " + _players[i].ipAddress + " - port : " + _players[i].port);
            Debug.Log("externalIp : " + _players[i].externalIP + " - externalPort : " + _players[i].externalPort);
        }
    }
}