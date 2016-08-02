using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Wavegun : Weapon
{
    public const float ParalysisTime = 5;
    const float Power = 100000;
    const float FireDelay = 10;
    const float Angle = 30;
    const int EnemyLayer = 1 << 8;
    public const int PointRadius = 30;
    public const int PersonRadius = 120;
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

    void Shoot(Vector3 direction, Vector3 position, bool isParalise)
    {
        source.Play();
        RaycastHit hit = new RaycastHit();
        coll.enabled = false;
        Physics.Raycast(position, direction + Vector3.right * Random.Range(-Accuracy, Accuracy) + Vector3.up * Random.Range(-Accuracy, Accuracy), out hit, 500);
        coll.enabled = true;
        if (hit.point != Vector3.zero)
        {
            GameObject wave;
            if (isParalise)
            {
                wave = GameObject.Instantiate(Resources.Load("WaveExplosion")) as GameObject;
                wave.transform.position = hit.point;
            }
            else
            {
                wave = GameObject.Instantiate(Resources.Load("Afterburner")) as GameObject;
                wave.transform.position = weapon.transform.position;
                wave.transform.rotation = Quaternion.LookRotation(transform.forward);
            }
            NetworkServer.Spawn(wave);

            Collider[] units;
            if (isParalise)
            {
                units = Physics.OverlapSphere(hit.point, PointRadius, EnemyLayer);
            }
            else
            {
                units = Physics.OverlapSphere(transform.position, PersonRadius, EnemyLayer);
            }

            foreach (Collider unit in units)
            {
                if (isParalise)
                {
                    unit.GetComponent<SynchronizeManager>().SetParalysisOn();
                }
                else
                {
                    if (Vector3.Angle(transform.forward, unit.transform.position - transform.position) <= Angle)
                    unit.GetComponent<SynchronizeManager>().AddForce((transform.forward + transform.up * 0.25F) * Power);
                }
            }
        }
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
