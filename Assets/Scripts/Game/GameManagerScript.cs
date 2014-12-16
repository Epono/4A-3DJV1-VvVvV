using UnityEngine;
using System.Collections;

public class GameManagerScript : MonoBehaviour
{

    //Singletonisation
    public static GameManagerScript currentGameManagerScript;

    [SerializeField]
    private APlayerScript[] _playersScript;

    void Awake()
    {
        if (currentGameManagerScript == null)
        {
            DontDestroyOnLoad(gameObject);
            currentGameManagerScript = this;
        }
        else if (currentGameManagerScript != null)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        PersistentPlayersScript.currentPersistentPlayersScript.displayNetworkPlayers();
    }

    // Update is called once per frame
    void Update()
    {

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
