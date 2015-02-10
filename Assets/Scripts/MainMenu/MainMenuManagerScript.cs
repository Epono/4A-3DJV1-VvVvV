using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenuManagerScript : MonoBehaviour {

    [SerializeField]
    Button _playButton;

    [SerializeField]
    Button _aboutButton;

    [SerializeField]
    Button _howToPlayButton;

    [SerializeField]
    Button _exitGameButton;

    [SerializeField]
    Button _optionsButton;

    [SerializeField]
    Toggle _fullScreenToggle;

    [SerializeField]
    Button _1080pButton;

    [SerializeField]
    Button _720pButton;

    [SerializeField]
    Button _480pButton;

    [SerializeField]
    Button _backButton;

    [SerializeField]
    CanvasGroup _mainCanvasGroup;

    [SerializeField]
    CanvasGroup _optionsCanvasGroup;

    //[SerializeField]
    //Slider _resolutionSlider;

    [SerializeField]
    string _gameScene;

    [SerializeField]
    string _lobbyScene;

    void Start() {
        NetworkManagerScript.currentNetworkManagerScript._mainMenuManagerScript = this;

        Debug.Log("Starting the game (" + (NetworkManagerScript.currentNetworkManagerScript._isServer ? "Server mode" : "Client mode") + "), waiting for a player input on a button");

        //Specifies that the Application should be running when in background (mandatory if multiple instances)
        Application.runInBackground = true;

        initScene();
    }

    public void initScene() {
        if(NetworkManagerScript.currentNetworkManagerScript._isServer) {
            NetworkManagerScript.currentNetworkManagerScript.runServer();
        } else {
            _playButton.onClick.AddListener(() => { NetworkManagerScript.currentNetworkManagerScript.TryToConnectToServer(); });
        }

        _aboutButton.onClick.AddListener(() => { displayAboutInformations(); });
        _exitGameButton.onClick.AddListener(() => { exitGame(); });
        _optionsButton.onClick.AddListener(() => { displayOptions(); });
        //_howToPlayButton.onClick.AddListener(() => { displayHowToPlayInformations(); });

        _1080pButton.onClick.AddListener(() => { SetResolution(1920, 1080); });
        _720pButton.onClick.AddListener(() => { SetResolution(1280, 720); });
        _480pButton.onClick.AddListener(() => { SetResolution(640, 480); });
        _backButton.onClick.AddListener(() => { BackToMainMenu(); });
    }

    public void Update() {
        if(Input.GetKeyDown(KeyCode.Return) && !NetworkManagerScript.currentNetworkManagerScript._isServer) {
            NetworkManagerScript.currentNetworkManagerScript.TryToConnectToServer();
        }
    }

    //TODO
    void displayAboutInformations() {
        Debug.Log("Display about informations");
    }

    /*
    //TODO: To implement
    void displayHowToPlayInformations() {
        Debug.Log("Display howToPlay informations");
    }
    */

    //TODO
    public void displayOptions() {
        //_fullScreenToggle.isOn = Screen.fullScreen;
        /*
        _mainCanvasGroup.alpha = 0;
        _playButton.enabled = false;
        _aboutButton.enabled = false;
        _optionsButton.enabled = false;
        _exitGameButton.enabled = false;

        _optionsCanvasGroup.alpha = 1;
        _480pButton.enabled = true;
        _720pButton.enabled = true;
        _1080pButton.enabled = true;
        _fullScreenToggle.enabled = true;
        _backButton.enabled = true;
        */
        ToggleDisplayOptions(true);
    }

    //TODO: Change so that buttons in background don't fuck up buttons in foreground
    // Use the Align Along Ratio thing Script
    public void ToggleDisplayOptions(bool isDisplayingOptions) {
        if(isDisplayingOptions) {
            _mainCanvasGroup.alpha = 0;
            _mainCanvasGroup.blocksRaycasts = false;
            _optionsCanvasGroup.alpha = 1;
            _optionsCanvasGroup.blocksRaycasts = true;
        } else {
            _mainCanvasGroup.alpha = 1;
            _mainCanvasGroup.blocksRaycasts = true;
            _optionsCanvasGroup.alpha = 0;
            _optionsCanvasGroup.blocksRaycasts = false;
        }

        /*
        _playButton.interactable = !isDisplayingOptions;
        _aboutButton.interactable = !isDisplayingOptions;
        _optionsButton.interactable = !isDisplayingOptions;
        _exitGameButton.interactable = !isDisplayingOptions;

        _480pButton.interactable = isDisplayingOptions;
        _720pButton.interactable = isDisplayingOptions;
        _1080pButton.interactable = isDisplayingOptions;
        _fullScreenToggle.interactable = isDisplayingOptions;
        _backButton.interactable = isDisplayingOptions;
        */
    }

    public void SetResolution(int width, int height) {
        Screen.SetResolution(width, height, Screen.fullScreen);
    }

    public void ToggleFullScreen() {
        Screen.fullScreen = !Screen.fullScreen;
    }

    /*
    public void OnResolutionSliderChange {
        // _resolutionSlider.
    }

    public void ApplyResolution() {
        //Screen.SetResolution(width, height, Screen.fullScreen);
    }
    */

    public void BackToMainMenu() {
        /*
        _mainCanvasGroup.alpha = 1;
        _optionsCanvasGroup.alpha = 0;
        */
        ToggleDisplayOptions(false);
    }

    void exitGame() {
        Debug.Log("Exiting the game ...");

        //Doesn't work in Editor mode
        Application.Quit();
    }

    public void failedToConnect(NetworkConnectionError error) {
        Debug.Log("Couldn't connect to the server : " + error);
        //Display an error message, and asks if the player wants to retry to connect
    }

    public void disconnectedFromServer(NetworkDisconnection networkDisconnection) {
        if(NetworkManagerScript.currentNetworkManagerScript._isServer) {
            //  _playButtonText.text = "Run Server";
            Debug.Log("Server disconnection successful : " + networkDisconnection);
        } else {
            Debug.Log("Disconnected from the server : " + networkDisconnection);
        }
    }
}