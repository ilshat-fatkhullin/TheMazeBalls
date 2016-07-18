using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map {
    //Хранение размеров карты и её создание из трёхмерного массива
    public const int XDemension = 11, YDemension = 3, ZDemension = 11,
                     ScaleXZ = 20, ScaleFloorY = 5, LevelHeight = 30, WholesCount = 1;
    public enum ElementType { Void, Floor, Wall, Start, End};
    public ElementType[,,]  map = new ElementType[XDemension, YDemension, ZDemension];
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
    }

    public void ClearMap()
    {
        for (int i = 0; i < Map.XDemension; i++)
            for (int j = 0; j < Map.YDemension; j++)
                for (int k = 0; k < Map.ZDemension; k++)
                {
                    map[i, j, k] = ElementType.Void;
                    GameObject.Destroy(mapObjects[i, j, k]);
                    mapObjects[i, j, k] = null;
                }
    }

    public void CreateMapObject(int i, int j, int k)
    {
        int sizeY = ScaleFloorY;
        switch (map[i, j, k])
        {
            case ElementType.Floor:
                mapObjects[i, j, k] = GameObject.Instantiate(Resources.Load("Floor")) as GameObject;
                break;
            case ElementType.Wall:
                mapObjects[i, j, k] = GameObject.Instantiate(Resources.Load("Wall")) as GameObject;
                sizeY = LevelHeight;
                break;
            case ElementType.Start:
                mapObjects[i, j, k] = GameObject.Instantiate(Resources.Load("Floor")) as GameObject;
                break;
            case ElementType.End:
                mapObjects[i, j, k] = GameObject.Instantiate(Resources.Load("Floor")) as GameObject;
                break;
        }

        if (mapObjects[i, j, k] != null)
        {
            mapObjects[i, j, k].transform.localScale = new Vector3(ScaleXZ, sizeY, ScaleXZ);
            mapObjects[i, j, k].transform.position = new Vector3(ScaleXZ * i, (LevelHeight * j) + (sizeY / 2), ScaleXZ * k);
        }
    }
}
