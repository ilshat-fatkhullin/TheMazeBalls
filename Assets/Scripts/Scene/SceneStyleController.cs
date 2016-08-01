using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SceneStyleController : NetworkBehaviour {

    public bool isDarkness = false;
    public Material darkSkybox;

    [ClientRpc]
    void RpcChangeScene(bool val)
    {
        if (val)
            MakeDarkness();
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        RpcChangeScene(DataManager.LoadBool().isDarkness);
    }

    void Start()
    {
        if (isServer)
        if (DataManager.LoadBool().isDarkness)
            MakeDarkness();
    }

    void MakeDarkness()
    {
        RenderSettings.ambientIntensity = 0.05F;
        RenderSettings.skybox = darkSkybox;
        isDarkness = true;
    }
}
