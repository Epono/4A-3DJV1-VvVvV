using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System.Timers;

//TODO
public class CharacterActionSetTrap : CharacterAction {

    PlayerScript playerScript;
    Timer timerSettingTrap;

    public CharacterActionSetTrap(PlayerScript playerScript) {
        actionPoint = 1;
        actionName = "SetTrap";
        this.playerScript = playerScript;
    }

    public override void Execute() {
        GameObject o = (GameObject)Network.Instantiate(GameManagerScript.currentGameManagerScript.GameVariables.TrapPrefab, playerScript.PlayerGameObject.transform.position, new Quaternion(), 0);
        TrapScript t = o.AddComponent<TrapScript>();

        t.Owner = playerScript;
        //t.TrapSetClip = ;
        //t.TrapTriggerClip = ;

        foreach(PlayerScript p in PersistentPlayersScriptScript.currentPersistentPlayersScriptScript.PlayersScript) {
            GameManagerScript.currentGameManagerScript.NetworkView.RPC("trapSet", p.NetworkPlayer, p == playerScript, o.networkView.viewID);
        }

        StartTimer();
    }

    public void StartTimer() {
        timerSettingTrap = new Timer(actionPoint * 1000);
        timerSettingTrap.Elapsed += OnTimerComplete;
        timerSettingTrap.Start();
    }

    private void OnTimerComplete(System.Object source, ElapsedEventArgs e) {
        timerSettingTrap.Stop();
        isFinished = true;
    }
}