using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class SelectionCamera : NetworkBehaviour {

    int count = 196;
    int start = 0, end = 48;
    int currentFlagIndex = 0;
    int scrollerMax = 19, scrollerCurrent;
    Texture[] flags;
    Rect[] flagsRects;
    Rect currentFlag, playButton, scrollerRect;
    MapGenerator mapGenerator;

    void Start()
    {
        flags = new Texture[count];
        flagsRects = new Rect[count];
        float pixelX = Screen.width / 10,
        pixelY = Screen.height / 10;
        for (int i = 0; i < count; i++)
        {
            if (i <= end)
            flagsRects[i] = new Rect(pixelX * (i % 8) + pixelX, pixelY * (i / 8) + pixelY, pixelX, pixelY);
            flags[i] = Resources.Load(@"Flags\" + i) as Texture;
        }

        if (isLocalPlayer)
        {
            gameObject.GetComponent<Camera>().enabled = true;
        }

        currentFlag = new Rect(Screen.width - 4 * pixelX, Screen.height - pixelY * 2, pixelX, pixelY);
        playButton = new Rect(Screen.width - 2 * pixelX, Screen.height - pixelY * 2, pixelX, pixelY);
        scrollerRect = new Rect(pixelX, pixelY / 2, pixelX * 8, pixelY);

        mapGenerator = GameObject.Find("SceneManager").GetComponent<MapGenerator>();
    }

	void OnGUI () {
        if (isLocalPlayer)
        {
            int j = 0;
            for (int i = start; i < end; i++)
            {
                if (GUI.Button(flagsRects[j], flags[i]))
                {
                    currentFlagIndex = i;
                }
                j++;
            }

            GUI.DrawTexture(currentFlag, flags[currentFlagIndex]);

            scrollerCurrent = Convert.ToInt32(GUI.HorizontalSlider(scrollerRect, scrollerCurrent, 0, scrollerMax));
            start = 8 * scrollerCurrent;
            end = start + 48;
            if (end > flags.GetLength(0))
            {
                end = flags.GetLength(0);
            }

            if (GUI.Button(playButton, "Играть"))
            {
                Play();
            }
        }
	}

    void Play()
    {
        CmdSpawn(currentFlagIndex);
    }

    [Command(channel = 0)]
    void CmdSpawn(int index)
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

        NetworkServer.Destroy(gameObject);
    }
}
