using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HelpersScript : MonoBehaviour {

    GameObject currentPlayerGameObject;

    GameManagerScript gameManagerScript;
    InputManagerScript inputManagerScript;

    SpriteRenderer targetRenderer;
    LineRenderer currentLineRenderer;

    List<Vector3> waypoints = new List<Vector3>();
    List<LineRenderer> paths = new List<LineRenderer>();
    List<SpriteRenderer> targets = new List<SpriteRenderer>();
    List<LineRenderer> collectCoinsSpots = new List<LineRenderer>();
    List<LineRenderer> traps = new List<LineRenderer>();

    [SerializeField]
    Sprite targetSprite;

    void Start() {
        gameManagerScript = GameManagerScript.currentGameManagerScript;
        inputManagerScript = InputManagerScript.currentInputManagerScript;
    }

    public void SetVariables() {
        targetRenderer = gameManagerScript.LocalPlayerGameObject.GetComponentInChildren<SpriteRenderer>();
        currentLineRenderer = gameManagerScript.LocalPlayerGameObject.GetComponent<LineRenderer>();
        currentPlayerGameObject = gameManagerScript.LocalPlayerGameObject;
    }

    void Update() {
        if(!gameManagerScript.IsPlanificating && !NetworkManagerScript.currentNetworkManagerScript.IsServer) {
            targetRenderer.enabled = false;
            currentLineRenderer.enabled = false;
        }
    }

    public void ClickPointChanged() {
        if(inputManagerScript.ClickPoint != Vector3.zero) {
            RenderClickPointAndTempPath();
        } else {
            targetRenderer.enabled = false;
            currentLineRenderer.enabled = false;
        }
    }

    public void RenderClickPointAndTempPath() {
        targetRenderer.transform.position = inputManagerScript.ClickPoint;
        targetRenderer.enabled = true;

        if(waypoints.Count == 0) {
            // if there are no waypoints, draw from player's position
            RenderPathFromToTemp(currentPlayerGameObject.transform.position, inputManagerScript.ClickPoint);
        } else {
            // else, draws from last saved waypoint to current
            RenderPathFromToTemp(waypoints[waypoints.Count - 1], inputManagerScript.ClickPoint);
        }
    }

    void RenderPathFromToTemp(Vector3 from, Vector3 to) {
        LineRenderer ligne = currentLineRenderer;
        ligne.SetColors(Color.black, Color.black);
        ligne.SetWidth(1f, 1f);
        ligne.enabled = true;

        NavMeshPath path = new NavMeshPath();

        NavMesh.CalculatePath(from, to, -1, path);

        if(path.status == NavMeshPathStatus.PathComplete) {
            ligne.SetVertexCount(path.corners.Length);
            for(int i = 0; i < path.corners.Length; i++) {
                ligne.SetPosition(i, path.corners[i]);
            }
        }

        SpriteRenderer s = targetRenderer;
        s.sprite = targetSprite;
        s.transform.position = to;
        s.transform.localScale = new Vector3(3, 3, 3);
        s.transform.localEulerAngles = new Vector3(90, 0, 0);
        s.enabled = true;
    }

    public void RenderPathFromTo(Vector3 from, Vector3 to) {
        GameObject container = new GameObject();
        container.transform.position = from;
        container.transform.parent = currentPlayerGameObject.transform;

        LineRenderer tempWaypointLineRenderer = container.AddComponent<LineRenderer>();
        tempWaypointLineRenderer.SetColors(Color.black, Color.black);
        tempWaypointLineRenderer.SetWidth(0.5f, 0.5f);
        tempWaypointLineRenderer.enabled = true;
        tempWaypointLineRenderer.transform.localScale = new Vector3(3, 2, 3);
        tempWaypointLineRenderer.material = GameManagerScript.currentGameManagerScript.GameVariables.PathMaterial;

        NavMeshPath path = new NavMeshPath();

        NavMesh.CalculatePath(from, to, -1, path);

        if(path.status == NavMeshPathStatus.PathComplete) {
            tempWaypointLineRenderer.SetVertexCount(path.corners.Length);
            for(int i = 0; i < path.corners.Length; i++) {
                tempWaypointLineRenderer.SetPosition(i, path.corners[i]);
            }
        }

        SpriteRenderer tempWaypointSpriteRenderer = container.AddComponent<SpriteRenderer>();
        tempWaypointSpriteRenderer.sprite = targetSprite;
        tempWaypointSpriteRenderer.transform.position = to;
        tempWaypointSpriteRenderer.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        tempWaypointSpriteRenderer.transform.localEulerAngles = new Vector3(90, 0, 0);
        tempWaypointSpriteRenderer.enabled = true;


        paths.Add(tempWaypointLineRenderer);
        targets.Add(tempWaypointSpriteRenderer);
        waypoints.Add(to);
    }

    public void AddWaypoint(Vector3 newWaypoint) {
        targetRenderer.enabled = false;
        currentLineRenderer.enabled = false;
        if(waypoints.Count == 0) {
            RenderPathFromTo(currentPlayerGameObject.transform.position, newWaypoint);
        } else {
            RenderPathFromTo(waypoints[waypoints.Count - 1], newWaypoint);
        }
    }

    public void AddCollectCoins() {
        Vector3 center;
        float radius = GameManagerScript.currentGameManagerScript.GameVariables.CoinSelectionRadius;
        if(waypoints.Count == 0) {
            center = currentPlayerGameObject.transform.position;
        } else {
            center = waypoints[waypoints.Count - 1];
        }

        float PI = 3.14f;
        float theta_scale = 0.1f;                                   //Set lower to add more points
        int size = Mathf.RoundToInt((2.0f * PI) / theta_scale);     //Total number of points in circle.

        GameObject container2 = new GameObject();
        container2.transform.position = center;
        container2.transform.parent = currentPlayerGameObject.transform;

        LineRenderer tempCollectingCoinSpotLineRenderer = container2.AddComponent<LineRenderer>();
        tempCollectingCoinSpotLineRenderer.SetWidth(1, 1);
        tempCollectingCoinSpotLineRenderer.SetVertexCount(size + 1);
        tempCollectingCoinSpotLineRenderer.useWorldSpace = false;
        tempCollectingCoinSpotLineRenderer.SetColors(Color.yellow, Color.yellow);
        tempCollectingCoinSpotLineRenderer.transform.position = center;
        tempCollectingCoinSpotLineRenderer.material = GameManagerScript.currentGameManagerScript.GameVariables.CoinSelectionMateriel;

        int i = 0;
        for(float theta = 0; theta < 2 * PI; theta += 0.1f) {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);

            Vector3 pos = new Vector3(x, 1, z);
            tempCollectingCoinSpotLineRenderer.SetPosition(i, pos);
            i += 1;
        }
        tempCollectingCoinSpotLineRenderer.SetPosition(i, new Vector3(radius * Mathf.Cos(0), 0, radius * Mathf.Sin(0)));

        collectCoinsSpots.Add(tempCollectingCoinSpotLineRenderer);
    }

    public void AddTrap() {
        Vector3 center;
        float radius = 2.0f;
        if(waypoints.Count == 0) {
            center = currentPlayerGameObject.transform.position;
        } else {
            center = waypoints[waypoints.Count - 1];
        }

        float PI = 3.14f;
        float theta_scale = 0.1f;                                   //Set lower to add more points
        int size = Mathf.RoundToInt((2.0f * PI) / theta_scale);     //Total number of points in circle.

        GameObject container2 = new GameObject();
        container2.transform.position = center;
        container2.transform.parent = currentPlayerGameObject.transform;

        LineRenderer tempTrapSetLineRenderer = container2.AddComponent<LineRenderer>();
        tempTrapSetLineRenderer.SetWidth(1, 1);
        tempTrapSetLineRenderer.SetVertexCount(size + 1);
        tempTrapSetLineRenderer.useWorldSpace = false;
        tempTrapSetLineRenderer.SetColors(Color.yellow, Color.yellow);
        tempTrapSetLineRenderer.transform.position = center;
        tempTrapSetLineRenderer.material = GameManagerScript.currentGameManagerScript.GameVariables.TrapRadiusMaterial;

        int i = 0;
        for(float theta = 0; theta < 2 * PI; theta += 0.1f) {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);

            Vector3 pos = new Vector3(x, 1, z);
            tempTrapSetLineRenderer.SetPosition(i, pos);
            i += 1;
        }
        tempTrapSetLineRenderer.SetPosition(i, new Vector3(radius * Mathf.Cos(0), 0, radius * Mathf.Sin(0)));

        traps.Add(tempTrapSetLineRenderer);
    }

    public void Clear() {
        foreach(LineRenderer l in paths) {
            Object.Destroy(l.gameObject);
        }

        foreach(LineRenderer l in collectCoinsSpots) {
            Object.Destroy(l.gameObject);
        }

        foreach(LineRenderer l in traps) {
            Object.Destroy(l.gameObject);
        }
        //TODO: mieux gerer la suppression ?

        waypoints = new List<Vector3>();
        paths = new List<LineRenderer>();
        targets = new List<SpriteRenderer>();
        collectCoinsSpots = new List<LineRenderer>();
        traps = new List<LineRenderer>();
    }
}