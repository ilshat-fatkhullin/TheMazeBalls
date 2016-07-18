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

    [Command(channel = 0)]
    public void CmdUpdatePosDamp(float x, float y, float z)
    {
        if (isServer)
        {
            currentPos = new Vector3(x, y, z);
            RpcUpdatePosDamp(x, y, z);
        }
    }

    [Command(channel = 0)]
    public void CmdUpdateRotationDamp(float w, float x, float y, float z)
    {
        if (isServer)
        {
            currentRot = new Quaternion(x, y, z, w);
            RpcUpdateRotationDamp(w, x, y, z);
        }
    }

    [ClientRpc(channel = 0)]
    public void RpcUpdatePosDamp(float x, float y, float z)
    {
        if (!isLocalPlayer && isClient && !isServer)
        {
            if (_rigidbody != null)
                currentPos = new Vector3(x, y, z);
        }
    }

    [ClientRpc(channel = 0)]
    public void RpcUpdateRotationDamp(float w, float x, float y, float z)
    {
        if (!isLocalPlayer && isClient && !isServer)
        {
            currentRot = new Quaternion(x, y, z, w);
        }
    }

    [ClientRpc(channel = 0)]
    public void RpcUpdatePos(Vector3 pos)
    {
        if (isLocalPlayer)
        {
            gameObject.GetComponent<Rigidbody>().position = pos;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    [ClientRpc(channel = 0)]
    public void RpcAddForce(Vector3 force)
    {
        if (isLocalPlayer)
        {
            gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        }
    }

    void BlaBla()
    {
        if (!isAI)
        {
            if (isLocalPlayer)
            {
                if (isServer)
                {
                    RpcUpdatePosDamp(_rigidbody.position.x, _rigidbody.position.y, _rigidbody.position.z);
                    RpcUpdateRotationDamp(transform.rotation.w, transform.rotation.x, transform.rotation.y, transform.rotation.z);
                }
                else
                {
                    CmdUpdatePosDamp(_rigidbody.position.x, _rigidbody.position.y, _rigidbody.position.z);
                    CmdUpdateRotationDamp(transform.rotation.w, transform.rotation.x, transform.rotation.y, transform.rotation.z);
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
                RpcUpdatePosDamp(_rigidbody.position.x, _rigidbody.position.y, _rigidbody.position.z);
                RpcUpdateRotationDamp(transform.rotation.w, transform.rotation.x, transform.rotation.y, transform.rotation.z);
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
