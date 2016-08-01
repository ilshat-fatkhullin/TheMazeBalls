using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayersTab : NetworkBehaviour {

    UserInterface userInterface;
    Exp[] exps;
    PlayerCell[] playerCells;
    GUIStyle guiStyle = new GUIStyle();
    RoundController roundController;

    [SyncVar]
    string array;
    string lastArray;

    void Start()
    {
        userInterface = GameObject.Find("UserInterface").GetComponent<UserInterface>();
        roundController = GameObject.Find("SceneManager").GetComponent<RoundController>();
    }

    public void UpdateExps()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] bots = GameObject.FindGameObjectsWithTag("AI");
        exps = new Exp[players.GetLength(0) + bots.GetLength(0)];

        for (int i = 0; i < exps.GetLength(0); i++)
        {
            if (i < players.GetLength(0))
            {
                exps[i] = players[i].GetComponent<Exp>();
            }
            else
            {
                exps[i] = bots[i - players.GetLength(0)].GetComponent<Exp>();
            }
        }
    }

    void UpdatePlayerCells()
    {
        playerCells = new PlayerCell[exps.GetLength(0)];

        for (int i = 0; i < exps.GetLength(0); i++)
        {
            if (exps[i].tag != "Player")
            {
                playerCells[i] = new PlayerCell(exps[i].nickname, exps[i].exp, false);
            }
            else
            {
                playerCells[i] = new PlayerCell(exps[i].nickname, exps[i].exp, exps[i].GetComponent<NetworkIdentity>().isLocalPlayer);
            }
        }
    }

    float lastUpdateTime;

    void Update () {
        if (Time.time - lastUpdateTime > 1)
        {
            if (isServer)
            {
                if (exps != null)
                {
                    if (!roundController.IsRoundEnd)
                    {
                        lastUpdateTime = Time.time;
                        UpdatePlayerCells();
                        QuickSort(ref playerCells, playerCells.GetLength(0));
                        Array.Reverse(playerCells);
                        userInterface.playerCells = playerCells;
                        array = GetStringFromArray(playerCells);
                    }
                }
            }
            else if (lastArray != array)
            {
                lastArray = array;
                userInterface.playerCells = GetArrayFromString(array);
            }
        }
	}

    private PlayerCell[] GetArrayFromString(string val)
    {
        string[] rows = val.Split('|');
        PlayerCell[] returnedValue = new PlayerCell[rows.GetLength(0)];
        for (int i = 0; i < rows.GetLength(0); i++)
        {
            string[] stringStruct = val.Split('%');
            returnedValue[i].Nickname = stringStruct[0];
            returnedValue[i].Exp = Convert.ToInt32(stringStruct[1]);
            returnedValue[i].IsLocalPlayer = stringStruct[2] == "1";
        }

        return returnedValue;
    }

    private string GetStringFromArray(PlayerCell[] val)
    {
        string returnedValue = "";

        for (int i = 0; i < val.GetLength(0); i++)
        {
            int boolInt = 0;
            if (val[i].IsLocalPlayer)
                boolInt = 1;

            returnedValue += val[i].Nickname + "%" + val[i].Exp + "%" + boolInt;
            if (i != val.GetLength(0) - 1)
                returnedValue += "|";
        }

        return returnedValue;
    }

    void QuickSort(ref PlayerCell[] array, int a, int b)
    {
        int A = a;
        int B = b;
        int mid;
        if (b > a)
        {
            mid = array[(a + b) / 2].Exp;

            while (A <= B)
            {
                while ((A < b) && (array[A].Exp < mid)) ++A;

                while ((B > a) && (array[B].Exp > mid)) --B;

                if (A <= B)
                {

                    PlayerCell T;

                    T = array[A];
                
                    array[A] = array[B];
                    array[B] = T;

                    ++A;
                    --B;
                }
            }

            if (a < B) QuickSort(ref array, a, B);

            if (A < b) QuickSort(ref array, A, b);
        }

    }

    void QuickSort(ref PlayerCell[] array, int n)
    {
        QuickSort(ref array, 0, n - 1);
    }
}
