using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {

    public Texture crosshairTexture;
    Rect crosshair;

	void Start () {
        float pixel = Screen.height / 10;
        crosshair = new Rect(Screen.width / 2 - pixel / 2, Screen.height/ 2 - pixel / 2, pixel, pixel);
    }

    void OnGUI()
    {
        GUI.DrawTexture(crosshair, crosshairTexture);
    }
}
