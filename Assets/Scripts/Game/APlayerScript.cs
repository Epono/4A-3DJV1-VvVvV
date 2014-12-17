using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class APlayerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//void IPlayer.

	abstract public  void TryToMove (Vector3 pos);


	abstract public  void TryToShoot ();

    abstract public void ExecuteAction(string actionName,Vector3 pos);
	
}
