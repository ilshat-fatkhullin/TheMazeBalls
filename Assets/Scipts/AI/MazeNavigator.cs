using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;

public class MazeNavigator : NetworkBehaviour {

    const float JumpDelay = 0.5F;

    public Vector3 desiredVelocity;
    public float remainingDistance;
    public bool jump = false;
    bool localJump = false;

    MapGenerator mapGeneratore;

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
        int x = (int)((pos.x + Map.ScaleXZ / 2) / Map.ScaleXZ);
        int y = (int)(pos.y / Map.LevelHeight);
        int z = (int)((pos.z + Map.ScaleXZ / 2) / Map.ScaleXZ);

        return new Vector3(x, y, z);
    }

    void Start()
    {
        if (isServer)
        {
            mapGeneratore = GameObject.Find("SceneManager").GetComponent<MapGenerator>();
        }
    }

    Vector3 lastPos = Vector3.zero;
    Vector3 target;
    Vector3 currentDirection;

    float jumpStartTime;

    void Update()
    {
        if (isServer)
        {
            if (localJump)
                if (Time.time - jumpStartTime > JumpDelay)
                {
                    localJump = false;
                    jump = true;
                }

            Vector3 currentPos = GetClosestPoint(transform.position);

            int x = Convert.ToInt32(currentPos.x);
            int y = Convert.ToInt32(currentPos.y);
            int z = Convert.ToInt32(currentPos.z);

            if (TestRange(currentPos))
            {
                mapGeneratore.map.unitMap[x, y, z] = Map.UnitElementType.Unit;
            }
            if (currentPos != lastPos)
            {
                if (TestRange(lastPos))
                    mapGeneratore.map.unitMap[Convert.ToInt32(lastPos.x), Convert.ToInt32(lastPos.y), Convert.ToInt32(lastPos.z)] = Map.UnitElementType.Void;

                if (TestRange(currentPos))
                {
                    if (IsIntersection(x, y, z) || IsDeadEnd(x, y, z) || desiredVelocity == Vector3.zero)
                    {
                        currentDirection = GetRandomDirection(x, y, z);                       
                    }

                    int x2 = RoundFloat(currentDirection.x) + x;
                    int y2 = RoundFloat(currentDirection.y) + y;
                    int z2 = RoundFloat(currentDirection.z) + z;

                    localJump = mapGeneratore.map.map[x2, y2, z2] != Map.ObjectElementType.Floor || mapGeneratore.map.unitMap[x2, y2, z2] != Map.UnitElementType.Void;
                    if (localJump)
                    {
                        jumpStartTime = Time.time;
                    }
                    target = new Vector3(x2 * Map.ScaleXZ, (y2 * Map.LevelHeight) + (Map.ScaleFloorY / 2) + 2.5F, z2 * Map.ScaleXZ);
                }

                lastPos = currentPos;
            }

            desiredVelocity = target - transform.position;
            desiredVelocity.y = 0;
            desiredVelocity.Normalize();
        }  
    }

    private int RoundFloat(float value)
    {
        if (Math.Sign(value) == -1)
            return (int)(value - 0.5);  
        else
            return (int)(value + 0.5);
    }

    private bool IsDeadEnd(int x, int y, int z)
    {
        int i = 0;
        if (TestRange(new Vector3(x + 1, y, z)))
            if (mapGeneratore.map.map[x + 1, y, z] != Map.ObjectElementType.Wall)
            i++;
        if (TestRange(new Vector3(x - 1, y, z)))
            if (mapGeneratore.map.map[x - 1, y, z] != Map.ObjectElementType.Wall)
            i++;
        if (TestRange(new Vector3(x, y, z + 1)))
            if (mapGeneratore.map.map[x, y, z + 1] != Map.ObjectElementType.Wall)
            i++;
        if (TestRange(new Vector3(x, y, z - 1)))
            if (mapGeneratore.map.map[x, y, z - 1] != Map.ObjectElementType.Wall)
            i++;

        return i <= 1;
    }

    private Vector3 GetRandomDirection(int x, int y, int z)
    {
        int i = UnityEngine.Random.Range(0, 4);
        Vector3 dir = Quaternion.Euler(0, 90 * i, 0) * Vector3.forward;
        for (int j = 0; j < 5; j++)
        {
            int xDelta = RoundFloat(dir.x),
                yDelta = RoundFloat(dir.y),
                zDelta = RoundFloat(dir.z);

            if (TestRange(new Vector3(x + xDelta, y, z + zDelta)))
                if (mapGeneratore.map.map[x + xDelta, y, z + zDelta] != Map.ObjectElementType.Wall)
                    return dir;
            dir = Quaternion.Euler(0, 90, 0) * dir;
        }

        return Vector3.zero;
    }

    private bool IsIntersection(int x, int y, int z)
    {
        bool x0 = false,
            x1 = false,
            z0 = false,
            z1 = false;

        if (TestRange(new Vector3(x + 1, y, z)))
            if (mapGeneratore.map.map[x + 1, y, z] != Map.ObjectElementType.Wall)
                x0 = true;

        if (TestRange(new Vector3(x - 1, y, z)))
            if (mapGeneratore.map.map[x - 1, y, z] != Map.ObjectElementType.Wall)
                x1 = true;

        if (TestRange(new Vector3(x, y, z + 1)))
            if (mapGeneratore.map.map[x, y, z + 1] != Map.ObjectElementType.Wall)
                z0 = true;

        if (TestRange(new Vector3(x, y, z - 1)))
            if (mapGeneratore.map.map[x, y, z - 1] != Map.ObjectElementType.Wall)
                z1 = true;

        return ((x0 || x1) && (z0 || z1));
    }
}
