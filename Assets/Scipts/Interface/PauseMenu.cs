using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.Networking;

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

    void Start () {
        userInterface = gameObject.GetComponent<UserInterface>();

        float pixel = Screen.height / 10;

        menuBackgroundRect = new Rect(0, 0, pixel * 6, Screen.height);

        for (int i = 0; i < buttons.GetLength(0); i++)
        {
            buttons[i] = new Rect(pixel, pixel * i + pixel, pixel * 4, pixel / 1.2F);
        }

        qualitySelection = new string[] { "Низкое", "Среднее", "Высокое" };

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
                    if (GUI.Button(buttons[0], "Продолжить"))
                    {
                        pause = false;
                        Cursor.visible = false;
                        userInterface.Controllable = true;
                    }
                    if (GUI.Button(buttons[1], "Настройки"))
                        menuStatus = MenuStatus.Options;
                    if (GUI.Button(buttons[2], "Выйти в главное меню"))
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
                    GUI.Label(buttons[0], "Качество графики:");
                    l_quality = GUI.SelectionGrid(buttons[1], l_quality, qualitySelection, 3);
                    GUI.Label(buttons[2], "Громкость музыки:");
                    l_musicLevel = GUI.HorizontalSlider(buttons[3], l_musicLevel, 0, 1);
                    GUI.Label(buttons[4], "Громкость эффектов:");
                    l_effectsLevel = GUI.HorizontalSlider(buttons[5], l_effectsLevel, 0, 1);
                    if (GUI.Button(buttons[6], "Принять"))
                    {
                        quality = l_quality;
                        musicLevel = l_musicLevel;
                        effectsLevel = l_effectsLevel;
                        DataManager.SaveVars(musicLevel, effectsLevel, sensetive, quality, crosshair, nickname);
                        menuStatus = MenuStatus.General;
                        SetQuality();
                    }
                    if (GUI.Button(buttons[7], "Назад"))
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
