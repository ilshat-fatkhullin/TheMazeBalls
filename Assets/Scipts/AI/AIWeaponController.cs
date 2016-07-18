using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AIWeaponController : NetworkBehaviour {

    Weapon[] weapons;
    [SyncVar]
    public int weaponIndex = 0;
    [SyncVar]
    public bool isEnabled = false, isBlock = false;
    int lastIndex = -1;

    void Start () {
        weapons = gameObject.GetComponentsInChildren<Weapon>();
    }

    void OnChanged(int val)
    {

    }
	
	void Update () {
        if (lastIndex != weaponIndex)
        {
            for (int i = 0; i < weapons.GetLength(0); i++)
            {
                weapons[i].weapon.gameObject.SetActive(false);
            }
            weapons[weaponIndex].weapon.gameObject.SetActive(true);
            lastIndex = weaponIndex;
        }

        weapons[weaponIndex].isEnabled = isEnabled;
        weapons[weaponIndex].isBlock = isBlock;
    }
}
