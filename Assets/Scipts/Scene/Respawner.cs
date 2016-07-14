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
                Respawn();
            }
        }
	}

    public void Respawn()
    {        
        if (!isAI)
        {
            synchronizeManager.RpcUpdatePos(spawn);
            gameObject.GetComponent<Health>().HP = gameObject.GetComponent<Health>().MAXHP;
            gameObject.GetComponent<Health>().ARMOR = gameObject.GetComponent<Health>().MAXARMOR;
            gameObject.GetComponent<Health>().RpcUpdateVars(gameObject.GetComponent<Health>().HP, gameObject.GetComponent<Health>().ARMOR);
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
