using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class FallController : NetworkBehaviour {

    const float ActiveTime = 3;
    Transform puncher;

    int lastLevel = 0;
    int level;

    float activeStartTime;

    public void SetPuncher(Transform newPuncher)
    {
        activeStartTime = Time.time;
        puncher = newPuncher;
    }
	
	// Update is called once per frame
	void Update () {
        if (isServer)
        {
            level = (int)(transform.position.y / Map.LevelHeight);
            if (level != lastLevel && puncher != null)
            {
                lastLevel = level;
                puncher.GetComponent<Exp>().exp += 10;
            }
            if (Time.time - activeStartTime > ActiveTime)
            {
                puncher = null;
            }
        }
	}
}
