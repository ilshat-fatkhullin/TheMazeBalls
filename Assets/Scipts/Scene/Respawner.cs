using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Respawner : NetworkBehaviour {
    Spawner spawner;
    SynchronizeManager synchronizeManager;
    public Vector3 spawn;
    bool isAI = false;
    public int AILevel = 0;

	void Start () {
        spawner = GameObject.Find("SceneManager").GetComponent<Spawner>();
        synchronizeManager = gameObject.GetComponent<SynchronizeManager>();
        isAI = gameObject.tag == "AI";
    }
	
	void Update () {
        if (isServer)
        {
            if (transform.position.y < - 10)
            {
                gameObject.GetComponent<Health>().Death();
            }
        }
	}

    public void Respawn()
    {        
        if (!isAI)
        {
            spawn = spawner.GenerateVoidPlace(0);
            synchronizeManager.RpcUpdatePos(spawn);
            gameObject.GetComponent<Health>().HP = Health.MAXHP;
            gameObject.GetComponent<Health>().ARMOR = Health.MAXARMOR;
            gameObject.GetComponent<Health>().startTime = Time.time;
            if (gameObject.GetComponent<Exp>() != null)
            {
                gameObject.GetComponent<Exp>().exp -= 50;
                if (gameObject.GetComponent<Exp>().exp < 0)
                {
                    gameObject.GetComponent<Exp>().exp = 0;
                }
            }
        }
        else
        {
            transform.position = spawner.GenerateVoidPlace(AILevel);
        }
    }
}
