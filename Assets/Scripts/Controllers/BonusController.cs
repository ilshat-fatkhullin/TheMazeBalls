using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BonusController : NetworkBehaviour {

    public enum BonusType { Health, Armor, Exp, Exit }
    public BonusType bonusType;

    float startTime;
    const float Delay = 60;
    bool off = false;
    MapGenerator mapGenerator;

    void Start()
    {
        gameObject.transform.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0));
        if (isServer)
        mapGenerator = GameObject.Find("SceneManager").GetComponent<MapGenerator>();
    }

    [ClientRpc]
    void RpcMakeOff()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    [ClientRpc]
    void RpcMakeOn()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    void Update()
    {
        transform.Rotate(Vector3.forward, -180 * Time.deltaTime);

        if (isServer && off)
        if (Time.time - startTime > Delay)
        {
            RpcMakeOn();
        }
    }

	void OnTriggerEnter (Collider col) {
        if (isServer)
        {
            if (col.tag == "Player" || col.tag == "AI")
            {
                switch (bonusType)
                {
                    case BonusType.Health:
                        col.GetComponent<Health>().HP = Health.MAXHP;
                        break;
                    case BonusType.Armor:
                        col.GetComponent<Health>().ARMOR = Health.MAXARMOR;
                        break;
                    case BonusType.Exp:
                        col.GetComponent<Exp>().exp += 10;
                        break;
                    case BonusType.Exit:
                        col.GetComponent<Exp>().exp += 100;
                        col.GetComponent<Respawner>().Respawn();
                        break;
                }
            }

            if (bonusType != BonusType.Exit)
            {
                off = true;
                startTime = Time.time;
                RpcMakeOff();
            }
        }
	}
}
