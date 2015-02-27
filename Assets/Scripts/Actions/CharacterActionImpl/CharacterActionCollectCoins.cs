using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System.Timers;

public class CharacterActionCollectCoins : CharacterAction {

    PlayerScript playerScript;
    Timer timerCollectingCoins;

    public CharacterActionCollectCoins(PlayerScript playerScript) {
        actionPoint = 1;
        actionName = "CollectCoins";
        this.playerScript = playerScript;
    }

    public override void Execute() {
        //TODO: recolter APRES le timer bug :(
        for(int i = 0; i < GameManagerScript.currentGameManagerScript.Coins.Count; i++) {
            GameObject coin = GameManagerScript.currentGameManagerScript.Coins[i];
            if(Vector3.Distance(coin.transform.position, playerScript.gameObject.transform.position) < GameManagerScript.currentGameManagerScript.GameVariables.CoinSelectionRadius) {
                CoinScript coinScript = coin.GetComponent<CoinScript>();
                coinScript.CollectCoin(playerScript);
                i--;
            }
        }

        StartTimer();
    }

    public void StartTimer() {
        timerCollectingCoins = new Timer(actionPoint * 1000);
        timerCollectingCoins.Elapsed += OnTimerComplete;
        timerCollectingCoins.Start();
    }

    private void OnTimerComplete(System.Object source, ElapsedEventArgs e) {
        timerCollectingCoins.Stop();
        isFinished = true;
    }
}