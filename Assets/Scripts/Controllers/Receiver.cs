using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Receiver : MonoBehaviour {

    const float detectionRadius = 100;
    int enemiesLayer = 1<<8;
    float delay = 1, lastUpdateTime = 0;
    AI ai;
    NetworkIdentity networkIdentity;

    void OnTriggerStay(Collider col)
    {
        if (networkIdentity.isServer)
        if (Time.time - lastUpdateTime > delay)
        if (col.tag == "AI" || col.tag == "Player")
        {
            if (!Physics.Linecast(transform.position, col.transform.position, ~enemiesLayer))
            {
                ai.enemie = col.transform;
                ai.lastUpdateTime = Time.time;
            }
        }
    }

    void Start () {
        networkIdentity = gameObject.transform.parent.GetComponent<NetworkIdentity>();
        if (networkIdentity.isServer)
        {
            ai = gameObject.transform.parent.GetComponent<AI>();
        }
	}
}
