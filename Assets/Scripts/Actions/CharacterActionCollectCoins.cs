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
        List<GameObject> coinsToDelete = new List<GameObject>();
        foreach(GameObject coin in GameManagerScript.currentGameManagerScript.Coins) {
            if(Vector3.Distance(coin.transform.position, playerScript.gameObject.transform.position) < 20) {
                //TODO: customizable distance, beware of walls ! 
                coinsToDelete.Add(coin);
                playerScript.IncrementScore();
            }
        }

        foreach(GameObject coin in coinsToDelete) {
            GameManagerScript.currentGameManagerScript.Coins.Remove(coin);
            Object.Destroy(coin);
        }

        //TODO: preferably, coroutine, but can't make it work
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