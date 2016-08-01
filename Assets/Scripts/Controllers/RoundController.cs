using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class RoundController : NetworkBehaviour {

    const int MaxTime = 300;

    [SyncVar]
    public int Time = MaxTime;
    [SyncVar]
    public bool IsRoundEnd = false;
    float startTime;

    MapGenerator mapGenerator;

    void Start()
    {
        if (isServer)
        {
            startTime = UnityEngine.Time.time;
        }

        mapGenerator = gameObject.GetComponent<MapGenerator>();
    }
	
	void Update () {
        if (isServer)
        {
            Time = MaxTime - (int)(UnityEngine.Time.time - startTime);
            if (!IsRoundEnd)
            {
                if (Time <= 0)
                {
                    RoundEnd();
                }
            }
            else
            {
                if (Time <= -5)
                {
                    Reload();
                }
            }
        }
	}

    void RoundEnd()
    {
        IsRoundEnd = true;
    }

    void Reload()
    {
        IsRoundEnd = false;
        startTime = UnityEngine.Time.time;
        mapGenerator.GenerateAsServer();
        Respawner[] respawners = GameObject.FindObjectsOfType<Respawner>();
        for (int i = 0; i < respawners.GetLength(0); i++)
        {
            respawners[i].Respawn();
        }
        Exp[] exps = GameObject.FindObjectsOfType<Exp>();
        for (int i = 0; i < exps.GetLength(0); i++)
        {
            exps[i].exp = 0;
        }
    }
}
