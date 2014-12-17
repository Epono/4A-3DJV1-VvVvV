using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameManagerScript : MonoBehaviour
{

    //Singletonisation
    public static GameManagerScript currentGameManagerScript;

    [SerializeField]
    SmartActionManagerScript _smartActionManager;

    [SerializeField]
    private APlayerScript[] _playersScript;

    //Liste qui contiendras les actions à executer a chaque tour
    List<string> _listActionName = new List<string>();

    //test avec un dictionnaire
    Dictionary<string, Object> _dicoActionName = new Dictionary<string, Object>();

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

    //J'EN SUIS LA
    public void ExecuteTurnAction(Vector3 pos)
    {
        Debug.Log("On rentre dans ExecuteTurnAction()");
       // for (int i = 0; i < 3; i++ )
        //{
        Debug.Log(_listActionName[0]);
        ExecuteActionOfPlayer(0, _listActionName, pos);
        //}
        
           
    }

    //Liste d'action du tour d'un joueur
    public void AddActionInActionList(string actionName)
    {
        _listActionName.Add(actionName);
        Debug.Log("L'action a bien été ajouté a la liste");
    }


    public void ExecuteActionOfPlayer(int player, List<string> actionNameList, Vector3 pos)
    {
        Debug.Log("On rentre dans ExecuteActionOfPlayer()");
        for (int i = 0; i < actionNameList.Count; i++)
        {
            _playersScript[player].ExecuteAction(actionNameList[i], pos);
        }
    }
}
