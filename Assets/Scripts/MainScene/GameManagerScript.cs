using UnityEngine;
using System.Collections;

public class GameManagerScript : MonoBehaviour {

	[SerializeField]
    private APlayerScript[] _playersScript;


//	[SerializeField]
//	bool _buildingServer = false;

//	[SerializeField]
//	NetworkView _networkView;

	// Use this for initialization
	void Start () 
	{
      
	}
	
	// Update is called once per frame
	void Update () {
	}
   

//	public void WantToShoot(int player)
//	{
//		_playersScript [player].TryToShoot();
//	}

    public void WantToMove(int player, Vector3 pos)
    {
        _playersScript[player].TryToMove(pos);
    }
}
