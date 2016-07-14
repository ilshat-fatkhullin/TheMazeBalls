using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Spawner : NetworkBehaviour {

    MapGenerator map;
    const int AICOUNT = (Map.XDemension * Map.YDemension) / 5;
    GameObject[] ais;
    bool set = true;
    float accuracy;

    void Update() {
        if (set)
        if (isServer)
        {
            set = false;
            map = gameObject.GetComponent<MapGenerator>();
            ais = new GameObject[AICOUNT * Map.YDemension];
            for (int i = 0; i < ais.GetLength(0); i++)
            {
                ais[i] = GameObject.Instantiate(Resources.Load("AISphere")) as GameObject;
            }
            int difficult = DataManager.LoadBool().difficult;
                if (difficult == 0)
                {
                    accuracy = 0.5F;
                }
                if (difficult == 1)
                {
                    accuracy = 0.25F;
                }
                if (difficult == 2)
                {
                    accuracy = 0.1F;
                }
                SpawnAIs();
        }
    }

    public Vector3 GenerateVoidPlace(int level)
    {
        Vector3 mapPoint = map.GetRandomFloor(level);

        int x = Convert.ToInt32(mapPoint.x);
        int z = Convert.ToInt32(mapPoint.z);

        return new Vector3(x * Map.ScaleXZ, level * Map.LevelHeight + Map.ScaleFloorY + 2.5F, z * Map.ScaleXZ);
    }

	public void SpawnAIs () {
        int level = 0;
        float updateTime = Time.time;
        float fUpdateTime = Time.time;
        for (int i = 0; i < ais.GetLength(0); i++)
        {
            level = i / AICOUNT;
            NetworkServer.Spawn(ais[i]);
            ais[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            ais[i].GetComponent<Respawner>().AILevel = level;
            ais[i].GetComponent<AI>().lastUpdateTime = updateTime;
            ais[i].GetComponent<FireArm>().Accuracy = accuracy;
            ais[i].GetComponent<AIFlashlightController>().delayTime = fUpdateTime;
            ais[i].transform.position = GenerateVoidPlace(level);
            updateTime += 1 / 60;
            fUpdateTime += 5;
        }
    }
}
