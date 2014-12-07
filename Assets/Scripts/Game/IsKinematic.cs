using UnityEngine;
using System.Collections;

public class IsKinematic : MonoBehaviour
{
    [SerializeField]
    private Rigidbody TargetRigibody;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKey(KeyCode.A))
        {
            TargetRigibody.isKinematic = false;
            TargetRigibody.AddForce(Vector3.down);
        }
        */
    }

    void OnTriggerEnter(Collider other)
    {
        //if (other is Player)
        // {
        Debug.Log("Intruder !!!!");
        TargetRigibody.isKinematic = false;
        TargetRigibody.useGravity = true;
        TargetRigibody.AddForce(Vector3.down);
        // }
    }

    public void OnDrawGizmos()
    {
        //Debug.DrawLine(this.transform.position, this.transform.position + _force / Physics.gravity.magnitude, Color.red);
        Debug.DrawLine(this.transform.position, TargetRigibody.transform.position, Color.yellow);
    }
}