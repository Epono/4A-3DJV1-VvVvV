using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    //To ease the configuration
    [SerializeField]
    int _serverPort;

    [SerializeField]
    string _serverAdress;

    [SerializeField]
    int _maxNumberOfConnections;
    //

    [SerializeField]
    bool _buildingServer;

    [SerializeField]
    NetworkView _networkView;

    [SerializeField]
    Button _playButton;

    [SerializeField]
    Text _playButtonText;

    [SerializeField]
    Button _aboutButton;

    [SerializeField]
    Button _howToPlayButton;

    [SerializeField]
    Button _exitGameButton;

    [SerializeField]
    string _lobbyScene;

    //Useless here ?
    [SerializeField]
    string _gameScene;

    private bool _isServerRunning = false;
    private List<NetworkPlayer> _players;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Starting the game (" + (_buildingServer ? "Server mode" : "Client mode") + "), waiting for a player input on a button");

        //Specifies that the Application should be running when in background (mandatory if multiple instances)
        Application.runInBackground = true;

        if (_buildingServer)
        {
            _playButtonText.text = "Run Server";
        }

        _playButton.onClick.AddListener(() => { launchGame(); });
        _aboutButton.onClick.AddListener(() => { displayAboutInformations(); });
        _howToPlayButton.onClick.AddListener(() => { displayHowToPlayInformations(); });
        _exitGameButton.onClick.AddListener(() => { exitGame(); });
    }

    void launchGame()
    {
        //Tries to create the server
        if (_buildingServer)
        {
            if (!_isServerRunning)
            {
                Debug.Log("Trying to initialize the server ...");
                Network.InitializeSecurity();
                Network.InitializeServer(_maxNumberOfConnections, _serverPort, true);
            }
            else
            {
                Debug.Log("Closing the server ...");
                Network.Disconnect();
            }
        }

        //Or tries to connect to the server
        else
        {
            Debug.Log("Trying to connect to the server ...");
            Network.Connect(_serverAdress, _serverPort);
        }
    }



    //Server
    void OnServerInitialized()
    {
        _isServerRunning = true;
        _playButtonText.text = "Close Server";
        _players = new List<NetworkPlayer>();
        Debug.Log("Server created, waiting for players ...");
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        _players.Add(player);
        Debug.Log("Player [" + player + "] connected");
        if (_players.Count == 1)
        {
            Application.LoadLevel(_lobbyScene);
        }
        else if (_players.Count == _maxNumberOfConnections)
        {
            Debug.Log("Launching the game ...");
            Application.LoadLevel(_gameScene);
        }
    }

    //Doesn't gets called, abnormal
    void OnPlayerDisConnected(NetworkPlayer player)
    {
        _players.Remove(player);
        Debug.Log("Player [" + player + "] disconnected");
    }

    //Client
    void OnFailedToConnect(NetworkConnectionError error)
    {
        Debug.Log("Couldn't connect to the server : " + error);
        //Display an error message, and asks if the player wants to retry to connect
    }

    void OnConnectedToServer()
    {
        Debug.Log("Connected to the server");
        Application.LoadLevel(_lobbyScene);
    }

    //Both
    void OnDisconnectedFromServer(NetworkDisconnection networkDisconnection)
    {
        if (_buildingServer)
        {
            _isServerRunning = false;
            _playButtonText.text = "Run Server";
            Debug.Log("Server disconnection successful : " + networkDisconnection);
        }
        else
        {
            Debug.Log("Disconnected from the server : " + networkDisconnection);
        }
    }

    //TODO
    void displayAboutInformations()
    {
        Debug.Log("Display about informations");
    }

    //TODO
    void displayHowToPlayInformations()
    {
        Debug.Log("Display howToPlay informations");
    }

    void exitGame()
    {
        Debug.Log("Exiting the game ...");

        //Doesn't work in Editor mode
        Application.Quit();
    }

    public void Update()
    {

    }
}