using UnityEngine;
using System.Collections;

public class CoinScript : MonoBehaviour {

    [SerializeField]
    float _coinRotationSpeed = 10f;

    [SerializeField]
    AudioClip _collectingCoinClip;

    void Update() {
        transform.Rotate(Vector3.down, _coinRotationSpeed * Time.deltaTime);
        transform.Rotate(Vector3.left, _coinRotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other) {
        CollectCoin(other.GetComponent<PlayerScript>());
    }

    public void CollectCoin(PlayerScript playerScript) {
        AudioSource.PlayClipAtPoint(_collectingCoinClip, transform.position);
        GameManagerScript.currentGameManagerScript.Coins.Remove(this.gameObject);
        playerScript.IncrementScore();
        Network.Destroy(this.gameObject);
    }
}
