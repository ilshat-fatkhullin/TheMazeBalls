using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SceneStyleController : NetworkBehaviour {

    [ClientRpc]
    void RpcChangeScene(bool val)
    {
        if (val)
            MakeDarkness();
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        RpcChangeScene(DataManager.LoadBool());
    }

    void Start()
    {
        if (isServer)
        if (DataManager.LoadBool())
            MakeDarkness();
    }

    void MakeDarkness()
    {
        RenderSettings.ambientIntensity = 0.1F;
    }
}
