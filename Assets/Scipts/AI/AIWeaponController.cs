using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AIWeaponController : NetworkBehaviour {

    Weapon[] weapons;
    public int weaponIndex = 0;
    public bool isActive = false;
    public bool isBlock = false;
    int lastIndex = 1;

    void Start () {
        weapons = gameObject.GetComponentsInChildren<Weapon>();
    }
	
	void Update () {
        if (isServer)
        {
            if (lastIndex != weaponIndex)
            {
                for (int i = 0; i < weapons.GetLength(0); i++)
                {
                    weapons[i].weapon.gameObject.SetActive(false);
                }
                weapons[weaponIndex].weapon.gameObject.SetActive(true);
                lastIndex = weaponIndex;
                RpcUpdateWeaponIndex(weaponIndex);
            }
        }
        weapons[weaponIndex].isEnabled = isActive;
        weapons[weaponIndex].isBlock = isBlock;
    }

    [ClientRpc]
    void RpcUpdateWeaponIndex(int newWeaponIndex)
    {
        if (!isServer)
        {
            for (int i = 0; i < weapons.GetLength(0); i++)
            {
                weapons[i].weapon.gameObject.SetActive(false);
            }
            weapons[weaponIndex].weapon.gameObject.SetActive(true);
            lastIndex = weaponIndex;
        }
    }
}
