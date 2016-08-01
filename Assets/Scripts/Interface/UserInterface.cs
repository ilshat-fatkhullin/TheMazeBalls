using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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
    public PlayerCell[] playerCells;
    Rect[] nicknamesRect = new Rect[15];
    Rect[] middleNicknamesRect = new Rect[15];
    Rect[] expsRect = new Rect[15];
    Rect[] middleExpsRect = new Rect[15];
    GUIStyle guiStyle = new GUIStyle();
    GUIStyle localPlayerLabelGuiStyle = new GUIStyle();
    Rect expRect, timeRect, hpRect, armorRect, roundIsOverLabel;
    public int hp, armor, exp;
    public List<string> wordsList = new List<string>();
    RoundController roundController;

    void LoadLanguage()
    {
        int language = DataManager.LoadLang();
        LanguageReader languageReader = GameObject.Find("NetworkManager").GetComponent<LanguageReader>();
        wordsList = new List<string>();
        //0
        wordsList.Add(languageReader.langDict[language]["continue"]);
        //1
        wordsList.Add(languageReader.langDict[language]["options"]);
        //2
        wordsList.Add(languageReader.langDict[language]["exitToMainMenu"]);
        //3
        wordsList.Add(languageReader.langDict[language]["drawingQuality"]);
        //4
        wordsList.Add(languageReader.langDict[language]["musicVolume"]);
        //5
        wordsList.Add(languageReader.langDict[language]["effectsVolume"]);
        //6
        wordsList.Add(languageReader.langDict[language]["accept"]);
        //7
        wordsList.Add(languageReader.langDict[language]["back"]);
        //8
        wordsList.Add(languageReader.langDict[language]["low"]);
        //9
        wordsList.Add(languageReader.langDict[language]["middle"]);
        //10
        wordsList.Add(languageReader.langDict[language]["high"]);
        //11
        wordsList.Add(languageReader.langDict[language]["health"]);
        //12
        wordsList.Add(languageReader.langDict[language]["armor"]);
        //13
        wordsList.Add(languageReader.langDict[language]["exp"]);
        //14
        wordsList.Add(languageReader.langDict[language]["color"]);
        //15
        wordsList.Add(languageReader.langDict[language]["mouth"]);
        //16
        wordsList.Add(languageReader.langDict[language]["eyes"]);
        //17
        wordsList.Add(languageReader.langDict[language]["play"]);
        //18
        wordsList.Add(languageReader.langDict[language]["time"]);
        //19
        wordsList.Add(languageReader.langDict[language]["roundIsOver"]);
    }

    void Awake()
    {
        LoadLanguage();
    }

    void Start()
    {
        float pixel = Screen.height / 20;
        expRect = new Rect(pixel, pixel, pixel * 4, pixel);
        timeRect = new Rect(pixel, pixel * 2.5F, pixel * 4, pixel);
        armorRect = new Rect(pixel, Screen.height - pixel * 4, pixel * 4, pixel);
        hpRect = new Rect(pixel, Screen.height - pixel * 2, pixel * 4, pixel);
        guiStyle.fontSize = Convert.ToInt32(pixel);
        guiStyle.normal.textColor = Color.white;
        localPlayerLabelGuiStyle.fontSize = Convert.ToInt32(pixel);
        localPlayerLabelGuiStyle.normal.textColor = Color.green;
        roundController = GameObject.Find("SceneManager").GetComponent<RoundController>();

        roundIsOverLabel = new Rect(Screen.width / 2 - pixel * 7, pixel, pixel * 5, pixel);

        for (int i = 0; i < nicknamesRect.GetLength(0); i++)
        {
            nicknamesRect[i] = new Rect(Screen.width - pixel * 10, pixel + pixel * i, pixel * 5, pixel);
            middleNicknamesRect[i] = new Rect(Screen.width / 2 - pixel * 8, pixel * 2 + pixel * i, pixel * 5, pixel);
            expsRect[i] = new Rect(Screen.width - pixel * 3, pixel + pixel * i, pixel * 3, pixel);
            middleExpsRect[i] = new Rect(Screen.width / 2, pixel * 2 + pixel * i, pixel * 3, pixel);
        }
    }



    void OnGUI()
    {
        if (!roundController.IsRoundEnd)
        {
            GUI.Label(expRect, wordsList[13] + " " + exp, guiStyle);
            GUI.Label(timeRect, wordsList[18] + " " + (roundController.Time / 60) + ":" + (roundController.Time % 60), guiStyle);
            GUI.Label(hpRect, wordsList[11] + " " + hp, guiStyle);
            GUI.Label(armorRect, wordsList[12] + " " + armor, guiStyle);

            if (Tab)
                if (playerCells != null)
                    for (int i = 0; i < playerCells.GetLength(0) && i < nicknamesRect.GetLength(0); i++)
                    {                        
                        if (!playerCells[i].IsLocalPlayer)
                        {
                            GUI.Label(expsRect[i], Convert.ToString(playerCells[i].Exp), guiStyle);
                            GUI.Label(nicknamesRect[i], (i + 1) + "." + playerCells[i].Nickname, guiStyle);
                        }
                        else
                        {
                            GUI.Label(expsRect[i], Convert.ToString(playerCells[i].Exp), localPlayerLabelGuiStyle);
                            GUI.Label(nicknamesRect[i], (i + 1) + "." + playerCells[i].Nickname, localPlayerLabelGuiStyle);
                        }
                    }
        }
        else
        {
            GUI.Label(roundIsOverLabel, wordsList[19], guiStyle);
            if (playerCells != null)
                for (int i = 0; i < playerCells.GetLength(0) && i < nicknamesRect.GetLength(0); i++)
                {                    
                    if (!playerCells[i].IsLocalPlayer)
                    {
                        GUI.Label(middleNicknamesRect[i], (i + 1) + "." + playerCells[i].Nickname, guiStyle);
                        GUI.Label(middleExpsRect[i], Convert.ToString(playerCells[i].Exp), guiStyle);
                    }
                    else
                    {
                        GUI.Label(middleNicknamesRect[i], (i + 1) + "." + playerCells[i].Nickname, localPlayerLabelGuiStyle);
                        GUI.Label(middleExpsRect[i], Convert.ToString(playerCells[i].Exp), localPlayerLabelGuiStyle);
                    }
                }
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
