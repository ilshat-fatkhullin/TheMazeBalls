using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AIFlashlightController : NetworkBehaviour
{
    bool isFlashlightEnabled = true;
    Light flashlight;
    const float UpdateTime = 60;
    public float delayTime = 0;

    void Start()
    {
        flashlight = gameObject.GetComponentInChildren<Light>();
    }

    [ClientRpc]
    void RpcChangeBool(bool val)
    {
        if (!isServer)
        isFlashlightEnabled = val;
    }

    void Update()
    {
        if (isServer)
        {
            if (Time.time - delayTime > UpdateTime)
            {
                delayTime += UpdateTime;
                if (Random.Range(0, 2) == 0)
                {
                    isFlashlightEnabled = !isFlashlightEnabled;
                    RpcChangeBool(isFlashlightEnabled);
                }
            }
        }
        flashlight.enabled = isFlashlightEnabled;
    }
}
