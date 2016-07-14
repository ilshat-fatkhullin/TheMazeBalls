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
        int x = UnityEngine.Random.Range(1, Map.XDemension - 1);
        int z = UnityEngine.Random.Range(1, Map.ZDemension - 1);
        if (map.map.map[x, level, z] != Map.ElementType.Floor)
        {
            if (x + 1 < Map.XDemension)
                if (map.map.map[x + 1, level, z] == Map.ElementType.Floor)
                {
                    x = x + 1;
                }
            if (x - 1 >= 0)
                if (map.map.map[x - 1, level, z] == Map.ElementType.Floor)
                {
                    x = x - 1;
                }
            if (z + 1 < Map.ZDemension)
                if (map.map.map[x, level, z + 1] == Map.ElementType.Floor)
                {
                    z = z + 1;
                }
            if (z - 1 >= 0)
                if (map.map.map[x, level, z - 1] == Map.ElementType.Floor)
                {
                    z = z - 1;
                }
        }

        return new Vector3(x * Map.ScaleXZ, level * Map.LevelHeight + Map.ScaleFloorY + 2.5F, z * Map.ScaleXZ);
    }

	public void SpawnAIs () {
        int level = 0;
        for (int i = 0; i < ais.GetLength(0); i++)
        {
            level = i / ((ais.GetLength(0) / 3));
            NetworkServer.Spawn(ais[i]);
            ais[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            ais[i].GetComponent<Respawner>().AILevel = level;
            ais[i].transform.position = GenerateVoidPlace(level);
        }
    }
}
