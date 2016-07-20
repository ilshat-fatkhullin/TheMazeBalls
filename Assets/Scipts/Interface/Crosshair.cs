using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {

    enum CrosshaitStyle { Red, Green, Blue };
    CrosshaitStyle crosshaitStyle = CrosshaitStyle.Red;
    public Texture redCrosshairTexture;
    public Texture greenCrosshairTexture;
    public Texture blueCrosshairTexture;
    Rect crosshair;

	void Start () {
        float pixel = Screen.height / 10;
        crosshair = new Rect(Screen.width / 2 - pixel / 2, Screen.height/ 2 - pixel / 2, pixel, pixel);
        int cType = GameObject.Find("UserInterface").GetComponent<UserInterface>().crosshairColor;
        if (cType == 0)
        {
            crosshaitStyle = CrosshaitStyle.Red;
        }
        if (cType == 1)
        {
            crosshaitStyle = CrosshaitStyle.Green;
        }
        if (cType == 2)
        {
            crosshaitStyle = CrosshaitStyle.Blue;
        }
    }

    void OnGUI()
    {
        switch (crosshaitStyle)
        {
            case CrosshaitStyle.Red:
                GUI.DrawTexture(crosshair, redCrosshairTexture);
                break;
            case CrosshaitStyle.Green:
                GUI.DrawTexture(crosshair, greenCrosshairTexture);
                break;
            case CrosshaitStyle.Blue:
                GUI.DrawTexture(crosshair, blueCrosshairTexture);
                break;
        }   
    }
}
