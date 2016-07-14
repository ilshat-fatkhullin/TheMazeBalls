﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Algorithms;
using System;
using System.Collections.Generic;

public class MapGenerator : NetworkBehaviour {

    public Map map;
    GameObject[] ends;
    int currentLevel = 1;

    void Start () {
        map = new Map();
        if (isServer)
        {
            ends = new GameObject[Map.YDemension - 1];
            for (int i = 0; i < ends.GetLength(0); i++)
            {
                ends[i] = GameObject.Instantiate(Resources.Load("End")) as GameObject;
                NetworkServer.Spawn(ends[i]);
            }
            GenerateAsServer();
        }
    }

    public string GetStringFromArray()
    {
        string returned_string = "";

        for (int i = 0; i < Map.XDemension; i++)
            for (int j = 0; j < Map.YDemension; j++)
                for (int k = 0; k < Map.ZDemension; k++)
                {
                    switch (map.map[i, j, k])
                    {
                        case Map.ElementType.Floor:
                            returned_string += "f";
                            break;
                        case Map.ElementType.Wall:
                            returned_string += "w";
                            break;
                        case Map.ElementType.Void:
                            returned_string += "v";
                            break;
                        case Map.ElementType.Start:
                            returned_string += "s";
                            break;
                        case Map.ElementType.End:
                            returned_string += "e";
                            break;
                    }
                }

        return returned_string;
    }

    public void UpdateArrayFromString(string s)
    {
        Clear();
        for (int i = 0; i < Map.XDemension; i++)
            for (int j = 0; j < Map.YDemension; j++)
                for (int k = 0; k < Map.ZDemension; k++)
                {
                    char cur_ch = s[k + j * Map.ZDemension + i * Map.ZDemension * Map.YDemension];
                    if (cur_ch == 'f')
                        map.map[i, j, k] = Map.ElementType.Floor;
                    if (cur_ch == 'w')
                        map.map[i, j, k] = Map.ElementType.Wall;
                    if (cur_ch == 'v')
                        map.map[i, j, k] = Map.ElementType.Void;
                    if (cur_ch == 's')
                        map.map[i, j, k] = Map.ElementType.Start;
                    if (cur_ch == 'e')
                        map.map[i, j, k] = Map.ElementType.End;
                }
    }

    public void Clear()
    {
        map.ClearMap();
    }

    public void GenerateAsClient()
    {
        map.CreateMap();
    }

    public void GenerateAsServer()
    {
        for (int j = 0; j < Map.YDemension; j++)
        {
            Maze maze = new Maze(Map.XDemension, Map.ZDemension);
            for (int i = 0; i < Map.XDemension; i++)
                for (int k = 0; k < Map.ZDemension; k++)
                {
                    switch (maze[i, k])
                    {
                        case MazeCell.Void:
                            map.map[i, j, k] = Map.ElementType.Floor;
                            break;
                        case MazeCell.Wall:
                            map.map[i, j, k] = Map.ElementType.Void;
                            if (UnityEngine.Random.Range(1, 3) == 1)
                            {
                                map.map[i, j, k] = Map.ElementType.Wall;
                            }
                            break;
                    }
                }
        }
        map.CreateMap();
        ReplaceEnds();
    }

    public Vector3 GetRandomFloor(int level)
    {
        int x = UnityEngine.Random.Range(0, Map.XDemension / 2);
        int z = UnityEngine.Random.Range(1, Map.ZDemension / 2);

        return new Vector3(x * 2 + 1, level, z * 2 + 1);
    }

    private void ReplaceEnds()
    {
        for (int i = 0; i < ends.GetLength(0); i++)
        {
            Vector3 randomFloor = GetRandomFloor(i);
            map.map[Convert.ToInt32(randomFloor.x), Convert.ToInt32(randomFloor.y), Convert.ToInt32(randomFloor.z)] = Map.ElementType.Start;
            ends[i].transform.position = new Vector3(randomFloor.x * Map.ScaleXZ, (randomFloor.y * Map.LevelHeight) + Map.ScaleFloorY, randomFloor.z * Map.ScaleXZ);
        }
    }
}