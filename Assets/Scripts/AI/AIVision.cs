using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Algorithms;

public class AIVision : NetworkBehaviour {

    MapGenerator mapGenerator;
    GameObject[] units;
    Vector3[] points;
    float lastUpdateTime;
    const float Delay = 1;

	void Start () {
        if (isServer)
        {
            mapGenerator = gameObject.GetComponent<MapGenerator>();
            UpdateUnits();
        }
	}

    public void UpdateUnits()
    {
        GameObject[] ais = GameObject.FindGameObjectsWithTag("AI");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        units = new GameObject[ais.GetLength(0) + players.GetLength(0)];
        for (int i = 0; i < ais.GetLength(0) + players.GetLength(0); i++)
        {
            if (i < ais.GetLength(0))
            {
                units[i] = ais[i];
            }
            else
            {
                units[i] = players[i - ais.GetLength(0)];
            }
        }

        points = new Vector3[units.GetLength(0)];
        for (int i = 0; i < units.GetLength(0); i++)
        {
            points[i] = MazeNavigator.GetClosestPoint(units[i].transform.position);
        }
    }
	
	void Update () {
        if (isServer && Time.time - lastUpdateTime > Delay)
        {
            UpdateUnits();

            for (int i = 0; i < units.GetLength(0); i++)
            {
                float distance = float.MaxValue;
                if (units[i].tag != "Player")
                    for (int j = 0; j < units.GetLength(0); j++)
                    {
                        Vector2 distVector = new Vector2(points[i].x - points[j].x, points[i].z - points[j].z);
                        if (distVector.magnitude < distance)
                            if (!(points[i].y == points[j].y && points[i].x == points[j].x && points[i].z == points[j].z))
                                if (points[i].y == points[j].y && (points[i].x == points[j].x || points[i].z == points[j].z))
                                {
                                    distance = distVector.magnitude;
                                    units[i].GetComponent<AI>().enemie = units[j].transform;
                                    units[i].GetComponent<AI>().lastUpdateTime = Time.time;
                                }
                    }
            }
        }
	}
}
