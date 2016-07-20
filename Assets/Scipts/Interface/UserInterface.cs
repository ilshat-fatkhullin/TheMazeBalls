using UnityEngine;
using System.Collections;
using System;

public class UserInterface : MonoBehaviour {

    public float sensetive = 0.5F;
    public int crosshairColor = 0;

    public float MouseX, MouseY, Horizontal, Vertical, Fire, Block;
    public int Number = 1;
    public bool Jump;
    public bool Num;
    public bool Tab;
    public bool Flashlight;
    public bool Controllable = true;
    public Exp[] exps;
    Rect[] nicknamesRect = new Rect[8];
    Rect[] expsRect = new Rect[8];
    GUIStyle guiStyle = new GUIStyle();
    Rect expRect, hpRect, armorRect;
    public int hp, armor, exp;

    void Start()
    {
        float pixel = Screen.height / 20;
        expRect = new Rect(pixel, pixel, pixel * 4, pixel);
        armorRect = new Rect(pixel, Screen.height - pixel * 4, pixel * 4, pixel);
        hpRect = new Rect(pixel, Screen.height - pixel * 2, pixel * 4, pixel);
        guiStyle.fontSize = Convert.ToInt32(pixel);
        guiStyle.normal.textColor = Color.white;

        for (int i = 0; i < nicknamesRect.GetLength(0); i++)
        {
            nicknamesRect[i] = new Rect(Screen.width - pixel * 8, pixel + pixel * i, pixel * 5, pixel);
            expsRect[i] = new Rect(Screen.width - pixel * 3, pixel + pixel * i, pixel * 3, pixel);
        }
    }

    void OnGUI()
    {
        GUI.Label(expRect, "EXP: " + exp, guiStyle);
        GUI.Label(hpRect, "HP: " + hp, guiStyle);
        GUI.Label(armorRect, "ARMOR: " + armor, guiStyle);

        if (Tab)
        if (exps != null)
            for (int i = 0; i < exps.GetLength(0) && i < nicknamesRect.GetLength(0); i++)
            {
                GUI.Label(nicknamesRect[i], exps[i].nickname, guiStyle);
                GUI.Label(expsRect[i], Convert.ToString(exps[i].exp), guiStyle);
            }
    }

    void Update () {
        Num = false;
        if (Controllable)
        {
            MouseX = Input.GetAxis("Mouse X") * (sensetive + 0.25F);
            MouseY = Input.GetAxis("Mouse Y") * (sensetive + 0.25F);
            Horizontal = Input.GetAxis("LeftRight");
            Vertical = Input.GetAxis("ForwardBack");
            Fire = Input.GetAxis("Fire");
            Block = Input.GetAxis("Block");
            Jump = Input.GetAxis("Jump") > 0;
            Tab = Input.GetAxis("Tab") > 0;
            Flashlight = Input.GetButtonDown("Flashlight");
            if (Input.GetKey(KeyCode.Alpha1))
            {
                Num = true;
                Number = 1;
            }
            if (Input.GetKey(KeyCode.Alpha2))
            {
                Num = true;
                Number = 2;
            }
            if (Input.GetKey(KeyCode.Alpha3))
            {
                Num = true;
                Number = 3;
            }
            if (Input.GetKey(KeyCode.Alpha4))
            {
                Num = true;
                Number = 4;
            }
            if (Input.GetKey(KeyCode.Alpha5))
            {
                Num = true;
                Number = 5;
            }
            if (Input.GetKey(KeyCode.Alpha6))
            {
                Num = true;
                Number = 6;
            }
            if (Input.GetKey(KeyCode.Alpha7))
            {
                Num = true;
                Number = 7;
            }
            if (Input.GetKey(KeyCode.Alpha8))
            {
                Num = true;
                Number = 8;
            }
            if (Input.GetKey(KeyCode.Alpha9))
            {
                Num = true;
                Number = 9;
            }
        }
        else
        {
            MouseX = 0;
            MouseY = 0;
            Horizontal = 0;
            Vertical = 0;
            Fire = 0;
            Block = 0;
            Jump = false;
            Tab = false;
        }
    }
}
