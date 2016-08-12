using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FireArm : Weapon {

    public float FireDelay = 0.15F;
    public int Damage = 20;
    public float Accuracy = 0;
    float lastShootTime;
    bool isAI;
    public Transform firePoint;
    SphereCollider coll;
    AudioSource source;
	void Start () {
        isFireArm = true;
        isAI = gameObject.tag == "AI";
        source = weapon.GetComponentInChildren<AudioSource>();
        coll = gameObject.GetComponent<SphereCollider>();
    }

    [Command(channel = 0)]
    void CmdShoot(Vector3 dir, Vector3 pos)
    {
        Shoot(dir, pos);
    }

    void Shoot(Vector3 direction, Vector3 position)
    {
        source.Play();
        RaycastHit hit = new RaycastHit();
        coll.enabled = false;
        Physics.Raycast(position, direction + Vector3.right * Random.Range(-Accuracy, Accuracy) + Vector3.up * Random.Range(-Accuracy, Accuracy), out hit, 500);
        coll.enabled = true;
        if (hit.point != Vector3.zero)
        {
            CreateParticle(hit.point);
            RpcCreateParticle(hit.point);
            if (hit.collider.GetComponent<Health>() != null)
                hit.collider.GetComponent<Health>().Damage(Damage, transform.position, transform);
        }
    }

    [ClientRpc(channel = 0)]
    void RpcCreateParticle(Vector3 point)
    {
        if (isClient)
        {
            CreateParticle(point);
        }
    }

    void CreateParticle(Vector3 point)
    {
        GameObject bullet = GameObject.Instantiate(Resources.Load("Flare")) as GameObject;
        bullet.transform.position = point;
    }

    [Command(channel = 0)]
    void CmdPlay()
    {
        RpcPlay();
    }

    [ClientRpc(channel = 0)]
    void RpcPlay()
    {
        if (isClient && !isLocalPlayer)
        {
            if (source != null)
            source.Play();
        }
    }

    void Update () {
        if ((isLocalPlayer && !isAI) || (isServer && isAI))
        if (isEnabled)
        {
            if (Time.time - lastShootTime > FireDelay)
            {
                source.Play();
                lastShootTime = Time.time;
                if (isServer)
                {
                    Shoot(transform.forward, firePoint.position);
                    RpcPlay();
                }
                else
                {
                    CmdShoot(transform.forward, firePoint.position);
                    CmdPlay();
                }
            }
        }
	}
}
