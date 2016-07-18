using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayersTab : NetworkBehaviour {

    UserInterface userInterface;
    Exp[] exps;

    GUIStyle guiStyle = new GUIStyle();

    void Start()
    {
        userInterface = GameObject.Find("UserInterface").GetComponent<UserInterface>();
    }

    float lastUpdateTime;
	
	void Update () {
        if (Time.time - lastUpdateTime > 1)
        {
            lastUpdateTime = Time.time;
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            exps = new Exp[players.GetLength(0)];
            for (int i = 0; i < exps.GetLength(0); i++)
            {
                exps[i] = players[i].GetComponent<Exp>();
            }
            userInterface.exps = exps;
        }
	}

    void QuickSort(ref Exp[] array, int a, int b)
    {
        int A = a;
        int B = b;
        int mid;
        if (b > a)
        {
            mid = array[(a + b) / 2].exp;

            while (A <= B)

            {

                while ((A < b) && (array[A].exp < mid)) ++A;

                while ((B > a) && (array[B].exp > mid)) --B;

                if (A <= B)

                {

                    int T;

                    T = array[A].exp;

                    array[A] = array[B];

                    array[B].exp = T;



                    ++A;

                    --B;

                }
            }

            if (a < B) QuickSort(ref array, a, B);

            if (A < b) QuickSort(ref array, A, b);
        }

    }

    void QuickSort(ref Exp[] array, int n)
    {
        QuickSort(ref array, 0, n - 1);
    }
}
