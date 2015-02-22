using UnityEngine;
using System.Collections;

public class CoinScript : MonoBehaviour {

    public float speed = 10f;

    void Update() {
        transform.Rotate(Vector3.down, speed * Time.deltaTime);
        transform.Rotate(Vector3.left, speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other) {
        GameManagerScript.currentGameManagerScript.Coins.Remove(this.gameObject);
        Object.Destroy(this.gameObject);
        other.gameObject.GetComponent<PlayerScript>().IncrementScore();
    }
}
