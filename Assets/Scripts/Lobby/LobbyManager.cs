using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LobbyManager : MonoBehaviour
{
    [SerializeField]
    Text _player1text;

    [SerializeField]
    Text _player2text;

    [SerializeField]
    Text _player3text;

    [SerializeField]
    Button _readyButton;

    [SerializeField]
    Text _readyButtonText;

    [SerializeField]
    Button _exitRoomButton;

    [SerializeField]
    string _gameScene;

    [SerializeField]
    string _mainMenuScene;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
