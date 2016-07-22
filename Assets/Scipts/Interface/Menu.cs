using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Menu : MonoBehaviour {

    public Texture background;
    public Texture menuBackground;

    NetworkManager networkManager;

    enum MenuStatus { General, SinglePlayer, Server, Client, MultiPlayer, Options, Sound, Graphics, Gameplay }
    MenuStatus menuStatus = MenuStatus.General;

    Rect[] buttons = new Rect[12];
    Rect backgroundRect;
    Rect menuBackgroundRect;

    int difficult = 1;
    int quality = 1;
    int crosshair = 0;
    float musicLevel = 1;
    float effectsLevel = 1;
    float sensetive = 1;
    int l_quality = 1;
    int l_crosshair = 0;
    float l_musicLevel = 1;
    float l_effectsLevel = 1;
    float l_sensetive = 1;
    string l_nickname;
    string nickname = "Player";
    string[] difficultSelection;
    string[] qualitySelection;
    string[] colorSelection;
    string host = "localhost";
    bool isDarkness;

    void Start () {
        backgroundRect = new Rect(0, 0, Screen.width, Screen.height);

        float pixel = Screen.height / 10;

        menuBackgroundRect = new Rect(0, 0, pixel * 6, Screen.height);

        for (int i = 0; i < buttons.GetLength(0); i++)
        {
            buttons[i] = new Rect(pixel, pixel * i + pixel / 3, pixel * 4, pixel / 1.5F);
        }

        difficultSelection = new string[] { "Легко", "Средне", "Сложно" };
        qualitySelection = new string[] { "Низкое", "Среднее", "Высокое" };
        colorSelection = new string[] { "Красный", "Зелёный", "Синий" };

        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        Settings settingsStruct = DataManager.LoadVars();
        musicLevel = settingsStruct.musicLevel;
        effectsLevel = settingsStruct.effectsLevel;
        quality = settingsStruct.qualityLevel;
        nickname = settingsStruct.nickname;
        crosshair = settingsStruct.crosshair;
        sensetive = settingsStruct.mouseSensetive;
        l_musicLevel = musicLevel;
        l_effectsLevel = effectsLevel;
        l_quality = quality;
        l_sensetive = sensetive;
        l_crosshair = crosshair;
        if (nickname == null)
            nickname = "Player";
        l_nickname = nickname;
    }
	
	void OnGUI () {
        GUI.DrawTexture(backgroundRect, background);
        GUI.DrawTexture(menuBackgroundRect, menuBackground);
        switch (menuStatus)
        {
            case MenuStatus.General:
                if (GUI.Button(buttons[0], "Одиночная игра"))
                {
                    menuStatus = MenuStatus.SinglePlayer;
                }
                if (GUI.Button(buttons[1], "Сетевая игра"))
                {
                    menuStatus = MenuStatus.MultiPlayer;
                }
                if (GUI.Button(buttons[2], "Настройки"))
                {
                    menuStatus = MenuStatus. Options;
                }
                if (GUI.Button(buttons[3], "Выйти"))
                {
                    Application.Quit();
                }
                break;
            case MenuStatus.SinglePlayer:
                GUI.Label(buttons[0], "Сложность:");
                difficult = GUI.SelectionGrid(buttons[1], difficult, difficultSelection, 3);
                isDarkness = GUI.Toggle(buttons[2], isDarkness, "Мрак");
                if (GUI.Button(buttons[3], "Играть"))
                {
                    StartGame();
                    networkManager.StartHost();
                }
                if (GUI.Button(buttons[4], "Назад"))
                {
                    menuStatus = MenuStatus.General;
                }
                break;
            case MenuStatus.MultiPlayer:
                if (GUI.Button(buttons[0], "Сервер"))
                {
                    menuStatus = MenuStatus.Server;
                }
                if (GUI.Button(buttons[1], "Клиент"))
                {
                    menuStatus = MenuStatus.Client;
                }
                if (GUI.Button(buttons[2], "Назад"))
                {
                    menuStatus = MenuStatus.General;
                }
                break;
            case MenuStatus.Options:
                if (GUI.Button(buttons[0], "Графика"))
                {
                    menuStatus = MenuStatus.Graphics;
                }
                if (GUI.Button(buttons[1], "Звук"))
                {
                    menuStatus = MenuStatus.Sound;
                }
                if (GUI.Button(buttons[2], "Игра"))
                {
                    menuStatus = MenuStatus.Gameplay;
                }
                if (GUI.Button(buttons[3], "Назад"))
                {
                    menuStatus = MenuStatus.General;
                }               
                break;
            case MenuStatus.Graphics:
                GUI.Label(buttons[0], "Качество графики:");
                l_quality = GUI.SelectionGrid(buttons[1], l_quality, qualitySelection, 3);
                if (GUI.Button(buttons[2], "Принять"))
                {
                    Submit();
                }
                if (GUI.Button(buttons[3], "Назад"))
                {
                    Back();
                }
                break;
            case MenuStatus.Sound:
                GUI.Label(buttons[0], "Громкость музыки:");
                l_musicLevel = GUI.HorizontalSlider(buttons[1], l_musicLevel, 0, 1);
                GUI.Label(buttons[2], "Громкость эффектов:");
                l_effectsLevel = GUI.HorizontalSlider(buttons[3], l_effectsLevel, 0, 1);
                if (GUI.Button(buttons[4], "Принять"))
                {
                    Submit();
                }
                if (GUI.Button(buttons[5], "Назад"))
                {
                    Back();
                }
                break;
            case MenuStatus.Gameplay:
                GUI.Label(buttons[0], "Никнейм:");
                l_nickname = GUI.TextField(buttons[1], l_nickname);
                if (l_nickname.Length > 15)
                    l_nickname = l_nickname.Substring(0, 15);
                GUI.Label(buttons[2], "Цвет прицела:");
                l_crosshair = GUI.SelectionGrid(buttons[3], l_crosshair, colorSelection, 3);
                GUI.Label(buttons[4], "Чувствительность мыши:");
                l_sensetive = GUI.HorizontalSlider(buttons[5], l_sensetive, 0, 2);
                if (GUI.Button(buttons[6], "Принять"))
                {
                    Submit();
                }
                if (GUI.Button(buttons[7], "Назад"))
                {
                    Back();
                }
                break;
            case MenuStatus.Server:
                GUI.Label(buttons[0], "Сложность:");
                difficult = GUI.SelectionGrid(buttons[1], difficult, difficultSelection, 3);
                isDarkness = GUI.Toggle(buttons[2], isDarkness, "Мрак");
                if (GUI.Button(buttons[3], "Создать"))
                {
                    StartGame();
                    networkManager.StartHost();
                }
                if (GUI.Button(buttons[4], "Назад"))
                {
                    menuStatus = MenuStatus.General;
                }
                break;
            case MenuStatus.Client:
                GUI.Label(buttons[0], "IP:");
                host = GUI.TextField(buttons[1], host);
                if (GUI.Button(buttons[2], "Подключиться"))
                {
                    Connect();
                }
                if (GUI.Button(buttons[3], "Назад"))
                {
                    menuStatus = MenuStatus.General;
                }
                break;
        }
	}

    void Back()
    {
        l_quality = quality;
        l_musicLevel = musicLevel;
        l_effectsLevel = effectsLevel;
        l_sensetive = sensetive;
        l_crosshair = crosshair;
        l_nickname = nickname;
        menuStatus = MenuStatus.Options;
    }

    void Submit()
    {
        quality = l_quality;
        musicLevel = l_musicLevel;
        effectsLevel = l_effectsLevel;
        sensetive = l_sensetive;
        crosshair = l_crosshair;
        nickname = l_nickname;
        DataManager.SaveVars(musicLevel, effectsLevel, sensetive, quality, crosshair, nickname);
        menuStatus = MenuStatus.Options;
    }

    void StartGame()
    {
        DataManager.SaveBool(isDarkness, difficult);
    }

    void OnServerError()
    {
        menuStatus = MenuStatus.Server;
    }

    void OnClientError()
    {
        menuStatus = MenuStatus.Client;
    }

    void Connect()
    {
        networkManager.networkAddress = host;
        networkManager.StartClient();
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        Debug.Log("Failed to connect: " + error.ToString());
        menuStatus = MenuStatus.General;
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        if (Network.isClient)
        {
            Debug.Log("Disconnected from server: " + info.ToString());
        }
        else
        {
            Debug.Log("Connections closed");
        }
        menuStatus = MenuStatus.General;
    }
}
