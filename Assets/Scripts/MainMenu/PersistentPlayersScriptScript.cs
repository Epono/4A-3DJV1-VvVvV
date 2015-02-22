using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/*
 * PlayerScript persistence for GameScene => GameOverScene 
 */
public class PersistentPlayersScriptScript : MonoBehaviour {
    public static PersistentPlayersScriptScript currentPersistentPlayersScriptScript;

    [SerializeField]
    List<PlayerScript> _playersScript;

    public List<PlayerScript> PlayersScript {
        get { return _playersScript; }
        set { _playersScript = value; }
    }

    int numberOfPlayersConnected;

    public int NumberOfPlayersConnected {
        get { return numberOfPlayersConnected; }
        set { numberOfPlayersConnected = value; }
    }

    void Awake() {
        if(currentPersistentPlayersScriptScript == null) {
            DontDestroyOnLoad(gameObject);
            currentPersistentPlayersScriptScript = this;
        } else if(currentPersistentPlayersScriptScript != null) {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start() {
        numberOfPlayersConnected = NetworkManagerScript.currentNetworkManagerScript.MaxNumberOfConnections;
    }

    public void playerConnected(NetworkPlayer player) {
        _playersScript[numberOfPlayersConnected].NetworkPlayer = player;
        _playersScript[numberOfPlayersConnected].IsConnected = true;
        numberOfPlayersConnected++;
    }

    public void playerDisconnected(NetworkPlayer player) {
        Debug.Log("Player [" + player.ipAddress + "] disconnected");
        foreach(PlayerScript playerScript in _playersScript) {
            if((player.ipAddress == playerScript.NetworkPlayer.ipAddress) && (player.port == playerScript.NetworkPlayer.port)) {
                playerScript.IsConnected = false;
                break;
            }
        }
        numberOfPlayersConnected--;
    }

    public void displayNetworkPlayersScript() {
        for(int i = 0; i < _playersScript.Count; i++) {
            Debug.Log("guid : " + _playersScript[i].NetworkPlayer.guid);
            Debug.Log("ip : " + _playersScript[i].NetworkPlayer.ipAddress + " - port : " + _playersScript[i].NetworkPlayer.port);
            Debug.Log("externalIp : " + _playersScript[i].NetworkPlayer.externalIP + " - externalPort : " + _playersScript[i].NetworkPlayer.externalPort);
        }
    }
}