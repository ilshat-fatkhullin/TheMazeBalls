using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Health : NetworkBehaviour {

    public float HP = 100, ARMOR = 100, MAXHP = 100, MAXARMOR = 100;
    public bool Block = false;
    MonoBehaviour[] scripts;
    MonoBehaviour[] child_scripts;
    Rigidbody _rigidbody;
    Rect hp, armor;
    Transform killer;

	void Start () {
        scripts = gameObject.GetComponents<MonoBehaviour>();
        child_scripts = gameObject.GetComponentsInChildren<MonoBehaviour>();
        _rigidbody = gameObject.GetComponent<Rigidbody>();

        float pixel = Screen.height / 20;
        hp = new Rect(pixel, Screen.height - pixel * 4, pixel * 4, pixel);
        armor = new Rect(pixel, Screen.height - pixel * 2, pixel * 4, pixel);
        guiStyle.fontSize = Convert.ToInt32(pixel);
        guiStyle.normal.textColor = Color.white;
    }

    public void Damage(float damage, Vector3 pos, Transform newKiller)
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

        RpcUpdateVars(HP, ARMOR);
    }

    private GUIStyle guiStyle = new GUIStyle();

    void OnGUI()
    {
        if (isLocalPlayer)
        {
            GUI.contentColor = Color.white;
            GUI.Label(hp, "HP: " + Convert.ToInt32(HP), guiStyle);
            GUI.Label(armor, "ARMOR: " + Convert.ToInt32(ARMOR), guiStyle);
        }
    }

    void Death()
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

    [ClientRpc(channel = 0)]
    public void RpcUpdateVars(float in_hp, float in_armor)
    {
        if (!isServer)
        {
            HP = in_hp;
            ARMOR = in_armor;
        }
    }

    public void Punch(float damage, Vector3 pos, Transform newKiller)
    {
        if (!Block)
            Damage(damage, pos, newKiller);
    }
}
