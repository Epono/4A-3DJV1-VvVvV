using UnityEngine;
using System.Collections;

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