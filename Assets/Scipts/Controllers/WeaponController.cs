using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeaponController : NetworkBehaviour {
    Weapon[] weapons;
    Health hp;
    UserInterface userInterface;
    public int weaponIndex = 0;

	void Start () {
        weapons = gameObject.GetComponentsInChildren<Weapon>();
        userInterface = GameObject.Find("UserInterface").GetComponent<UserInterface>();
        hp = gameObject.GetComponent<Health>();

        for (int i = 0; i < weapons.GetLength(0); i++)
        {
            weapons[i].gameObject.SetActive(false);
        }
        weapons[weaponIndex].gameObject.SetActive(true);
    }

    void ChangeWeapon()
    {
        if (userInterface.Number <= weapons.GetLength(0))
        {
            weaponIndex = userInterface.Number - 1;
        }

        for (int i = 0; i < weapons.GetLength(0); i++)
        {
            weapons[i].weapon.gameObject.SetActive(false);
        }
        weapons[weaponIndex].weapon.gameObject.SetActive(true);
    }
	
	void Update () {
        if (isLocalPlayer)
        {
            if (userInterface.Num)
            {
                ChangeWeapon();
                if (!isServer)
                {
                    CmdUpdateWeaponIndex(weaponIndex);
                }
                else
                {
                    RpcUpdateWeaponIndex(weaponIndex);
                }
            }
        }
        weapons[weaponIndex].isEnabled = userInterface.Fire > 0;
        weapons[weaponIndex].isBlock = userInterface.Block > 0;
        if (!weapons[weaponIndex].isFireArm)
            hp.Block = userInterface.Block > 0;
    }

    [ClientRpc]
    void RpcUpdateWeaponIndex(int newWeaponIndex)
    {
        weaponIndex = newWeaponIndex;
        ChangeWeapon();
    }

    [Command]
    void CmdUpdateWeaponIndex(int newWeaponIndex)
    {
        weaponIndex = newWeaponIndex;
        ChangeWeapon();
        RpcUpdateWeaponIndex(newWeaponIndex);
    }
}
