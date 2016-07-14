using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FlagsSynchronizer : NetworkBehaviour {

    public int flagIndex;

    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load(@"Flags\" + flagIndex) as Texture;
    }

    public void UpdateFlagTexture()
    {
        RpcSetFlagTexture(flagIndex);
    }

    [ClientRpc]
    void RpcSetFlagTexture(int newFlagIndex)
    {
        flagIndex = newFlagIndex;
        gameObject.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load(@"Flags\" + newFlagIndex) as Texture;
    }
}
