using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlagManager {
    public static void UpdateFlags()
    {
        FlagsSynchronizer[] scriptsArray = GameObject.FindObjectsOfType<FlagsSynchronizer>();

        for (int i = 0; i < scriptsArray.GetLength(0); i++)
        {
            scriptsArray[i].UpdateFlagTexture();
        }
    }
}
