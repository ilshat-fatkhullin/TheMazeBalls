using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Algorithms;
using System;
using System.Collections.Generic;

public class MapGenerator : NetworkBehaviour {

    public Map map;
    GameObject[] ends;
    const int BonusesCount = Map.ScaleXZ / 8;

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
            Maze maze = new Maze(Map.XDemension / 2, Map.ZDemension / 2);
            for (int i = 0; i < Map.XDemension; i++)
                for (int k = 0; k < Map.ZDemension; k++)
                {
                    switch (maze[i, k])
                    {
                        case MazeCell.Void:
                            map.map[i, j, k] = Map.ElementType.Floor;
                            break;
                        case MazeCell.Wall:
                            map.map[i, j, k] = Map.ElementType.Wall;
                            break;
                    }
                }
        }
        ReplaceEnds();
        MakeWholes();
        MakeExit();
        map.CreateMap();
        MakeBonuses();
    }

    public void UpdateBonus(GameObject bonus, BonusController.BonusType bonusType)
    {
        NetworkServer.Destroy(bonus);

        Vector3 bonusPos = GetRandomFloor(Convert.ToInt32(transform.position.y / Map.LevelHeight));
        bonusPos = new Vector3(bonusPos.x * Map.ScaleXZ, bonusPos.y * Map.LevelHeight + Map.ScaleFloorY + 2.5F, bonusPos.z * Map.ScaleXZ);
        GameObject newBonus = null;
        switch (bonusType)
        {
            case BonusController.BonusType.Health:
                newBonus = GameObject.Instantiate(Resources.Load("Bonus0")) as GameObject;
                break;
            case BonusController.BonusType.Armor:
                newBonus = GameObject.Instantiate(Resources.Load("Bonus1")) as GameObject;
                break;
            case BonusController.BonusType.Exp:
                newBonus = GameObject.Instantiate(Resources.Load("Bonus2")) as GameObject;
                break;
        }
        newBonus.transform.position = bonusPos;
        NetworkServer.Spawn(newBonus);
    }

    public Vector3 GetRandomFloor(int level)
    {
        int x = UnityEngine.Random.Range(0, Map.XDemension / 2);
        int z = UnityEngine.Random.Range(1, Map.ZDemension / 2);

        while (true)
        {
            if (map.map[x, level, z] == Map.ElementType.Floor)
            {
                return new Vector3(x, level, z);
            }
            x += 1;
            if (x >= Map.XDemension)
            {
                x = 0;
                z += 1;
                if (z >= Map.YDemension)
                {
                    z = 0;
                }
            }
        }
    }

    private void MakeWholes()
    {
        for (int l = 0; l < Map.YDemension; l++)
        for (int i = 0; i < Map.WholesCount; i++)
        {
            Vector3 randomFloor = GetRandomFloor(l);
            map.map[Convert.ToInt32(randomFloor.x), Convert.ToInt32(randomFloor.y), Convert.ToInt32(randomFloor.z)] = Map.ElementType.Void;
        }
    }

    private void MakeBonuses()
    {
        for (int j = 0; j < 3; j++)
        for (int l = 0; l < Map.YDemension; l++)
            for (int i = 0; i < BonusesCount; i++)
            {
                Vector3 bonusPos = GetRandomFloor(l);
                bonusPos = new Vector3(bonusPos.x * Map.ScaleXZ, bonusPos.y * Map.LevelHeight + Map.ScaleFloorY + 2.5F, bonusPos.z * Map.ScaleXZ);
                GameObject bonus = GameObject.Instantiate(Resources.Load("Bonus" + j)) as GameObject;
                bonus.transform.position = bonusPos;
                NetworkServer.Spawn(bonus);
            }
    }

    private void MakeExit()
    {
        int index = UnityEngine.Random.Range(1, (map.map.GetLength(0) / 2) * 2 - 1);
        int side = UnityEngine.Random.Range(0, 3);

        int x = 0, y = 0;

        Vector2 floor_dir = Vector2.zero;

        if (side == 0)
        {
            floor_dir = Vector2.left;
            x = 0; y = index;
        }
        if (side == 1)
        {
            floor_dir = Vector2.up;
            x = index; y = map.map.GetLength(0) - 1;
        }
        if (side == 2)
        {
            floor_dir = Vector2.right;
            x = map.map.GetLength(0) - 1; y = index;
        }
        if (side == 3)
        {
            floor_dir = Vector2.down;
            x = index; y = 0;
        }

        map.map[x, Map.YDemension - 1, y] = Map.ElementType.Floor;

        Vector3 bonusPos = new Vector3(x * Map.ScaleXZ, (Map.YDemension - 1) * Map.LevelHeight + Map.ScaleFloorY + 2.5F, y * Map.ScaleXZ);
        GameObject bonus = GameObject.Instantiate(Resources.Load("ExitBonus")) as GameObject;
        bonus.transform.position = bonusPos;
        NetworkServer.Spawn(bonus);
    }

    private void ReplaceEnds()
    {
        for (int i = 0; i < ends.GetLength(0); i++)
        {
            Vector3 randomFloor = GetRandomFloor(i);
            map.map[Convert.ToInt32(randomFloor.x), Convert.ToInt32(randomFloor.y), Convert.ToInt32(randomFloor.z)] = Map.ElementType.Start;
            map.map[Convert.ToInt32(randomFloor.x), Convert.ToInt32(randomFloor.y) + 1, Convert.ToInt32(randomFloor.z)] = Map.ElementType.End;
            ends[i].transform.position = new Vector3(randomFloor.x * Map.ScaleXZ, (randomFloor.y * Map.LevelHeight) + Map.ScaleFloorY, randomFloor.z * Map.ScaleXZ);
        }
    }
}
