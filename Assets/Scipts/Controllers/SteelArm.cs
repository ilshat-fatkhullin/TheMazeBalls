using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SteelArm : Weapon {

    enum PunchStatus { Start, End };
    PunchStatus punchStatus;
    float punchStartDelay = 0.2F, punchEndDelay = 0.4F, punchPeriodStartTime, damage = 20;
    int enemiesLayer = 1 << 8;
    Quaternion normalRotation, punchRotation, blockRotation;
    NetworkIdentity parentIdentity;
    void Start () {
        normalRotation = transform.localRotation;
        punchRotation = Quaternion.LookRotation(Vector3.down + transform.forward);
        blockRotation = Quaternion.LookRotation(Vector3.down + Vector3.left);
        parentIdentity = gameObject.transform.parent.GetComponent<NetworkIdentity>();
    }

    void Awake()
    {
        weapon = gameObject.transform;
    }

    void Update() {
        if (isBlock)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, blockRotation, Time.deltaTime * 20);
        }
        else if (isEnabled)
        {
            switch (punchStatus)
            {
            case PunchStatus.Start:
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, punchRotation, Time.deltaTime * 20);
                    if (Time.time - punchPeriodStartTime > punchStartDelay)
                    {
                        punchPeriodStartTime = Time.time;
                        punchStatus = PunchStatus.End;
                        if (parentIdentity.isServer)
                        {
                            Punch();
                        }
                    }
                break;
            case PunchStatus.End:
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, normalRotation, Time.deltaTime * 10);
                    if (Time.time - punchPeriodStartTime > punchEndDelay)
                    {
                        punchPeriodStartTime = Time.time;
                        punchStatus = PunchStatus.Start;
                    }
                break;
            }
        }
        else
        {
            punchStatus = PunchStatus.Start;
            punchPeriodStartTime = Time.time;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, normalRotation, Time.deltaTime * 20);
        }
	}

    void Punch()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 5, enemiesLayer);

        foreach (Collider col in cols)
        {
            if (col.GetComponent<Health>() != null && col.transform != gameObject.transform.parent)
            {
                if (Vector3.Angle(transform.forward, col.transform.position - transform.parent.position) < 120)
                col.GetComponent<Health>().Punch(damage, transform.position, transform);
            }
        }
    }
}
