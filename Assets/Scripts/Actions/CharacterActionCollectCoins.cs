using UnityEngine;
using System.Collections;

public class CharacterActionCollectCoins : CharacterAction {

    public CharacterActionCollectCoins() {
        actionPoint = 3;
        actionName = "CollectCoins";
    }

    public override void Execute() {
        // Execute
        isFinished = true;
    }
}