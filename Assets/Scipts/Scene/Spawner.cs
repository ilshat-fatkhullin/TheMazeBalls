using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Spawner : NetworkBehaviour {

    MapGenerator map;
    int aiCount = 3;
    GameObject[] ais;
    bool set = true;

    void Update() {
        if (set)
        if (isServer)
        {
            set = false;
            map = gameObject.GetComponent<MapGenerator>();
            ais = new GameObject[aiCount];
            for (int i = 0; i < ais.GetLength(0); i++)
            {
                ais[i] = GameObject.Instantiate(Resources.Load("AISphere")) as GameObject;
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
        for (int i = 0; i < ais.GetLength(0); i++)
        {
            level = i / ((ais.GetLength(0) / 3));
            NetworkServer.Spawn(ais[i]);
            ais[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            ais[i].GetComponent<Respawner>().AILevel = level;
            ais[i].GetComponent<AI>().lastUpdateTime = updateTime;
            ais[i].transform.position = GenerateVoidPlace(level);
            updateTime += 1 / 60;
        }
    }
}
