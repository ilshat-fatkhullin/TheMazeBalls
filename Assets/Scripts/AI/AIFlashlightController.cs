using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AIFlashlightController : NetworkBehaviour
{
    [SyncVar]
    bool isFlashlightEnabled = true;
    Light flashlight;
    const float UpdateTime = 60;
    public float delayTime = 0;
    SceneStyleController sceneController;

    void Start()
    {
        flashlight = gameObject.GetComponentInChildren<Light>();
        sceneController = GameObject.Find("SceneManager").GetComponent<SceneStyleController>();
        if (!sceneController.isDarkness)
        {
            isFlashlightEnabled = false;
        }
        else
        {
            isFlashlightEnabled = Random.Range(0, 3) == 0;
        }
    }

    void Update()
    {
        if (isServer)
        {
            if (Time.time - delayTime > UpdateTime)
            {
                delayTime += UpdateTime;
                if (Random.Range(0, 3) == 0)
                {
                    isFlashlightEnabled = !isFlashlightEnabled;
                }
                if (!sceneController.isDarkness)
                {
                    isFlashlightEnabled = false;
                }
            }
        }
        flashlight.enabled = isFlashlightEnabled;
    }
}
