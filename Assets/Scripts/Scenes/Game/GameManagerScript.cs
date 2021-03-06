﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManagerScript : MonoBehaviour {

    public static GameManagerScript currentGameManagerScript;

    [SerializeField]
    GameObject[] _playersGameObject;

    List<PlayerScript> _playerScript;

    //TODO: normaliser, soit networkplayer, soit playerScript
    Dictionary<NetworkPlayer, PlayerScript> playerScriptFromNetworkPlayer = new Dictionary<NetworkPlayer, PlayerScript>();

    Dictionary<PlayerScript, bool> playersWantsToFinishTurn = new Dictionary<PlayerScript, bool>();
    bool finishTurn;

    // Dico pour association player <=> scores (gameover)
    //Dictionary<string, GameOverGUIPlayerItems> playerScoreTextFromNetworkPlayer = new Dictionary<string, GameOverGUIPlayerItems>();
    //GameOverGUIPlayerItems[] playerScoreTextFromNetworkPlayer = new GameOverGUIPlayerItems[NetworkManagerScript.currentNetworkManagerScript.MaxNumberOfConnections];

    [SerializeField]
    List<GameObject> _coins;

    public List<GameObject> Coins {
        get { return _coins; }
        set { _coins = value; }
    }

    Dictionary<int, GameObject> traps = new Dictionary<int, GameObject>();

    [SerializeField]
    Material _playerMaterial;

    [SerializeField]
    InputManagerScript _inputManagerScript;

    [SerializeField]
    HelpersScript _helpersScript;

    [SerializeField]
    NetworkView _networkView;

    public NetworkView NetworkView {
        get { return _networkView; }
        set { _networkView = value; }
    }

    float currentGameTimeRemaining;
    float currentTurnTimeRemaining;
    float currentSimulationTimeRemaining;
    float currentIntervalBetweenRPCs;

    // Server
    bool isPlaying = false;

    public bool IsPlaying {
        get { return isPlaying; }
        set { isPlaying = value; }
    }

    // Client
    bool isPlanificating = true;

    public bool IsPlanificating {
        get { return isPlanificating; }
        set { isPlanificating = value; }
    }

    GameObject localPlayerGameObject;

    public GameObject LocalPlayerGameObject {
        get { return localPlayerGameObject; }
        set { localPlayerGameObject = value; }
    }

    [SerializeField]
    GeneralGameVariables _gameVariables;

    public GeneralGameVariables GameVariables {
        get { return _gameVariables; }
        set { _gameVariables = value; }
    }

    [SerializeField]
    GameGUIItems _gameGUIItems;

    public GameGUIItems GameGuiItems {
        get { return _gameGUIItems; }
        set { _gameGUIItems = value; }
    }

    [SerializeField]
    GameOverGUIItems _gameOverGuiItems;

    public GameOverGUIItems GameOverGuiItems {
        get { return _gameOverGuiItems; }
        set { _gameOverGuiItems = value; }
    }

    bool gameOver = false;

    void Start() {
        AudioSource.PlayClipAtPoint(GameVariables.BackgroundMusic, transform.position);
        currentGameManagerScript = this;
        _inputManagerScript.GameManagerScript = this;

        currentGameTimeRemaining = GameVariables.GameDuration;
        currentSimulationTimeRemaining = GameVariables.SimulationDuration;
        currentTurnTimeRemaining = GameVariables.TurnDuration;
        currentIntervalBetweenRPCs = GameVariables.IntervalBetweenRPCs;

        if(NetworkManagerScript.currentNetworkManagerScript.IsServer) {
            NetworkManagerScript.currentNetworkManagerScript.GameManagerScript = this;

            PersistentNetworkPlayersScript.currentPersistentNetworkPlayersScript.displayNetworkPlayers();

            for(int i = 0; i < PersistentNetworkPlayersScript.currentPersistentNetworkPlayersScript.Players.Count; i++) {
                NetworkPlayer tempNetworkPlayer = PersistentNetworkPlayersScript.currentPersistentNetworkPlayersScript.Players[i];
                PlayerScript tempPlayerScript = PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript[i];

                tempPlayerScript.NetworkPlayer = tempNetworkPlayer;
                playerScriptFromNetworkPlayer.Add(tempNetworkPlayer, tempPlayerScript);
                //playerScoreTextFromNetworkPlayer.Add(Utils.NetworkPlayerToFormattedAddress(tempNetworkPlayer), GameOverGuiItems.GameOverGuiPlayerItems[i]);
                //playerScoreTextFromNetworkPlayer[i] = GameOverGuiItems.GameOverGuiPlayerItems[i];
                _networkView.RPC("TellPlayerWhoHeIs", tempNetworkPlayer, i + 1);
                playersWantsToFinishTurn[tempPlayerScript] = false;
            }
        }
    }

    void Update() {
        if(NetworkManagerScript.currentNetworkManagerScript.IsServer) {
            currentGameTimeRemaining -= Time.deltaTime;
            GameGuiItems.TextGameTimeRemaining.text = "Jeu : " + currentGameTimeRemaining.ToString("F0") + "s";

            if(currentGameTimeRemaining < 0 || _coins.Count == 0 && !gameOver) {
                gameOver = true;
                GameOver();
            } else {
                if(isPlaying) {
                    UpdateDuringSimulation();
                } else {
                    UpdateDuringPlanification();
                }
            }
        }
    }

    void UpdateDuringSimulation() {
        currentSimulationTimeRemaining -= Time.deltaTime;
        GameGuiItems.TextTurnOrSimulationTimeRemaining.text = "Simu : " + currentSimulationTimeRemaining.ToString("F1") + "s";

        if(currentSimulationTimeRemaining < 0) {
            isPlaying = false;
            currentSimulationTimeRemaining = GameVariables.SimulationDuration;
            foreach(PlayerScript tempPlayerScript in PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript) {
                tempPlayerScript.StopMove();
                tempPlayerScript.ClearActionsList();
                playersWantsToFinishTurn[tempPlayerScript] = false;
            }
            // Reprend la planif au prochain update
            _networkView.RPC("PlanificationStarted", RPCMode.Others);
        } else {
            foreach(PlayerScript tempPlayerScript in PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript) {
                if(tempPlayerScript.HasMoreActions() && tempPlayerScript.GetCurrentAction() == null) {
                    tempPlayerScript.ExecuteNextAction();
                }
            }
        }

        currentIntervalBetweenRPCs -= Time.deltaTime;
        if(currentIntervalBetweenRPCs < 0) {
            currentIntervalBetweenRPCs = GameVariables.IntervalBetweenRPCs;
            foreach(PlayerScript tempPlayerScript in PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript) {
                if(tempPlayerScript.IsConnected) {
                    _networkView.RPC("UpdatePlayersDuringSimulation", tempPlayerScript.NetworkPlayer, currentGameTimeRemaining, currentSimulationTimeRemaining, tempPlayerScript.GetScore());
                }
            }
        }
    }

    void UpdateDuringPlanification() {
        currentTurnTimeRemaining -= Time.deltaTime;
        GameGuiItems.TextTurnOrSimulationTimeRemaining.text = "Tour : " + currentTurnTimeRemaining.ToString("F1") + " s";

        if(currentTurnTimeRemaining < 0 || finishTurn) {
            isPlaying = true;
            finishTurn = false;
            currentTurnTimeRemaining = GameVariables.TurnDuration;
            // Lance la simu au prochain update
            _networkView.RPC("SimulationStarted", RPCMode.Others);
        }

        currentIntervalBetweenRPCs -= Time.deltaTime;
        if(currentIntervalBetweenRPCs < 0) {
            currentIntervalBetweenRPCs = GameVariables.IntervalBetweenRPCs;
            foreach(PlayerScript playerScript in PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript) {
                if(playerScript.IsConnected) {
                    _networkView.RPC("UpdatePlayersDuringPlanification", playerScript.NetworkPlayer, currentGameTimeRemaining, currentTurnTimeRemaining, playerScript.GetScore());
                }
            }
        }
    }

    public void serverPlayerConnected(NetworkPlayer player) {
        foreach(PlayerScript playerScript in PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript) {
            if(player.ipAddress.Equals(playerScript.NetworkPlayer.ipAddress) && player.port.Equals(playerScript.NetworkPlayer.port)) {
                playerScript.NetworkPlayer = player;
                playerScript.IsConnected = true;
                NetworkManagerScript.currentNetworkManagerScript.NetworkView.RPC("ClientLaunchGameScene", player);
                break;
            }
        }
    }

    public void serverPlayerDisconnected(NetworkPlayer player) {
        foreach(PlayerScript tempPlayerScript in PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript) {
            if(player.ipAddress.Equals(tempPlayerScript.NetworkPlayer.ipAddress) && player.port.Equals(tempPlayerScript.NetworkPlayer.port)) {
                tempPlayerScript.IsConnected = false;
                tempPlayerScript.gameObject.renderer.enabled = false;
            }
        }
    }

    [RPC]
    public void WantsToFinishTurn(NetworkPlayer player) {
        if(!isPlaying && NetworkManagerScript.currentNetworkManagerScript.IsServer) {
            PlayerScript tempPlayerScript = playerScriptFromNetworkPlayer[player];
            playersWantsToFinishTurn[tempPlayerScript] = true;
            finishTurn = true;

            foreach(bool tempPlayerWantsToFinishTurn in playersWantsToFinishTurn.Values) {
                if(!tempPlayerWantsToFinishTurn) {
                    finishTurn = false;
                    break;
                }
            }
        }
    }

    [RPC]
    public void WantsToAddWayPoint(NetworkPlayer player, Vector3 point) {
        if(!isPlaying && NetworkManagerScript.currentNetworkManagerScript.IsServer) {
            PlayerScript currentPlayerScript = playerScriptFromNetworkPlayer[player];

            currentPlayerScript.AddActionInList(new CharacterActionMove(currentPlayerScript, point));
        }
    }

    [RPC]
    public void WantsToCollectCoins(NetworkPlayer player) {
        if(!isPlaying && NetworkManagerScript.currentNetworkManagerScript.IsServer) {
            PlayerScript currentPlayerScript = playerScriptFromNetworkPlayer[player];

            currentPlayerScript.AddActionInList(new CharacterActionCollectCoins(currentPlayerScript));
        }
    }

    [RPC]
    public void WantsToSetTrap(NetworkPlayer player) {
        if(!isPlaying && NetworkManagerScript.currentNetworkManagerScript.IsServer) {
            PlayerScript currentPlayerScript = playerScriptFromNetworkPlayer[player];

            currentPlayerScript.AddActionInList(new CharacterActionSetTrap(currentPlayerScript));
        }
    }

    [RPC]
    public void TrapCreated(int id) {
        GameObject go = (GameObject)GameObject.Instantiate(GameVariables.TrapPrefab, localPlayerGameObject.transform.position, new Quaternion());
        Destroy(go.GetComponent<TrapScript>());
        traps.Add(id, go);
    }

    [RPC]
    public void DestroyTrap(int id) {
        GameObject.Destroy(traps[id]);
    }

    [RPC]
    public void UpdatePlayersDuringPlanification(float newCurrentGameTimeRemaining, float newCurrentTurnTimeRemaining, int newScore) {
        GameGuiItems.TextGameTimeRemaining.text = "Jeu : " + newCurrentGameTimeRemaining.ToString("F0") + "s";
        GameGuiItems.TextTurnOrSimulationTimeRemaining.text = "Tour : " + newCurrentTurnTimeRemaining.ToString("F1") + " s";
        GameGuiItems.TextScore.text = "Score : " + newScore;
    }

    [RPC]
    public void UpdatePlayersDuringSimulation(float newCurrentGameTimeRemaining, float newCurrentSimulationTimeRemaining, int newScore) {
        GameGuiItems.TextGameTimeRemaining.text = "Jeu : " + newCurrentGameTimeRemaining.ToString("F0") + "s";
        GameGuiItems.TextTurnOrSimulationTimeRemaining.text = "Simu : " + newCurrentSimulationTimeRemaining.ToString("F1") + "s";
        GameGuiItems.TextScore.text = "Score : " + newScore;
    }

    [RPC]
    public void SimulationStarted() {
        isPlanificating = false;
        _inputManagerScript.ClickPoint = Vector3.zero;
        _helpersScript.Clear();
    }

    [RPC]
    public void PlanificationStarted() {
        isPlanificating = true;
    }

    [RPC]
    void TellPlayerWhoHeIs(int index) {
        foreach(GameObject go in _playersGameObject) {
            if(go.name.Equals("Player" + index)) {
                go.GetComponent<Renderer>().material = _playerMaterial;
                localPlayerGameObject = go;
                _helpersScript.SetVariables();
            }
        }
    }

    //TODO: arreter les timers, relancer un lobby toussa (recycler le serveur)
    void GameOver() {
        int[] scores = new int[PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript.Count];
        for(int i = 0; i < PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript.Count; i++) {
            scores[i] = PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript[i].Score;
        }
        int maxScore = Mathf.Max(scores);

        foreach(PlayerScript PlayerScriptToSendRPCTo in PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript) {
            _networkView.RPC("ClientLaunchGameOver", PlayerScriptToSendRPCTo.NetworkPlayer, PlayerScriptToSendRPCTo.Score == maxScore);
            int i = 0;
            foreach(PlayerScript PlayerScriptConcerned in PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript) {
                _networkView.RPC("ClientSetScoreForPlayer", PlayerScriptToSendRPCTo.NetworkPlayer, PlayerScriptConcerned.NetworkPlayer, PlayerScriptConcerned.Score, i, PlayerScriptToSendRPCTo.NetworkPlayer);
                i++;
            }
        }
    }

    //Client-only method
    [RPC]
    void ClientSetScoreForPlayer(NetworkPlayer OtherNetworkPlayer, int finalScore, int index, NetworkPlayer CurrentNetworkPlayer) {
        GameOverGUIPlayerItems tempGameOverGUIPlayerItems = GameOverGuiItems.GameOverGuiPlayerItems[index];
        //tempGameOverGUIPlayerItems.PlayerNameLabel.text = Utils.NetworkPlayerToFormattedAddress(networkPlayer);
        tempGameOverGUIPlayerItems.PlayerScore.text = finalScore.ToString();

        Debug.Log(Utils.NetworkPlayerToFormattedAddress(OtherNetworkPlayer));
        Debug.Log(Utils.NetworkPlayerToFormattedAddress(LocalPlayerGameObject.GetComponent<PlayerScript>().NetworkPlayer));

        if(Utils.NetworkPlayerToFormattedAddress(OtherNetworkPlayer).Equals(Utils.NetworkPlayerToFormattedAddress(CurrentNetworkPlayer))) {
            tempGameOverGUIPlayerItems.PlayerNameLabel.color = Color.red;
            tempGameOverGUIPlayerItems.PlayerScore.color = Color.red;
        }
    }

    [RPC]
    void ClientLaunchGameOver(bool winner) {
        GameGuiItems.GameGUICamera.enabled = false;
        GameOverGuiItems.GameOverGUICamera.enabled = true;
        GameOverGuiItems.WinnerOrLooserText.text = winner ? "VICTOIRE !" : "Bouuuuuuuuh !!";

        //TODO: deplacer dans le input manager script ?
        GameOverGuiItems.BackToMainMenuButton.onClick.AddListener(() => {
            //TODO: reinitialisé scene game ? :(
            Network.Disconnect();
            SceneStateManager.currentStateManager.loadLevel(SceneStateManager.sceneState.MainMenu);
        });
    }
}