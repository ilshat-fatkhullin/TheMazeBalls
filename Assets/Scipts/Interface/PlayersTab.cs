using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayersTab : NetworkBehaviour {

    UserInterface userInterface;
    Rect[] nicknamesRect = new Rect[8];
    Rect[] expsRect = new Rect[8];
    Exp[] exps;
    PlayerExpAndNickname[] array, last_array;

    GUIStyle guiStyle = new GUIStyle();

    void Start () {
        userInterface = gameObject.GetComponent<UserInterface>();
        float pixel = Screen.height / 10;
        for (int i = 0; i < nicknamesRect.GetLength(0); i++)
        {
            nicknamesRect[i] = new Rect(Screen.width - pixel * 5, pixel + pixel * i, pixel * 3, pixel);
            expsRect[i] = new Rect(Screen.width - pixel * 2, pixel + pixel * i, pixel * 2, pixel);
        }
        guiStyle.fontSize = Convert.ToInt32(pixel / 2);
        guiStyle.normal.textColor = Color.white;
    }

    private bool IsEqual(PlayerExpAndNickname[] a, PlayerExpAndNickname[] b)
    {
        if ((a == null && b != null) || (a != null) && (b == null))
        {
            return false;
        }

        if (a == null && b == null)
        {
            return true;
        }


        if (a.GetLength(0) != b.GetLength(0))
            return false;

        for (int i = 0; i < a.GetLength(0); i++)
        {
            if (a[i].exp != b[i].exp)
            {
                return false;
            }
        }

        return true;
    }
	
	void Update () {
        if (isServer)
        {            
            exps = GameObject.FindObjectsOfType<Exp>();
            array = new PlayerExpAndNickname[exps.GetLength(0)];
            for (int i = 0; i < exps.GetLength(0); i++)
            {
                array[i] = new PlayerExpAndNickname();
                array[i].exp = exps[i].exp;
                array[i].nickname = exps[i].nickname;
            }

            if (!IsEqual(array, last_array))
            {
                last_array = array;

                string output_string = "";

                for (int i = 0; i < array.GetLength(0); i++)
                {
                    output_string += array[i].nickname;
                    output_string += ':';
                    output_string += array[i].exp;
                    if (i != array.GetLength(0) - 1)
                    output_string += '|';
                }

                RpcUpdatePlayersTab(output_string);
            }      
        }
	}

    [ClientRpc(channel = 1)]
    void RpcUpdatePlayersTab(string newString)
    {
        if (!isServer)
        {
            string[] arrayString = newString.Split('|');
            array = new PlayerExpAndNickname[arrayString.GetLength(0)];
            for (int i = 0; i < array.GetLength(0); i++)
            {      
                string[] structArray = arrayString[i].Split(':');
                array[i] = new PlayerExpAndNickname();
                array[i].nickname = structArray[0];
                array[i].exp = Convert.ToInt32(structArray[1]);
            }
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

    void OnGUI()
    {
        if (userInterface.Tab)
        {
            if (array != null)
            for (int i = 0; i < array.GetLength(0) && i < nicknamesRect.GetLength(0); i++)
            {
                GUI.Label(nicknamesRect[i], array[i].nickname, guiStyle);
                GUI.Label(expsRect[i], Convert.ToString(array[i].exp), guiStyle);
            }
        }
    }
}

public class PlayerExpAndNickname
{
    public string nickname;
    public int exp;
}
