using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FlashLightController : NetworkBehaviour {

    UserInterface userInterface;
    bool isFlashlightEnabled = true;
    Light flashlight;

	void Start () {
        if (isLocalPlayer)
        userInterface = GameObject.Find("UserInterface").GetComponent<UserInterface>();
        flashlight = gameObject.GetComponentInChildren<Light>();
    }

    [Command]
    void CmdChangeBool(bool val)
    {
        isFlashlightEnabled = val;
        RpcChangeBool(val);
    }

    [ClientRpc]
    void RpcChangeBool(bool val)
    {
        if (!isLocalPlayer)
        isFlashlightEnabled = val;
    }
	
	void Update () {
        if (isLocalPlayer)
        if (userInterface.Flashlight)
        {
            isFlashlightEnabled = !isFlashlightEnabled;
            if (isServer)
            {
                RpcChangeBool(isFlashlightEnabled);
            }
            else
            {
                CmdChangeBool(isFlashlightEnabled);
            }
        }
        flashlight.enabled = isFlashlightEnabled;
    }
}
