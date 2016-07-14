using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Health : NetworkBehaviour {
    [SyncVar]
    public int HP = 100, ARMOR = 100;
    public const int MAXHP = 100, MAXARMOR = 100, UNTOUCHABLETIME = 3;
    public bool Block = false;
    public float startTime;
    MonoBehaviour[] scripts;
    MonoBehaviour[] child_scripts;
    Rigidbody _rigidbody;
    Transform killer;
    UserInterface userInterface;

	void Start () {
        scripts = gameObject.GetComponents<MonoBehaviour>();
        child_scripts = gameObject.GetComponentsInChildren<MonoBehaviour>();
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        startTime = Time.time;
        userInterface = GameObject.Find("UserInterface").GetComponent<UserInterface>();
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            userInterface.hp = HP;
            userInterface.armor = ARMOR;
        }
    }

    public void Damage(int damage, Vector3 pos, Transform newKiller)
    {
        if (Time.time - startTime > UNTOUCHABLETIME)
        {
            if (!isServer)
            {
                throw new Exception("Damage called by server.");
            }

            if (!(gameObject.tag == "AI"))
            {
                gameObject.GetComponent<SynchronizeManager>().RpcAddForce((((transform.position - pos).normalized * 3) + Vector3.up * 0.5F) * 60);
            }
            else
            {
                _rigidbody.AddForce((((transform.position - pos).normalized * 3) + Vector3.up * 0.5F) * 60, ForceMode.Impulse);
            }

            killer = newKiller;

            ARMOR -= damage;

            if (ARMOR < 0)
            {
                HP += ARMOR;
                ARMOR = 0;
            }
            if (HP < 0)
            {
                HP = 0;
            }
            if (HP == 0)
            {
                Death();
            }
        }      
    }

    public void Death()
    {
        if (killer != null)
        {
            if (killer.tag != "AI")
                killer.GetComponent<Exp>().exp += 100;
        }

        GameObject bang = GameObject.Instantiate(Resources.Load("Explosion")) as GameObject;
        bang.transform.position = transform.position;
        NetworkServer.Spawn(bang);
        HP = MAXHP;
        ARMOR = MAXARMOR;
        gameObject.GetComponent<Respawner>().Respawn();
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void Punch(int damage, Vector3 pos, Transform newKiller)
    {
        if (!Block)
            Damage(damage, pos, newKiller);
    }
}
