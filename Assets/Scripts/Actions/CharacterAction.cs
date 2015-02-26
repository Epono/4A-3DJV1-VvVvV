using UnityEngine;
using System.Collections;

/**
 * Represents an atomic action the player can execute, like moving, collecting coins or setting a trap 
 */
//TODO: virer les getters/setters faits a la main
public abstract class CharacterAction {

    protected int actionPoint;
    protected string actionName;
    protected bool isFinished;

    public string GetActionName() {
        return actionName;
    }

    public int GetActionPoint() {
        return actionPoint;
    }

    public bool IsFinished() {
        return isFinished;
    }

    public virtual void Execute() {

    }

    public virtual void CheckIfFinished() {

    }
}