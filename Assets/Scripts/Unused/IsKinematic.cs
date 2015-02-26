using UnityEngine;
using System.Collections;

public class IsKinematic : MonoBehaviour {

    [SerializeField]
    private Rigidbody TargetRigibody;



    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("Un intrus !");
        TargetRigibody.isKinematic = false;
        TargetRigibody.AddForce(Vector3.down);
    }

    public void OnDrawGizmos() {
        //Debug.DrawLine(this.transform.position, this.transform.position + _force / Physics.gravity.magnitude, Color.red);
        Debug.DrawLine(this.transform.position, TargetRigibody.transform.position, Color.yellow);
    }
}