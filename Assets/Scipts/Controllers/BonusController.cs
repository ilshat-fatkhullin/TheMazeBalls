using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BonusController : NetworkBehaviour {

    MapGenerator mapGenerator;

    void Start()
    {
        if (isServer)
        mapGenerator = GameObject.Find("SceneManager").GetComponent<MapGenerator>();
    }

    void Update()
    {
        transform.Rotate(Vector3.forward, -180 * Time.deltaTime);
    }

	void OnTriggerEnter (Collider col) {
        if (isServer)
        {
            if (col.tag == "Player" || col.tag == "AI")
            {
                col.GetComponent<Health>().HP = Health.MAXHP;
                mapGenerator.UpdateBonus(gameObject);
            }
        }
	}
}
