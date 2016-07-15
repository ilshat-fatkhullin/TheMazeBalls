using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Exp : NetworkBehaviour {
    public int exp = 0;
    public string nickname = "Player";
    int lastExp = 0;
    Rect expRect;
    GUIStyle guiStyle = new GUIStyle();

	void Start () {
        if (isLocalPlayer)
        {
            float pixel = Screen.height / 20;
            expRect = new Rect(pixel, pixel, pixel * 4, pixel);
            guiStyle.fontSize = Convert.ToInt32(pixel);
            guiStyle.normal.textColor = Color.white;
        }
    }

    void Update()
    {
        if (isServer)
        {
            if (lastExp != exp)
            {
                lastExp = exp;
                RpcUpdateExp(exp);
            }
        }
    }

    [ClientRpc]
    void RpcUpdateExp(int newExp)
    {
        exp = newExp;
    }

    [ClientRpc]
    void RpcUpdateNickname(string newNickname)
    {
        nickname = newNickname;
    }

    void OnGUI () {
        if (isLocalPlayer)
        GUI.Label(expRect, "Exp: " + exp, guiStyle);
	}
}
