using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TrapScript : MonoBehaviour {

    [SerializeField]
    AudioClip _trapSetClip;

    public AudioClip TrapSetClip {
        get { return _trapSetClip; }
        set { _trapSetClip = value; }
    }

    [SerializeField]
    AudioClip _trapTriggerClip;

    public AudioClip TrapTriggerClip {
        get { return _trapTriggerClip; }
        set { _trapTriggerClip = value; }
    }

    [SerializeField]
    PlayerScript _owner;

    public PlayerScript Owner {
        get { return _owner; }
        set { _owner = value; }
    }

    void OnTriggerEnter(Collider other) {
        PlayerScript p = other.GetComponent<PlayerScript>();
        if(!Utils.NetworkPlayerToFormattedAddress(p.NetworkPlayer).Equals(Utils.NetworkPlayerToFormattedAddress(_owner.NetworkPlayer))) {
            AudioSource.PlayClipAtPoint(_trapTriggerClip, transform.position);
            GameManagerScript.currentGameManagerScript.NetworkView.RPC("DestroyTrap", _owner.NetworkPlayer, this.gameObject.GetInstanceID());
            GameObject.Destroy(this.gameObject);

            p.DecreaseScoreTrap();
        }
    }
}
