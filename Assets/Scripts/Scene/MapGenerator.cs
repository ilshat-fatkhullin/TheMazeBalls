using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Algorithms;
using System;
using System.Collections.Generic;

public class MapGenerator : NetworkBehaviour {

    public Map map;
	public Maze[] matrix;
    List<Point2D>[] floorPoints;
    List<GameObject> bonuses = new List<GameObject>();
    public List<GameObject> teleports = new List<GameObject>();
    int[] floorIndexes;
    [SyncVar]
    string array;

    void Start () {
        map = new Map();
        matrix = new Maze[Map.YDemension];
        if (isServer)
        {
            GenerateAsServer();
        }
        else
        {
            GenerateAsClient();
        }
    }

    string lastArray = "";

    void Update()
    {
        if (!isServer)
        {
            if (lastArray != array)
            {
                GenerateAsClient();
                lastArray = array;
            }
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
                        case Map.ObjectElementType.Floor:
                            returned_string += "f";
                            break;
                        case Map.ObjectElementType.Wall:
                            returned_string += "w";
                            break;
                        case Map.ObjectElementType.Void:
                            returned_string += "v";
                            break;
                        case Map.ObjectElementType.Teleport:
                            returned_string += "s";
                            break;
                        case Map.ObjectElementType.TeleportationPlace:
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
                        map.map[i, j, k] = Map.ObjectElementType.Floor;
                    if (cur_ch == 'w')
                        map.map[i, j, k] = Map.ObjectElementType.Wall;
                    if (cur_ch == 'v')
                        map.map[i, j, k] = Map.ObjectElementType.Void;
                    if (cur_ch == 's')
                        map.map[i, j, k] = Map.ObjectElementType.Teleport;
                    if (cur_ch == 'e')
                        map.map[i, j, k] = Map.ObjectElementType.TeleportationPlace;
                }
    }

    public void Clear()
    {
        ClearTeleports();
        ClearBonuses();
        map.ClearMap();
    }

    public void GenerateAsClient()
    {
        Clear();
        UpdateArrayFromString(array);
        map.CreateMap();
    }

    public void GenerateAsServer()
    {
        Clear();

        map = new Map();
        matrix = new Maze[Map.YDemension];

        for (int j = 0; j < Map.YDemension; j++)
        {
			matrix[j] = new Maze(Map.XDemension / 2, Map.ZDemension / 2);
            for (int i = 0; i < Map.XDemension; i++)
                for (int k = 0; k < Map.ZDemension; k++)
                {
					switch (matrix[j][i, k])
                    {
                        case MazeCell.Void:
                            map.map[i, j, k] = Map.ObjectElementType.Floor;
                            break;
                        case MazeCell.Wall:
                            map.map[i, j, k] = Map.ObjectElementType.Wall;
                            break;
                    }
                }
        }

        FillFloorList();
        CreateTeleports();
        MarkHoles();
        map.CreateMap();
        CreateBonuses();

        array = GetStringFromArray();
    }

    public Vector3 GetRandomFloor(int level)
    {
        int x = UnityEngine.Random.Range(0, Map.XDemension - 1);
        int z = UnityEngine.Random.Range(0, Map.ZDemension - 1);

        for (int i = 0; i < Map.XDemension * Map.ZDemension; i++)
        {
            if (map.map[x, level, z] == Map.ObjectElementType.Floor)
            {
                return new Vector3(x, level, z);
            }
            x += 1;
            if (x >= Map.XDemension)
            {
                x = 0;
                z += 1;
                if (z >= Map.ZDemension)
                {
                    z = 0;
                }
            }
        }
        return Vector3.zero;
    }

    private void FillFloorList()
    {
        floorPoints = new List<Point2D>[Map.YDemension];
        floorIndexes = new int[Map.YDemension];

        for (int i = 0; i < floorPoints.GetLength(0); i++)
        {
            floorPoints[i] = new List<Point2D>();
        }

        for (int x = 0; x < Map.XDemension / 2; x++)
            for (int y = 0; y < Map.YDemension; y++)
                for (int z = 0; z < Map.ZDemension / 2; z++)
                {
                    floorPoints[y].Add(new Point2D(x * 2 + 1, z * 2 + 1));                    
                }

        for (int l = 0; l < Map.YDemension; l++)
            for (int i = 0; i < floorPoints[l].Count; i++)
            {
                int index = UnityEngine.Random.Range(i, floorPoints[l].Count - 1);

                Point2D temp = floorPoints[l][i];
                floorPoints[l][i] = floorPoints[l][index];
                floorPoints[l][index] = temp;
            }
    }

    private void MarkHoles()
    {
        for (int l = 0; l < Map.YDemension; l++)
        for (int i = 0; i < Map.WholesCount / (Map.YDemension - l); i++)
        {
            Point2D posPoint = floorPoints[l][floorIndexes[l]];
            floorIndexes[l]++;
            map.map[Convert.ToInt32(posPoint.X), l, Convert.ToInt32(posPoint.Y)] = Map.ObjectElementType.Void;
        }
    }

    private void CreateBonuses()
    {
        if (isServer)
        {
            for (int j = 0; j < 3; j++)
                for (int l = 0; l < Map.YDemension; l++)
                    for (int i = 0; i < Map.BonusesCount; i++)
                    {
                        Point2D posPoint = floorPoints[l][floorIndexes[l]];
                        floorIndexes[l]++;
                        Vector3 bonusPos = new Vector3(posPoint.X * Map.ScaleXZ, l * Map.LevelHeight + Map.ScaleFloorY + 2.5F, posPoint.Y * Map.ScaleXZ);
                        GameObject bonus = GameObject.Instantiate(Resources.Load("Bonus" + j)) as GameObject;
                        bonus.transform.position = bonusPos;
                        NetworkServer.Spawn(bonus);
                        bonuses.Add(bonus);
                    }
        }
    }

    private void ClearBonuses()
    {
        if (isServer)
        {
            for (int i = 0; i < bonuses.Count; i++)
            {
                NetworkServer.Destroy(bonuses[i]);
            }
            bonuses.Clear();
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

        map.map[x, Map.YDemension - 1, y] = Map.ObjectElementType.Floor;

        Vector3 bonusPos = new Vector3(x * Map.ScaleXZ, (Map.YDemension - 1) * Map.LevelHeight + Map.ScaleFloorY + 2.5F, y * Map.ScaleXZ);
        GameObject bonus = GameObject.Instantiate(Resources.Load("ExitBonus")) as GameObject;
        bonus.transform.position = bonusPos;
        NetworkServer.Spawn(bonus);
    }

    private void CreateTeleports()
    {
        if (isServer)
        {
            for (int l = 0; l < Map.YDemension - 1; l++)
                for (int i = 0; i < Map.EndsCount; i++)
                {
                    Point2D posPoint = floorPoints[l][floorIndexes[l]];
                    floorIndexes[l]++;
                    map.map[posPoint.X, l, posPoint.Y] = Map.ObjectElementType.Teleport;
                    map.map[posPoint.X, l + 1, posPoint.Y] = Map.ObjectElementType.TeleportationPlace;
                    for (int k = 0; k < floorPoints[l + 1].Count; k++)
                        if (floorPoints[l + 1][k].X == posPoint.X && floorPoints[l + 1][k].Y == posPoint.Y)
                            floorPoints[l + 1].RemoveAt(k);

                    GameObject teleport = GameObject.Instantiate(Resources.Load("Teleport")) as GameObject;
                    teleport.transform.position = new Vector3(Map.ScaleXZ * posPoint.X, Map.LevelHeight * l + Map.ScaleFloorY, Map.ScaleXZ * posPoint.Y);
                    NetworkServer.Spawn(teleport);
                    teleports.Add(teleport);
                }
        }
    }

    private void ClearTeleports()
    {
        if (isServer)
        {
            for (int i = 0; i < teleports.Count; i++)
            {
                NetworkServer.Destroy(teleports[i]);
            }
            teleports.Clear();
        }
    }
}
