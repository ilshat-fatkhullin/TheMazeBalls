using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AI : NetworkBehaviour {
    MazeNavigator mazeNavigator;
    int enemiesLayer = (1 << 8);
    float punchDistance = 6, fireDistance = 50, speed = 60, detectionRadius = 50, maxSpeed = 20;
    Rigidbody _rigidbody;
    Transform enemie;
    AIWeaponController weaponController;
    const float Delay = 1;

	void Start () {
        if (isServer)
        {
            _rigidbody = gameObject.GetComponent<Rigidbody>();
            mazeNavigator = gameObject.GetComponent<MazeNavigator>();
            weaponController = gameObject.GetComponent<AIWeaponController>();
        }
    }

    void FixedUpdate()
    {
        if (isServer)
        {
            _rigidbody.AddForce(mazeNavigator.desiredVelocity * speed * 60 * Time.deltaTime, ForceMode.Force);
            Vector2 dir = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z);
            if (dir.magnitude > maxSpeed)
            {
                dir = dir.normalized * maxSpeed;
                _rigidbody.velocity = new Vector3(dir.x, _rigidbody.velocity.y, dir.y);
            }
        }
    }

    float lastUpdateTime = 0;
    void Update()
    {
        if (isServer)
        {
            if (Time.time - lastUpdateTime > Delay)
            {
                lastUpdateTime = Time.time;
                TestEnemie();
            }
            if (enemie != null)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(enemie.position - transform.position), 5 * Time.deltaTime);
                float distance = Vector3.Distance(transform.position, enemie.position);
                if (distance < punchDistance)
                {
                    Punch();
                }
                else if (distance < fireDistance)
                {
                    Fire();
                }
                else
                {
                    weaponController.isActive = false;
                    weaponController.isBlock = false;
                }
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mazeNavigator.desiredVelocity), 5 * Time.deltaTime);
            }
        }
    }

    void Punch()
    {
        weaponController.weaponIndex = 1;
        weaponController.isActive = true;
        weaponController.isBlock = false;
    }

    void Fire()
    {
        weaponController.weaponIndex = 0;
        weaponController.isActive = true;
        weaponController.isBlock = false;
    }

    void TestEnemie()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemiesLayer);

        foreach (Collider e in enemies)
        {
            if (e.tag == "Player")
            {
                if (!Physics.Linecast(transform.position, e.transform.position, ~enemiesLayer))
                {
                    enemie = e.transform;
                }
            }
        }
    }
}
