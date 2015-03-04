using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameOverGUIItems {

    [SerializeField]
    Camera _gameOverGUICamera;

    public Camera GameOverGUICamera {
        get { return _gameOverGUICamera; }
        set { _gameOverGUICamera = value; }
    }

    [SerializeField]
    Text _winnerOrLooserText;

    public Text WinnerOrLooserText {
        get { return _winnerOrLooserText; }
        set { _winnerOrLooserText = value; }
    }

    [SerializeField]
    Button _backToMainMenuButton;

    public Button BackToMainMenuButton {
        get { return _backToMainMenuButton; }
        set { _backToMainMenuButton = value; }
    }

    [SerializeField]
    List<GameOverGUIPlayerItems> _gameOverGuiPlayerItems;

    public List<GameOverGUIPlayerItems> GameOverGuiPlayerItems {
        get { return _gameOverGuiPlayerItems; }
        set { _gameOverGuiPlayerItems = value; }
    }
};