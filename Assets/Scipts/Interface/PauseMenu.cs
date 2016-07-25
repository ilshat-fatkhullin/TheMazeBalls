using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour {

    UserInterface userInterface;
    bool pause = false;
    Rect[] buttons = new Rect[9];
    Rect menuBackgroundRect;
    enum MenuStatus { General, Options }
    MenuStatus menuStatus = MenuStatus.General;
    string[] qualitySelection;
    int l_quality;
    float l_musicLevel, l_effectsLevel;
    int quality, crosshair;
    float musicLevel = 1, effectsLevel = 1, sensetive = 0.5F;
    string nickname;
    public Texture menuBackground;
    NetworkManager networkManager;
    public GUIStyle buttonGuiStyle;

    void Start () {
        userInterface = gameObject.GetComponent<UserInterface>();

        float pixel = Screen.height / 10;

        menuBackgroundRect = new Rect(0, 0, pixel * 6, Screen.height);

        for (int i = 0; i < buttons.GetLength(0); i++)
        {
            buttons[i] = new Rect(pixel, pixel * i + pixel, pixel * 4, pixel / 1.5F);
        }

        Settings settingsStruct = DataManager.LoadVars();
        musicLevel = settingsStruct.musicLevel;
        effectsLevel = settingsStruct.effectsLevel;
        quality = settingsStruct.qualityLevel;
        nickname = settingsStruct.nickname;
        crosshair = settingsStruct.crosshair;
        sensetive = settingsStruct.mouseSensetive;
        if (sensetive < 0.5F)
        {
            sensetive = 0.5F;
        }
        l_musicLevel = musicLevel;
        l_effectsLevel = effectsLevel;
        l_quality = quality;
        SetQuality();
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        qualitySelection = new string[] { userInterface.wordsList[8], userInterface.wordsList[9], userInterface.wordsList[10] };
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause = !pause;
            Cursor.visible = pause;
            userInterface.Controllable = !pause;
        }
	}

    void OnGUI()
    {
        if (pause)
        {
            GUI.DrawTexture(menuBackgroundRect, menuBackground);
            switch (menuStatus)
            {
                case MenuStatus.General:
                    if (GUI.Button(buttons[0], userInterface.wordsList[0], buttonGuiStyle))
                    {
                        pause = false;
                        Cursor.visible = false;
                        userInterface.Controllable = true;
                    }
                    if (GUI.Button(buttons[1], userInterface.wordsList[1], buttonGuiStyle))
                        menuStatus = MenuStatus.Options;
                    if (GUI.Button(buttons[2], userInterface.wordsList[2], buttonGuiStyle))
                    {
                        if (gameObject.GetComponent<NetworkIdentity>().isServer)
                        {
                            networkManager.StopHost();
                        }
                        else
                        {
                            networkManager.StopClient();
                        }
                    }
                    break;
                case MenuStatus.Options:
                    GUI.Label(buttons[0], userInterface.wordsList[3]);
                    l_quality = GUI.SelectionGrid(buttons[1], l_quality, qualitySelection, 3, buttonGuiStyle);
                    GUI.Label(buttons[2], userInterface.wordsList[4]);
                    l_musicLevel = GUI.HorizontalSlider(buttons[3], l_musicLevel, 0, 1);
                    GUI.Label(buttons[4], userInterface.wordsList[5]);
                    l_effectsLevel = GUI.HorizontalSlider(buttons[5], l_effectsLevel, 0, 1);
                    if (GUI.Button(buttons[6], userInterface.wordsList[6], buttonGuiStyle))
                    {
                        quality = l_quality;
                        musicLevel = l_musicLevel;
                        effectsLevel = l_effectsLevel;
                        DataManager.SaveVars(musicLevel, effectsLevel, sensetive, quality, crosshair, nickname);
                        menuStatus = MenuStatus.General;
                        SetQuality();
                    }
                    if (GUI.Button(buttons[7], userInterface.wordsList[7], buttonGuiStyle))
                    {
                        l_quality = quality;
                        l_musicLevel = musicLevel;
                        l_effectsLevel = effectsLevel;
                        menuStatus = MenuStatus.General;
                    }
                    break;
            }
        }
    }

    void SetQuality()
    {
        if (quality == 0)
            QualitySettings.SetQualityLevel(0);
        if (quality == 1)
            QualitySettings.SetQualityLevel(2);
        if (quality == 2)
            QualitySettings.SetQualityLevel(5);
        AudioSource[] sources = GameObject.FindObjectsOfType<AudioSource>();
        userInterface.sensetive = sensetive;
        userInterface.crosshairColor = crosshair;
        for (int i = 0; i < sources.GetLength(0); i++)
        {
            sources[i].volume = effectsLevel;
        }
    }
}
