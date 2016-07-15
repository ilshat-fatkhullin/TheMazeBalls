using UnityEngine;
using System.Collections;
using System;

public class PlayersTab : MonoBehaviour {

    UserInterface userInterface;
    Rect[] playersRect = new Rect[8];
    Exp[] exps;

	void Start () {
        userInterface = gameObject.GetComponent<UserInterface>();
        float pixel = Screen.height / 10;
        for (int i = 0; i < playersRect.GetLength(0); i++)
        {
            playersRect[i] = new Rect(Screen.width / 2 - pixel * 2, pixel + pixel * i, pixel * 4, pixel);
        }
	}
	
	void Update () {
        if (userInterface.Tab)
        {
            exps = GameObject.FindObjectsOfType<Exp>();
        }
	}

    void OnGUI()
    {
        if (userInterface.Tab)
        {
            if (exps != null)
            for (int i = 0; i < exps.GetLength(0) && i < playersRect.GetLength(0); i++)
            {
                GUI.Label(playersRect[i], exps[i].nickname + "  " + Convert.ToString(exps[i].exp));
            }
        }
    }
}
