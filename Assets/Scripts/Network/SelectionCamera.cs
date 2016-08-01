using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class SelectionCamera : NetworkBehaviour {

    int count = 125;
    int currentFlagIndex = 0;
    int smile = 0, eye = 0, background = 0;
    Texture[] flags;
    UserInterface userInterface;
    GUIStyle guiStyle = new GUIStyle();

    Rect currentFlag, playButton,
        backgroundScrollerRect, smileScrollerRect, eyeScrollerRect,
        backgroundLabelRect, smileLabelRect, eyeLabelRect;
    MapGenerator mapGenerator;

    void Start()
    {
        flags = new Texture[count];
        float pixelX = Screen.width / 10,
        pixelY = Screen.height / 10;
        for (int i = 0; i < count; i++)
        {         
            flags[i] = Resources.Load(@"Flags\" + i) as Texture;
        }

        if (isLocalPlayer)
        {
            gameObject.GetComponent<Camera>().enabled = true;
        }

        currentFlag = new Rect(Screen.width - 4 * pixelX, pixelY, pixelY * 6, pixelY * 6);
        playButton = new Rect(Screen.width - 2 * pixelX, Screen.height - pixelY * 2, pixelX, pixelY);

        backgroundScrollerRect = new Rect(pixelX * 3, pixelY * 2, pixelX * 2, pixelY);
        smileScrollerRect = new Rect(pixelX * 3, pixelY * 4, pixelX * 2, pixelY);
        eyeScrollerRect = new Rect(pixelX * 3, pixelY * 6, pixelX * 2, pixelY);

        backgroundLabelRect = new Rect(pixelX * 3, pixelY, pixelX * 4, pixelY);
        smileLabelRect = new Rect(pixelX * 3, pixelY * 3, pixelX * 4, pixelY);
        eyeLabelRect = new Rect(pixelX * 3, pixelY * 5, pixelX * 4, pixelY);

        mapGenerator = GameObject.Find("SceneManager").GetComponent<MapGenerator>();

        guiStyle.fontSize = Convert.ToInt32(pixelY / 2);
        guiStyle.normal.textColor = Color.white;

        userInterface = GameObject.Find("UserInterface").GetComponent<UserInterface>();
    }

    void Update()
    {
        if (isLocalPlayer)
            Cursor.visible = true;
    }

	void OnGUI () {
        if (isLocalPlayer)
        {
            GUI.DrawTexture(currentFlag, flags[currentFlagIndex]);

            background = Convert.ToInt32(GUI.HorizontalSlider(backgroundScrollerRect, background, 0, 4));
            smile = Convert.ToInt32(GUI.HorizontalSlider(smileScrollerRect, smile, 0, 4));
            eye = Convert.ToInt32(GUI.HorizontalSlider(eyeScrollerRect, eye, 0, 4));

            GUI.Label(backgroundLabelRect, userInterface.wordsList[14], guiStyle);
            GUI.Label(smileLabelRect, userInterface.wordsList[16], guiStyle);
            GUI.Label(eyeLabelRect, userInterface.wordsList[15], guiStyle);

            currentFlagIndex = eye + smile * 5 + background * 25;

            if (GUI.Button(playButton, userInterface.wordsList[17]))
            {
                Cursor.visible = false;
                Play();
            }
        }
	}

    void Play()
    {
        CmdSpawn(currentFlagIndex, DataManager.LoadVars().nickname);
    }

    [Command(channel = 0)]
    void CmdSpawn(int index, string nickname)
    {
        GameObject character = null;
        character = (GameObject)Instantiate(Resources.Load("Character"));
        character.GetComponent<MeshRenderer>().material.mainTexture = flags[index];

        Vector3 generatedPos = mapGenerator.GetRandomFloor(0);
        generatedPos = new Vector3(generatedPos.x * Map.ScaleXZ, generatedPos.y * Map.LevelHeight, generatedPos.z * Map.ScaleXZ);
        generatedPos.y += 5F;
        character.transform.position = generatedPos;
        NetworkServer.Spawn(character);
        NetworkServer.ReplacePlayerForConnection(connectionToClient, character, playerControllerId);
        character.GetComponent<SynchronizeManager>().RpcUpdatePos(generatedPos);
        character.GetComponent<FlagsSynchronizer>().flagIndex = index;
        FlagManager.UpdateFlags();
        character.GetComponent<Respawner>().spawn = generatedPos;
        if (nickname != null)
        character.GetComponent<Exp>().nickname = nickname;

        GameObject.Find("UserInterface").GetComponent<PlayersTab>().UpdateExps();

        NetworkServer.Destroy(gameObject);
    }
}
