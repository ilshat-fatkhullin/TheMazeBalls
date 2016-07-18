using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Distributor : NetworkBehaviour {

    const float detectionRadius = 100;
    int enemiesLayer = 1<<8;
    float delay = 1, lastUpdateTime = 0;
	
	void Update () {
        if (isServer)
        {
            if (Time.time - lastUpdateTime > delay)
            {
                lastUpdateTime = Time.time;

                Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemiesLayer);

                foreach (Collider e in enemies)
                {
                    if (e.tag == "AI")
                    {
                        if (!Physics.Linecast(transform.position, e.transform.position, ~enemiesLayer))
                        {
                            e.GetComponent<AI>().enemie = transform;
                            e.GetComponent<AI>().lastUpdateTime = Time.time;
                        }
                    }
                }
            }
        }
	}
}
