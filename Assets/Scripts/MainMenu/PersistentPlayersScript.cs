using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PersistentPlayersScript : MonoBehaviour
{
    //Singleton
    public static PersistentPlayersScript currentPersistentPlayersScript;

    //[SerializeField]
    private List<NetworkPlayer> _players;

    void Awake()
    {
        if (currentPersistentPlayersScript == null)
        {
            DontDestroyOnLoad(gameObject);
            currentPersistentPlayersScript = this;
        }
        else if (currentPersistentPlayersScript != null)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("Initialization of players' list");
        _players = new List<NetworkPlayer>();
    }

    public void addPlayer(NetworkPlayer player)
    {
        _players.Add(player);

        /*
        if (_players.Count == 1)
        {
            Application.LoadLevel(_lobbyScene);
        }
        else if (_players.Count == _networkManagerScript._maxNumberOfConnections)
        {
            Debug.Log("Launching the game ...");
            Application.LoadLevel(_gameScene);
        }
         */
    }

    public void playerDisconnected(NetworkPlayer player)
    {
        Debug.Log("Player [" + player.ipAddress + "] disconnected");
        _players.Remove(player);
    }

    public List<NetworkPlayer> getPlayers()
    {
        return _players;
    }

    // Update is called once per frame
    void Update()
    {

    }
}