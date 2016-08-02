using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MovementController : NetworkBehaviour {

    [SyncVar]
    public bool isParalysis = false;
    Rigidbody _rigidbody;
    float speed = 120, jumpForce = 60, k_GroundRayLength = 3, maxSpeed = 30;
    bool grounded;
    public bool jump;
    public Vector3 movement = Vector3.zero;
    UserInterface userInterface;

    void Start () {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        userInterface = GameObject.Find("UserInterface").GetComponent<UserInterface>();
    }

    void FixedUpdate()
    {
        if (isLocalPlayer && !isParalysis)
        {
            grounded = Physics.Raycast(transform.position, -Vector3.up, k_GroundRayLength);
            if (jump && grounded)
            {
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
                _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            _rigidbody.AddForce(movement * speed * Time.deltaTime * 60, ForceMode.Force);
            Vector2 dir = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z);
            if (dir.magnitude > maxSpeed)
            {
                dir = dir.normalized * maxSpeed;
                _rigidbody.velocity = new Vector3(dir.x, _rigidbody.velocity.y, dir.y);
            }
            jump = false;
        }
    }
	
	void Update () {
        if (isLocalPlayer)
        {
            float h = userInterface.Horizontal;
            float v = userInterface.Vertical;
            jump = userInterface.Jump;

            Vector3 m_CamForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1));
            movement = (v * m_CamForward + h * transform.right).normalized;
        }
    }
}
