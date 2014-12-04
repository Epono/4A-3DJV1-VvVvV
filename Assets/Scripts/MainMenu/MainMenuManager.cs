using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{

    // Appelle directement la prochaine scène (matchmaking room) ou reste sur celle là pednant ce temps ?
    // [SerializeField]
    // string _nextScene;

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

    //To ease the configuration
    [SerializeField]
    int _serverPort;

    [SerializeField]
    string _serverAdress;

    [SerializeField]
    int _maxNumberOfConnections;

    private bool _isServerRunning = false;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Starting the game, waiting for a player input on a button");

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

    void OnDisconnectedFromServer(NetworkDisconnection networkDisconnection)
    {
        //If it's called in the server, means the server disconnected (probably by user input)
        if (_buildingServer)
        {
            _isServerRunning = false;
            _playButtonText.text = "Run Server";
            Debug.Log("Server disconnection successful : " + networkDisconnection);
        }
        else
        {
            Debug.Log("Shouldn't (I think) ever gets called"); ;
        }
        
    }

    void OnServerInitialized()
    {
        _isServerRunning = true;
        _playButtonText.text = "Close Server";
        Debug.Log("Server created, waiting for players ...");
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        Debug.Log("Couldn't connect to the server : " + error);
    }

    void OnConnectedToServer()
    {
        Debug.Log("Connected to the server");
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