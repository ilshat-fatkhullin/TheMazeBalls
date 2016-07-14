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

    void Start()
    {
        userInterface = GameObject.Find("UserInterface").GetComponent<UserInterface>();
    }

    void Update() {
        if (isLocalPlayer && lastExp != exp)
        {
            lastExp = exp;
            userInterface.exp = exp;
        }
	}
}
