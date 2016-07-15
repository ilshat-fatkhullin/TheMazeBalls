using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Menu : MonoBehaviour {

    public Texture background;
    public Texture menuBackground;

    NetworkManager networkManager;

    enum MenuStatus { General, SinglePlayer, Server, Client, MultiPlayer, Options }
    MenuStatus menuStatus = MenuStatus.General;

    Rect[] buttons = new Rect[8];
    Rect backgroundRect;
    Rect menuBackgroundRect;

    int difficult = 1;
    int quality = 1;
    float musicLevel = 1;
    float effectsLevel = 1;
    int l_quality = 1;
    float l_musicLevel = 1;
    float l_effectsLevel = 1;
    string[] difficultSelection;
    string[] qualitySelection;
    string host = "localhost";

    void Start () {
        backgroundRect = new Rect(0, 0, Screen.width, Screen.height);

        float pixel = Screen.height / 10;

        menuBackgroundRect = new Rect(0, 0, pixel * 6, Screen.height);

        for (int i = 0; i < buttons.GetLength(0); i++)
        {
            buttons[i] = new Rect(pixel, pixel * i + pixel, pixel * 4, pixel / 1.2F);
        }

        difficultSelection = new string[] { "Легко", "Средне", "Сложно" };
        qualitySelection = new string[] { "Низкое", "Среднее", "Высокое" };

        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        Settings settingsStruct = DataManager.LoadVars();
        musicLevel = settingsStruct.musicLevel;
        effectsLevel = settingsStruct.effectsLevel;
        quality = settingsStruct.qualityLevel;
        l_musicLevel = musicLevel;
        l_effectsLevel = effectsLevel;
        l_quality = quality;
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
                if (GUI.Button(buttons[2], "Играть"))
                {
                    networkManager.StartHost();
                }
                if (GUI.Button(buttons[3], "Назад"))
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
                    DataManager.SaveVars(musicLevel, effectsLevel, quality);
                    menuStatus = MenuStatus.General;
                }
                if (GUI.Button(buttons[7], "Назад"))
                {
                    l_quality = quality;
                    l_musicLevel = musicLevel;
                    l_effectsLevel = effectsLevel;
                    menuStatus = MenuStatus.General;
                }
                break;
            case MenuStatus.Server:
                GUI.Label(buttons[0], "Сложность:");
                difficult = GUI.SelectionGrid(buttons[1], difficult, difficultSelection, 3);
                if (GUI.Button(buttons[2], "Создать"))
                {
                    networkManager.StartHost();
                }
                if (GUI.Button(buttons[3], "Назад"))
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
