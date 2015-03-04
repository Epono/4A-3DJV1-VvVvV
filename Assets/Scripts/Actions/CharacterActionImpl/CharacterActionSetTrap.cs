using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System.Timers;

public class CharacterActionSetTrap : CharacterAction {
    PlayerScript playerScript;
    Timer timerSettingTrap;

    public CharacterActionSetTrap(PlayerScript playerScript) {
        actionPoint = 1;
        actionName = "SetTrap";
        this.playerScript = playerScript;
    }

    public override void Execute() {
        GameObject o = (GameObject)GameObject.Instantiate(GameManagerScript.currentGameManagerScript.GameVariables.TrapPrefab, playerScript.PlayerGameObject.transform.position, new Quaternion());
        TrapScript t = o.GetComponent<TrapScript>();

        t.Owner = playerScript;

        AudioSource.PlayClipAtPoint(t.TrapSetClip, playerScript.PlayerGameObject.transform.position);

        GameManagerScript.currentGameManagerScript.NetworkView.RPC("TrapCreated", t.Owner.NetworkPlayer, o.GetInstanceID());

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