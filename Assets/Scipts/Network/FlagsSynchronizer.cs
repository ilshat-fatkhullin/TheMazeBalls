using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FlagsSynchronizer : NetworkBehaviour {

    public int flagIndex;

	void Start () {
        gameObject.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load(@"Flags\" + flagIndex) as Texture;
        if (isLocalPlayer)
        {            
            RpcSetFlagTexture(flagIndex);
        }
	}

    [Command]
    void CmdSetFlagTexture()
    {
        RpcSetFlagTexture(flagIndex);
    }

    [ClientRpc]
    void RpcSetFlagTexture(int newFlagIndex)
    {
        gameObject.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load(@"Flags\" + newFlagIndex) as Texture;
    }
}
