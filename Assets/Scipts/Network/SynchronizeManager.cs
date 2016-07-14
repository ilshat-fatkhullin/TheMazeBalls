using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SynchronizeManager : NetworkBehaviour
{
    const float Damp = 60;

    WeaponController weaponController;
    AIWeaponController aiWeaponController;
    Health healthScript;
    Exp expScript;
    Rigidbody _rigidbody;
    bool isAI = false;

    Vector3 currentPos = Vector3.zero;
    Quaternion currentRot = Quaternion.identity;

    void Start()
    {
        healthScript = gameObject.GetComponent<Health>();
        weaponController = gameObject.GetComponent<WeaponController>();
        aiWeaponController = gameObject.GetComponent<AIWeaponController>();
        expScript = gameObject.GetComponent<Exp>();
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        isAI = gameObject.tag == "AI";
        if (!isAI)
        {
            if (isLocalPlayer)
            {
                gameObject.GetComponentInChildren<Camera>().tag = "MainCamera";
                gameObject.GetComponentInChildren<Camera>().enabled = true;
                gameObject.GetComponentInChildren<AudioListener>().enabled = true;
            }
            else
            {
                gameObject.GetComponentInChildren<Camera>().tag = "Untagged";
                gameObject.GetComponentInChildren<Camera>().enabled = false;
                gameObject.GetComponentInChildren<AudioListener>().enabled = false;
            }
        }
    }

    [Command]
    public void CmdUpdatePosDamp(Vector3 pos)
    {
        if (isServer)
        {
            currentPos = pos;
            RpcUpdatePosDamp(pos);
        }
    }

    [Command]
    public void CmdUpdateRotationDamp(Quaternion rotation)
    {
        if (isServer)
        {
            currentRot = rotation;
            RpcUpdateRotationDamp(rotation);
        }
    }

    [ClientRpc]
    public void RpcUpdatePosDamp(Vector3 pos)
    {
        if (!isLocalPlayer && isClient && !isServer)
        {
            if (_rigidbody != null)
                currentPos = pos;
        }
    }

    [ClientRpc]
    public void RpcUpdateRotationDamp(Quaternion rotation)
    {
        if (!isLocalPlayer && isClient && !isServer)
        {
            currentRot = rotation;
        }
    }

    [ClientRpc]
    public void RpcUpdatePos(Vector3 pos)
    {
        if (isLocalPlayer)
        {
            gameObject.GetComponent<Rigidbody>().position = pos;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    [ClientRpc]
    public void RpcAddForce(Vector3 force)
    {
        if (isLocalPlayer)
        {
            gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (!isAI)
        {
            if (isLocalPlayer)
            {
                if (isServer)
                {
                    RpcUpdatePosDamp(_rigidbody.position);
                    RpcUpdateRotationDamp(transform.rotation);
                }
                else
                {
                    CmdUpdatePosDamp(_rigidbody.position);
                    CmdUpdateRotationDamp(transform.rotation);
                }
            }
            else
            {
                if (_rigidbody != null && currentPos != Vector3.zero)
                    _rigidbody.position = Vector3.Lerp(transform.position, currentPos, Damp * Time.deltaTime);
                if (_rigidbody != null && currentRot != Quaternion.identity)
                    transform.rotation = Quaternion.Lerp(transform.rotation, currentRot, Damp * Time.deltaTime);
            }
        }
        else 
        {
            if (isServer)
            {
                RpcUpdatePosDamp(_rigidbody.position);
                RpcUpdateRotationDamp(transform.rotation);
            }
            else
            {
                if (_rigidbody != null && currentPos != Vector3.zero)
                    _rigidbody.position = Vector3.Lerp(transform.position, currentPos, Damp * Time.deltaTime);
                if (_rigidbody != null && currentRot != Quaternion.identity)
                    transform.rotation = Quaternion.Lerp(transform.rotation, currentRot, Damp * Time.deltaTime);
            }
        }
    }
}
