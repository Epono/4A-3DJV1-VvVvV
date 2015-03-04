using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameOverGUIPlayerItems {
    [SerializeField]
    Text _playerNameLabel;

    public Text PlayerNameLabel {
        get { return _playerNameLabel; }
        set { _playerNameLabel = value; }
    }

    [SerializeField]
    Text _playerScore;

    public Text PlayerScore {
        get { return _playerScore; }
        set { _playerScore = value; }
    }
};
