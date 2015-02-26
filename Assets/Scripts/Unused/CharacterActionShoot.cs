using UnityEngine;
using System.Collections;

// TODO
public class CharacterActionShoot : CharacterAction {

    //Pour désigner une target soit il faut passer un NetworkPlayer soit un Int (a determiner)
    // int actionTarget = -1;
    // int actionRange = 5;

    // Use this for initialization
    void Start() {
        this.actionPoint = 2;
        this.actionName = "ShootDrugs";
        //  this.actionRange = 5;
    }

    // Update is called once per frame
    public override void CheckIfFinished() {

    }

    void ShootInLine() {

    }

    void SelectTarget(int target) {
        //      this.actionTarget = target;
    }
}
