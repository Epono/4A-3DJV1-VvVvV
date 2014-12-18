using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript
{

    [SerializeField]
    NavMeshAgent _agent;

    [SerializeField]
    Transform _transform;

    [SerializeField]
    int _actionPoints;

    [SerializeField]
    ArrayList actionList;

    void Start()
    {
        actionList = new ArrayList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {

    }

    public void ExecuteAction(string actionName)
    {

    }
}
