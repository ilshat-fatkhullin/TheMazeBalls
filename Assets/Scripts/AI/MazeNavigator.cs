using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;
using Algorithms;

public class MazeNavigator : NetworkBehaviour {

    const float JumpDelay = 0.1F;

    public Vector3 desiredVelocity;
    public float remainingDistance;
    public bool jump = false;
    public bool leftRight = false;
    bool localJump = false;

    MapGenerator mapGenerator;

    private bool TestRange(Vector3 pos)
    {
        if (pos.x >= Map.XDemension || pos.x < 0 ||
            pos.y >= Map.YDemension || pos.y < 0 ||
            pos.z >= Map.ZDemension || pos.z < 0)
            return false;
        return true;
    }

    public static Vector3 GetClosestPoint(Vector3 pos)
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
            mapGenerator = GameObject.Find("SceneManager").GetComponent<MapGenerator>();
        }
    }

    Vector3 lastPos = Vector3.zero;
    Vector3 target;
    Point2D currentDirection;
    Point2D currentRandomTarget;

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

            if (currentPos != lastPos)
            {
                if (TestRange(currentPos))
                {
                    mapGenerator.map.unitMap[x, y, z] = Map.UnitElementType.Unit;
                }

                if (TestRange(lastPos))
                {
                    mapGenerator.map.unitMap[(int)lastPos.x, (int)lastPos.y, (int)lastPos.z] = Map.UnitElementType.Void;
                }

                if (TestRange(currentPos))
                {
                    if (y != Map.YDemension - 1)
                    {
                        currentDirection = GetDirection(x, y, z);
                    }
                    else
                    {
                        currentDirection = GetRandomDirection(x, y, z);
                    }

                    int x2 = currentDirection.X + x;
                    int z2 = currentDirection.Y + z;

                    localJump = mapGenerator.map.map[x2, y, z2] == Map.ObjectElementType.Void || mapGenerator.map.unitMap[x2, y, z2] == Map.UnitElementType.Unit;
                    if (localJump)
                    {
                        jumpStartTime = Time.time;
                    }
                    target = new Vector3(x2 * Map.ScaleXZ, (y * Map.LevelHeight) + (Map.ScaleFloorY / 2) + 2.5F, z2 * Map.ScaleXZ);
                }

                lastPos = currentPos;
            }

            desiredVelocity = target - transform.position;
            desiredVelocity.y = 0;
            desiredVelocity.Normalize();
        }  
    }

	private Point2D GetDirection(int x, int y, int z)
	{
        Point2D aimPoint = null;
        Point2D startPoint = new Point2D(x, z);
        int length = int.MaxValue;
        int index = 0;
        for (int i = 0; i < mapGenerator.teleports.Count; i++)
        {
            Vector3 aimPos = GetClosestPoint(mapGenerator.teleports[i].transform.position);
            if (aimPos.y == y)
            {
                aimPoint = new Point2D(RoundFloat(aimPos.x), RoundFloat(aimPos.z));
                int newLength = mapGenerator.matrix[y].GetLength(aimPoint, startPoint);
                if (newLength < length)
                {
                    index = i;
                    length = newLength;
                }
            }
        }

        Vector3 aimPos2 = GetClosestPoint(mapGenerator.teleports[index].transform.position);
        aimPoint = new Point2D(RoundFloat(aimPos2.x), RoundFloat(aimPos2.z));

        Point2D path = mapGenerator.matrix[y].FindNext(startPoint, aimPoint);
        if (path != null)
        {
            return new Point2D(path.X - x, path.Y - z);
        }
        else
        {
            return new Point2D(0, 0);
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
            if (mapGenerator.map.map[x + 1, y, z] != Map.ObjectElementType.Wall)
            i++;
        if (TestRange(new Vector3(x - 1, y, z)))
            if (mapGenerator.map.map[x - 1, y, z] != Map.ObjectElementType.Wall)
            i++;
        if (TestRange(new Vector3(x, y, z + 1)))
            if (mapGenerator.map.map[x, y, z + 1] != Map.ObjectElementType.Wall)
            i++;
        if (TestRange(new Vector3(x, y, z - 1)))
            if (mapGenerator.map.map[x, y, z - 1] != Map.ObjectElementType.Wall)
            i++;

        return i <= 1;
    }

    Point2DEqualityComparer comparer = new Point2DEqualityComparer();

    private Point2D GetRandomDirection(int x, int y, int z)
    {        
        if (currentRandomTarget == null)
        {
            Vector3 currentRandomVector = mapGenerator.GetRandomFloor(Map.YDemension - 1);
            currentRandomTarget = new Point2D(RoundFloat(currentRandomVector.x), RoundFloat(currentRandomVector.z));
        }

        Point2D startPoint = new Point2D(x, z);
        Point2D path = mapGenerator.matrix[y].FindNext(startPoint, currentRandomTarget);

        if (comparer.Equals(path, currentRandomTarget))
        {
            Vector3 currentRandomVector = mapGenerator.GetRandomFloor(Map.YDemension - 1);
            currentRandomTarget = new Point2D(RoundFloat(currentRandomVector.x), RoundFloat(currentRandomVector.z));
        }

        if (path != null)
        {
            return new Point2D(path.X - x, path.Y - z);
        }
        else
        {
            return new Point2D(0, 0);
        }
    }

    private bool IsIntersection(int x, int y, int z)
    {
        bool x0 = false,
            x1 = false,
            z0 = false,
            z1 = false;

        if (TestRange(new Vector3(x + 1, y, z)))
            if (mapGenerator.map.map[x + 1, y, z] != Map.ObjectElementType.Wall)
                x0 = true;

        if (TestRange(new Vector3(x - 1, y, z)))
            if (mapGenerator.map.map[x - 1, y, z] != Map.ObjectElementType.Wall)
                x1 = true;

        if (TestRange(new Vector3(x, y, z + 1)))
            if (mapGenerator.map.map[x, y, z + 1] != Map.ObjectElementType.Wall)
                z0 = true;

        if (TestRange(new Vector3(x, y, z - 1)))
            if (mapGenerator.map.map[x, y, z - 1] != Map.ObjectElementType.Wall)
                z1 = true;

        return ((x0 || x1) && (z0 || z1));
    }
}
