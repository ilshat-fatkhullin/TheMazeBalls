using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SynchronizeManager : NetworkBehaviour
{
    const float Damp = 60;

    Rigidbody _rigidbody;
    bool isAI = false;
    bool isParalisis = false;
    float paralisisTime;
    MovementController movementController;
    AI ai;
    Vector3 currentPos = Vector3.zero;
    Quaternion currentRot = Quaternion.identity;

    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        movementController = gameObject.GetComponent<MovementController>();
        ai = gameObject.GetComponent<AI>();
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

    public void UpdatePosition(Vector3 pos, bool setVelocityToZero)
    {
        if (isServer)
        {
            if (isLocalPlayer)
            {
                gameObject.GetComponent<Rigidbody>().position = pos;
                if (setVelocityToZero)
                {
                    gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
            }
            else
            {
                RpcUpdatePos(pos, setVelocityToZero);
            }
        }
    }

    public void AddForce(Vector3 force)
    {
        if (isServer)
        {
            if (isLocalPlayer || (isServer && isAI))
            {
                gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            }
            else
            {
                RpcAddForce(force);
            }
        }
    }

    public void SetParalysisOn()
    {
        isParalisis = true;
        paralisisTime = Time.time;
        UpdatePosition(transform.position, true);
        if (isAI)
        {
            ai.isParalysis = true;
        }
        else
        {
            movementController.isParalysis = true;
        }
    }

    public void SetParalysisOff()
    {
        isParalisis = false;
        if (isAI)
        {
            ai.isParalysis = false;
        }
        else
        {
            movementController.isParalysis = false;
        }
    }

    void Update()
    {
        if (isServer)
        {
            if (Time.time - paralisisTime > Wavegun.ParalysisTime)
            {
                SetParalysisOff();
            }
        }
    }

    [ClientRpc(channel = 0)]
    void RpcUpdatePos(Vector3 pos, bool setVelocityToZero)
    {
        if (isLocalPlayer)
        {
            gameObject.GetComponent<Rigidbody>().position = pos;
            if (setVelocityToZero)
            {
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }

    [ClientRpc(channel = 0)]
    void RpcAddForce(Vector3 force)
    {
        if (isLocalPlayer)
        {
            gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        }
    }
}
