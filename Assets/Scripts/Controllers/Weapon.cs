using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Weapon : NetworkBehaviour {
    public bool isEnabled = false, isBlock = false, isFireArm = false;
    public Transform weapon;
}
