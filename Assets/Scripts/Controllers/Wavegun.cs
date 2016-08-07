using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Wavegun : Weapon
{
    public const float ParalysisTime = 1;
    const float Power = 100;
    const float FireDelay = 1.5F;
    const float Angle = 25;
    const int EnemyLayer = 1 << 8;
    public const int PersonRadius = 60;
    public float Accuracy = 0;
    float lastShootTime = -FireDelay;
    bool isAI;
    public Transform firePoint;
    SphereCollider coll;
    AudioSource source;

    void Start()
    {
        isFireArm = true;
        isAI = gameObject.tag == "AI";
        source = weapon.GetComponentInChildren<AudioSource>();
        coll = gameObject.GetComponent<SphereCollider>();
    }

    [Command(channel = 0)]
    void CmdShoot(Vector3 dir, Vector3 pos, bool isParalise)
    {
        Shoot(dir, pos, isParalise);
    }

    void Shoot(Vector3 direction, Vector3 position, bool isBack)
    {
        source.Play();
        CreateParticle(isBack);
        RpcCreateParticle(isBack);

        Collider[] units;
        units = Physics.OverlapSphere(transform.position, PersonRadius, EnemyLayer);

        foreach (Collider unit in units)
        {
            if (Vector3.Angle(transform.forward, unit.transform.position - transform.position) <= Angle)
            {
                if (isBack)
                {
                    unit.GetComponent<SynchronizeManager>().AddForce(-(transform.forward + transform.up * 0.25F) * Power);
                }
                else
                {
                    unit.GetComponent<SynchronizeManager>().AddForce((transform.forward + transform.up * 0.25F) * Power);                    
                }
                unit.GetComponent<FallController>().SetPuncher(transform);
                unit.GetComponent<SynchronizeManager>().SetParalysisOn();
            }
        }
    }

    [ClientRpc(channel = 0)]
    void RpcCreateParticle(bool isBack)
    {
        if (isClient && !isLocalPlayer)
        {
            CreateParticle(isBack);
        }
    }

    void CreateParticle(bool isBack)
    {
        GameObject wave;
        if (isBack)
        {
            wave = GameObject.Instantiate(Resources.Load("ParaliseWave")) as GameObject;
        }
        else
        {
            wave = GameObject.Instantiate(Resources.Load("ForceWave")) as GameObject;
        }
        wave.transform.position = weapon.transform.position;
        wave.transform.rotation = Quaternion.LookRotation(transform.forward);
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

    void Update()
    {
        if ((isLocalPlayer && !isAI) || (isServer && isAI))
            if (isEnabled || isBlock)
            {
                if (Time.time - lastShootTime > FireDelay)
                {
                    source.Play();
                    lastShootTime = Time.time;
                    CreateParticle(isBlock);
                    if (isServer)
                    {
                        Shoot(transform.forward, firePoint.position, isBlock);
                        RpcPlay();
                    }
                    else
                    {
                        CmdShoot(transform.forward, firePoint.position, isBlock);
                        CmdPlay();
                    }
                }
            }
    }
}
