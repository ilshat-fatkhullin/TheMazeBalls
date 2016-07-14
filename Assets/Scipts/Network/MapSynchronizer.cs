using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class MapSynchronizer : NetworkBehaviour {

    MapGenerator mapGenerator;
    Transform start;

	void Start () {
        mapGenerator = GameObject.Find("SceneManager").GetComponent<MapGenerator>();

        if (isLocalPlayer && !isServer)
        {
            CmdGetMap();
        }
    }

    private IEnumerable<string> SplitString(string str, int maxChunkSize)
    {
        for (int i = 0; i < str.Length; i += maxChunkSize)
            yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
    }

    [Command(channel = 0)]
    void CmdGetMap()
    {
        mapArrayString = mapGenerator.GetStringFromArray();
        string[] stringArray = SplitString(mapArrayString, 1300).ToArray<string>();
        for (int i = 0; i < stringArray.GetLength(0); i++)
        {
            RpcGetMapLocalPlayer(stringArray[i], i == stringArray.GetLength(0) - 1);
        }
    }

    public void UpdateMap()
    {
        if (isServer)
        {
            mapArrayString = mapGenerator.GetStringFromArray();
            string[] stringArray = SplitString(mapArrayString, 1300).ToArray<string>();
            for (int i = 0; i < stringArray.GetLength(0); i++)
            {
                RpcGetMapLocalPlayer(stringArray[i], i == stringArray.GetLength(0) - 1);
            }

            Vector3 generatedPos = mapGenerator.GetRandomFloor(0);
            generatedPos = new Vector3(generatedPos.x * Map.ScaleXZ, generatedPos.y * Map.LevelHeight, generatedPos.z * Map.ScaleXZ);
            generatedPos.y += 8F;
            gameObject.transform.position = generatedPos;
            gameObject.GetComponent<SynchronizeManager>().RpcUpdatePos(generatedPos);
            gameObject.GetComponent<Respawner>().spawn = generatedPos;
        }
    }

    string mapArrayString = "";
    [ClientRpc(channel = 0)]
    void RpcGetMapLocalPlayer(string arrayString, bool completed)
    {
        if (isLocalPlayer)
        {
            mapArrayString += arrayString;
            if (completed)
            {
                mapGenerator.UpdateArrayFromString(mapArrayString);
                mapGenerator.GenerateAsClient();
            }
        }
    }
}
