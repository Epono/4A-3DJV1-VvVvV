using UnityEngine;
using System.Collections;

public abstract class CharacterAction  {

    protected int actionPoint;
    protected string actionName;
    //protected int actionRange;

    public virtual Vector3 getLocation()
    {
        return new Vector3(0,0,0); 
    }
    
    public string GetActionName()
    {
        return actionName;
    }

    public int GetActionPoint()
    {
        return actionPoint;
    }

    public virtual void Execute()
    {

    }


    //void initiateActionPoint(int actionLvlSwitch)
    //{
    //    switch (actionLvlSwitch)
    //    {
    //        case 1:
    //            this.actionPoint = 1;
    //            break;
    //        case 2:
    //            this.actionPoint = 2;
    //            break;
    //        default:
    //            this.actionPoint = 3;
    //            break;
    //    }
    //}
}
