using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BonusController : NetworkBehaviour {

    public enum BonusType { Health, Armor, Exp, Exit }
    public BonusType bonusType;

    MapGenerator mapGenerator;

    void Start()
    {
        gameObject.transform.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0));
        if (isServer)
        mapGenerator = GameObject.Find("SceneManager").GetComponent<MapGenerator>();
    }

    void Update()
    {
        transform.Rotate(Vector3.forward, -180 * Time.deltaTime);
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
                        mapGenerator.UpdateBonus(gameObject, BonusType.Health);
                        break;
                    case BonusType.Armor:
                        col.GetComponent<Health>().ARMOR = Health.MAXARMOR;
                        mapGenerator.UpdateBonus(gameObject, BonusType.Armor);
                        break;
                    case BonusType.Exp:
                        if (col.tag == "Player")
                        col.GetComponent<Exp>().exp += 100;
                        mapGenerator.UpdateBonus(gameObject, BonusType.Exp);
                        break;
                    case BonusType.Exit:
                        if (col.tag == "Player")
                        {
                            col.GetComponent<Exp>().exp += 10050;
                            col.GetComponent<Respawner>().Respawn();
                        }
                        break;
                }
            }
        }
	}
}
