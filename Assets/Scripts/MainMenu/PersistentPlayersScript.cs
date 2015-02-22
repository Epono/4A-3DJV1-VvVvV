using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PersistentPlayersScript : MonoBehaviour {
    //Singleton
    public static PersistentPlayersScript currentPersistentPlayersScript;

    private List<NetworkPlayer> _players;

    private List<PlayerScript> _playersScript;

    public int nbPlayersConnected = 0;

    void Awake() {
        if(currentPersistentPlayersScript == null) {
            DontDestroyOnLoad(gameObject);
            currentPersistentPlayersScript = this;
        } else if(currentPersistentPlayersScript != null) {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start() {
        Debug.Log("Initialization of players' list");
        _players = new List<NetworkPlayer>();
        _playersScript = new List<PlayerScript>(3);
        nbPlayersConnected = 0;
    }

    public void playerConnected(NetworkPlayer player) {
        _players.Add(player);
        _playersScript[nbPlayersConnected]._networkPlayer = player;
        nbPlayersConnected++;
    }

    public void playerDisconnected(NetworkPlayer player) {
        Debug.Log("Player [" + player.ipAddress + "] disconnected");
        _players.Remove(player);
        for(int i = 0; i < _playersScript.Count; i++) {
            if((player.ipAddress == _playersScript[i]._networkPlayer.ipAddress) && (player.port == _playersScript[i]._networkPlayer.port)) {
                _playersScript[]._networkPlayer = null;
                _playersScript[].IsConnected = false;
                break;
            }
        }
        nbPlayersConnected--;
    }

    [System.Obsolete]
    public List<NetworkPlayer> getPlayers() {
        return _players;
    }

    public List<PlayerScript> getPlayersScript() {
        return _playersScript;
    }

    [System.Obsolete]
    public void displayNetworkPlayers() {
        for(int i = 0; i < _players.Count; i++) {
            Debug.Log("guid : " + _players[i].guid);
            Debug.Log("ip : " + _players[i].ipAddress + " - port : " + _players[i].port);
            Debug.Log("externalIp : " + _players[i].externalIP + " - externalPort : " + _players[i].externalPort);
        }
    }

    public void displayNetworkPlayersScript() {
        for(int i = 0; i < _players.Count; i++) {
            Debug.Log("guid : " + _playersScript[i]._networkPlayer.guid);
            Debug.Log("ip : " + _playersScript[i]._networkPlayer.ipAddress + " - port : " + _playersScript[i]._networkPlayer.port);
            Debug.Log("externalIp : " + _playersScript[i]._networkPlayer.externalIP + " - externalPort : " + _playersScript[i]._networkPlayer.externalPort);
        }
    }
}