using UnityEngine;
using System.Collections;


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
	
}
