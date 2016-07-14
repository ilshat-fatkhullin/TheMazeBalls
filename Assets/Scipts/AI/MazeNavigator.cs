using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;

public class MazeNavigator : NetworkBehaviour {

    public Vector3 desiredVelocity;
    public float remainingDistance;

    MapGenerator map;

    private bool TestRange(Vector3 pos)
    {
        if (pos.x >= Map.XDemension || pos.x < 0 ||
            pos.y >= Map.YDemension || pos.y < 0 ||
            pos.z >= Map.ZDemension || pos.z < 0)
            return false;
        return true;
    }

    private Vector3 GetClosestPoint(Vector3 pos)
    {
        int x = Convert.ToInt32(pos.x / Map.ScaleXZ);
        int y = Convert.ToInt32(pos.y / Map.LevelHeight);
        int z = Convert.ToInt32(pos.z / Map.ScaleXZ);

        return new Vector3(x, y, z);
    }

    void Start()
    {
        if (isServer)
        map = GameObject.Find("SceneManager").GetComponent<MapGenerator>();
    }

    Vector3 last = Vector3.zero;
    Vector3 el;
    Vector3 target;

    void Update()
    {
        if (isServer)
        {
            Vector3 beginPos = GetClosestPoint(transform.position);
            if (beginPos != last)
            {
                int i = UnityEngine.Random.Range(0, 4);
                Vector3 dir = Quaternion.Euler(0, 90 * i, 0) * Vector3.forward;
                Vector3 psDir = Vector3.zero;
                for (int j = 0; j < 5; j++)
                {
                    psDir = (beginPos + dir);
                    if (TestRange(psDir) && map.map.map[Convert.ToInt32(psDir.x), Convert.ToInt32(psDir.y), Convert.ToInt32(psDir.z)] == Map.ElementType.Floor)
                    {
                        break;
                    }
                    dir = Quaternion.Euler(0, 90, 0) * dir;
                }
                if (TestRange(psDir) && psDir != Vector3.zero)
                {
                    el = new Vector3(Convert.ToInt32(psDir.x), Convert.ToInt32(psDir.y), Convert.ToInt32(psDir.z));
                }
                if (el != null)
                target = new Vector3(el.x * Map.ScaleXZ, el.y * Map.LevelHeight, el.z * Map.ScaleXZ);
            }


            desiredVelocity = (target - transform.position).normalized;
            desiredVelocity.y = 0;
            last = beginPos;
        }  
    }
}
