using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Spawner : NetworkBehaviour {

    MapGenerator map;
    const int AICOUNT = (Map.XDemension * Map.YDemension) / 7;
    GameObject[] ais;
    bool set = true;
    float accuracy;
    float fireDelay;

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
                    fireDelay = 3;
                }
                if (difficult == 1)
                {
                    accuracy = 0.25F;
                    fireDelay = 2;
                }
                if (difficult == 2)
                {
                    accuracy = 0.1F;
                    fireDelay = 1;
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
            level = 0;
            NetworkServer.Spawn(ais[i]);
            ais[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            ais[i].GetComponent<Respawner>().AILevel = level;
            ais[i].GetComponent<AI>().lastUpdateTime = updateTime;
            ais[i].GetComponent<FireArm>().Accuracy = accuracy;
            ais[i].GetComponent<FireArm>().FireDelay *= fireDelay;
            ais[i].GetComponent<Wavegun>().Accuracy = accuracy;
            ais[i].GetComponent<Wavegun>().FireDelay *= fireDelay;
            ais[i].GetComponent<AIFlashlightController>().delayTime = fUpdateTime;
            ais[i].transform.position = GenerateVoidPlace(level);
            updateTime += 1 / 60;
            fUpdateTime += 5;
        }
    }
}
