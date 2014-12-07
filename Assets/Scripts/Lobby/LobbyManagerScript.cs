using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LobbyManagerScript : MonoBehaviour
{
    //Singletonisation
    public static LobbyManagerScript currentLobbyManagerScript;

    [SerializeField]
    private Text[] _playersText;

    [SerializeField]
    Button _readyButton;

    [SerializeField]
    Text _readyButtonText;

    [SerializeField]
    Button _exitRoomButton;

    [SerializeField]
    NetworkView _networkView;

    [SerializeField]
    string _gameScene;

    [SerializeField]
    string _mainMenuScene;

    void Awake()
    {
        if (currentLobbyManagerScript == null)
        {
            DontDestroyOnLoad(gameObject);
            currentLobbyManagerScript = this;
        }
        else if (currentLobbyManagerScript != null)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (NetworkManagerScript.currentNetworkManagerScript._isServer)
        {
            Debug.Log("Server created, waiting for players ...");
        }
        else
        {
            Debug.Log("Lobby joined, waiting for other players ...");
        }
    }

    void Update()
    {

    }

    // Server-only method
    public void serverPlayerJoined(NetworkPlayer player)
    {
        PersistentPlayersScript.currentPersistentPlayersScript.addPlayer(player);

        Debug.Log(PersistentPlayersScript.currentPersistentPlayersScript.getPlayers().Count - 1);
        _playersText[PersistentPlayersScript.currentPersistentPlayersScript.getPlayers().Count - 1].text = player.externalIP;

        NetworkManagerScript.currentNetworkManagerScript._networkView.RPC("clientPlayerJoined", RPCMode.Others, PersistentPlayersScript.currentPersistentPlayersScript.getPlayers().Count, player.externalIP);

        string stringOfAllFloats = "";
        foreach (Text t in _playersText)
        {
            stringOfAllFloats += t.text.ToString() + ",";
        }
        stringOfAllFloats.TrimEnd(","[0]);
        NetworkManagerScript.currentNetworkManagerScript._networkView.RPC("clientGetPlayersText", player, stringOfAllFloats);


        if (PersistentPlayersScript.currentPersistentPlayersScript.getPlayers().Count == NetworkManagerScript.currentNetworkManagerScript._maxNumberOfConnections)
        {
            Debug.Log("Lobby full, launching the game ...");
            NetworkManagerScript.currentNetworkManagerScript._networkView.RPC("launchGameScene", RPCMode.Others);
            Application.LoadLevel(_gameScene);
        }
    }

    //Client-only method
    public void clientPlayerJoined(int index, string externalIP)
    {
        _playersText[index].text = externalIP;
    }

    //Client-only method
    public void clientGetPlayersText(string playersText)
    {
        string[] splittedPlayerstext = playersText.Split(","[0]);
        for (int i = 0; i < splittedPlayerstext.Length; i++)
        {
            _playersText[i].text = splittedPlayerstext[i];
        }
    }

    //Client-only method
    public void clientLaunchGameScene()
    {
        Application.LoadLevel(_gameScene);
    }
}
