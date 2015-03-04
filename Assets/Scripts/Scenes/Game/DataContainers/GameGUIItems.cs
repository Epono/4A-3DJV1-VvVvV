using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameGUIItems {

    [SerializeField]
    Camera _gameGUICamera;

    public Camera GameGUICamera {
        get { return _gameGUICamera; }
        set { _gameGUICamera = value; }
    }

    [SerializeField]
    Text _textTurnOrSimulationTimeRemaining;

    public Text TextTurnOrSimulationTimeRemaining {
        get { return _textTurnOrSimulationTimeRemaining; }
        set { _textTurnOrSimulationTimeRemaining = value; }
    }

    [SerializeField]
    Text _textGameTimeRemaining;

    public Text TextGameTimeRemaining {
        get { return _textGameTimeRemaining; }
        set { _textGameTimeRemaining = value; }
    }

    [SerializeField]
    Text _textScore;

    public Text TextScore {
        get { return _textScore; }
        set { _textScore = value; }
    }
};
