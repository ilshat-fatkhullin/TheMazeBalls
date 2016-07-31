using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map {
    public const int XDemension = 21, YDemension = 3, ZDemension = 21,
                     ScaleXZ = 20, ScaleFloorY = 5, LevelHeight = 30,
                     WholesCount = (XDemension * YDemension) / 2, EndsCount = (XDemension * ZDemension) / 50, BonusesCount = (XDemension * YDemension) / 10;
    public enum ObjectElementType { Void, Floor, Wall, Start, End};
    public enum UnitElementType { Void, Unit };
    public ObjectElementType[,,]  map = new ObjectElementType[XDemension, YDemension, ZDemension];
    public UnitElementType[,,] unitMap = new UnitElementType[XDemension, YDemension, ZDemension];
    GameObject[,,] mapObjects = new GameObject[XDemension, YDemension, ZDemension];

    public Map()
    {

    }

    public void CreateMap()
    {
        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
                for (int k = 0; k < map.GetLength(2); k++)
                {
                    CreateMapObject(i, j, k);
                }
        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(2); j++)
            {
                CreateUpside(i, j);
            }
    }

    public void ClearMap()
    {
        for (int i = 0; i < Map.XDemension; i++)
            for (int j = 0; j < Map.YDemension; j++)
                for (int k = 0; k < Map.ZDemension; k++)
                {
                    map[i, j, k] = ObjectElementType.Void;
                    GameObject.Destroy(mapObjects[i, j, k]);
                    mapObjects[i, j, k] = null;
                }
    }

    public void CreateUpside(int i, int j)
    {
        GameObject upside = GameObject.Instantiate(Resources.Load("Glass")) as GameObject;

        upside.transform.localScale = new Vector3(ScaleXZ, ScaleFloorY, ScaleXZ);
        upside.transform.position = new Vector3(ScaleXZ * i, (LevelHeight * Map.YDemension) + (ScaleFloorY / 2), ScaleXZ * j);
    }

    public void CreateMapObject(int i, int j, int k)
    {
        int sizeY = ScaleFloorY;
        switch (map[i, j, k])
        {
            case ObjectElementType.Floor:
                mapObjects[i, j, k] = GameObject.Instantiate(Resources.Load("Floor" + j)) as GameObject;
                break;
            case ObjectElementType.Wall:
                mapObjects[i, j, k] = GameObject.Instantiate(Resources.Load("Wall" + j)) as GameObject;
                sizeY = LevelHeight;
                break;
            case ObjectElementType.Start:
                mapObjects[i, j, k] = GameObject.Instantiate(Resources.Load("Floor" + j)) as GameObject;
                break;
            case ObjectElementType.End:
                mapObjects[i, j, k] = GameObject.Instantiate(Resources.Load("Floor" + j)) as GameObject;
                break;
        }

        if (mapObjects[i, j, k] != null)
        {
            mapObjects[i, j, k].transform.localScale = new Vector3(ScaleXZ, sizeY, ScaleXZ);
            mapObjects[i, j, k].transform.position = new Vector3(ScaleXZ * i, (LevelHeight * j) + (sizeY / 2), ScaleXZ * k);
        }
    }
}
