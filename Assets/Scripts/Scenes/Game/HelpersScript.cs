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

        LineRenderer ligne = container.AddComponent<LineRenderer>();
        ligne.SetColors(Color.black, Color.black);
        ligne.SetWidth(0.5f, 0.5f);
        ligne.enabled = true;
        ligne.transform.localScale = new Vector3(3, 2, 3);

        NavMeshPath path = new NavMeshPath();

        NavMesh.CalculatePath(from, to, -1, path);

        if(path.status == NavMeshPathStatus.PathComplete) {
            ligne.SetVertexCount(path.corners.Length);
            for(int i = 0; i < path.corners.Length; i++) {
                ligne.SetPosition(i, path.corners[i]);
            }
        }

        GameObject container2 = new GameObject();
        container2.transform.position = from;
        container2.transform.parent = currentPlayerGameObject.transform;

        SpriteRenderer s = container.AddComponent<SpriteRenderer>();
        s.sprite = targetSprite;
        s.transform.position = to;
        s.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        s.transform.localEulerAngles = new Vector3(90, 0, 0);
        s.enabled = true;

        paths.Add(ligne);
        targets.Add(s);
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

    public void Clear() {
        foreach(LineRenderer t in paths) {
            Object.Destroy(t.gameObject);
        }

        foreach(SpriteRenderer t in targets) {
            Object.Destroy(t.gameObject);
        }
        //TODO: mieux gerer la suppression

        waypoints = new List<Vector3>();
        paths = new List<LineRenderer>();
        targets = new List<SpriteRenderer>();
    }
}