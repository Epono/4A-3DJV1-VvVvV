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
        StartTimer();
    }

    public void StartTimer() {
        timerSettingTrap = new Timer(actionPoint * 1000);
        timerSettingTrap.Elapsed += OnTimerComplete;
        timerSettingTrap.Start();
    }

    private void OnTimerComplete(System.Object source, ElapsedEventArgs e) {
        timerSettingTrap.Stop();

        //DoStuff

        isFinished = true;
    }
}