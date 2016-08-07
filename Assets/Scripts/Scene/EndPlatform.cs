using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EndPlatform : NetworkBehaviour {

    Spawner spawner;
    MapGenerator generator;

	void OnTriggerEnter (Collider col) {
        if (isServer)
            if (col.tag == "Player" || col.tag == "AI")
            {
                col.GetComponent<Health>().startTime = Time.time;
                if (col.tag == "Player")
                {
                    col.GetComponent<SynchronizeManager>().UpdatePosition(new Vector3(col.transform.position.x, col.transform.position.y + Map.LevelHeight, col.transform.position.z), false);
                }
                else
                {
                    col.gameObject.transform.position = new Vector3(col.transform.position.x, col.transform.position.y + Map.LevelHeight, col.transform.position.z);
                }
            }
	}
	
	void Start () {
        spawner = GameObject.Find("SceneManager").GetComponent<Spawner>();
        generator = GameObject.Find("SceneManager").GetComponent<MapGenerator>();
        transform.rotation = Quaternion.Euler(90, -180, 0);
    }
}
