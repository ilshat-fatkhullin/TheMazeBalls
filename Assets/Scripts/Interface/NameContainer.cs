using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class NameContainer : MonoBehaviour {
    public TextAsset text;
    string[] nameList;

    public string GetRandomName()
    {
        return nameList[Random.Range(0, nameList.GetLength(0))];
    }

	void Start () {
        nameList = Regex.Split(text.text, "\r\n|\r|\n");
    }
}
