using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Exp : NetworkBehaviour {
    [SyncVar]
    public int exp = 0;
    [SyncVar]
    public string nickname = "Player";
    int lastExp = 0;
    UserInterface userInterface;
    float lastTime;

    void Start()
    {
        userInterface = GameObject.Find("UserInterface").GetComponent<UserInterface>();
    }

    void Update() {
        if (isServer)
        {
            if (Time.time - lastTime > 1)
            {
                lastTime = Time.time;
                exp += (int)(gameObject.transform.position.y / Map.LevelHeight) + 1;
            }
        }
        if (isLocalPlayer && lastExp != exp)
        {
            lastExp = exp;
            userInterface.exp = exp;
        }
	}
}
