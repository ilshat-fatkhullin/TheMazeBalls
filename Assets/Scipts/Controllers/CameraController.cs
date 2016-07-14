using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CameraController : NetworkBehaviour {

    UserInterface userInterface;
	void Start () {
        userInterface = GameObject.Find("UserInterface").GetComponent<UserInterface>();
	}

    public float mouseSensitivity = 200, clampAngle = 60;
    float rotY, rotX;
	void Update () {
        if (isLocalPlayer)
        {
            float mouseX = userInterface.MouseX;
            float mouseY = -userInterface.MouseY;

            rotY += mouseX * mouseSensitivity * Time.deltaTime;
            rotX += mouseY * mouseSensitivity * Time.deltaTime;

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

            Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
            transform.rotation = localRotation;
        }
    }
}
