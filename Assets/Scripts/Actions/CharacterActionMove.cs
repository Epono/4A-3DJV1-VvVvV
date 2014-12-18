using UnityEngine;
using System.Collections;

public class CharacterActionMove : CharacterAction {

    public Vector3 location;

	
    public CharacterActionMove(Vector3 locationChoose)
    {
        actionPoint = 1;
        actionName = "MoveToLocation";
        location = locationChoose;
    }

   // public override void Execute()
   // {
   //     _agent.SetDestination(getLocation());
   // }
    
    public override Vector3 getLocation()
    {
        return location;
    }

    void MoveToLocation()
    {
       
    }
}
