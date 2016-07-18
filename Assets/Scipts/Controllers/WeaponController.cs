using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeaponController : NetworkBehaviour {
    Weapon[] weapons;
    Health hp;
    UserInterface userInterface;
    public int weaponIndex = 0;
    bool isBlock, isEnabled;

	void Start () {
        weapons = gameObject.GetComponents<Weapon>();
        userInterface = GameObject.Find("UserInterface").GetComponent<UserInterface>();
        hp = gameObject.GetComponent<Health>();

        for (int i = 0; i < weapons.GetLength(0); i++)
        {
            weapons[i].weapon.gameObject.SetActive(false);
        }
        weapons[weaponIndex].weapon.gameObject.SetActive(true);
    }

    void ChangeWeapon(int newIndex)
    {
        if (newIndex <= weapons.GetLength(0))
        {
            weaponIndex = newIndex;
        }

        for (int i = 0; i < weapons.GetLength(0); i++)
        {
            weapons[i].weapon.gameObject.SetActive(false);
            weapons[i].isEnabled = false;
        }
        weapons[weaponIndex].weapon.gameObject.SetActive(true);
    }
	
	void Update () {
        if (isLocalPlayer)
        {
            if (userInterface.Fire > 0 != isEnabled || userInterface.Block > 0 != isBlock)
            {
                isEnabled = userInterface.Fire > 0;
                isBlock = userInterface.Block > 0;
                if (isServer)
                {
                    RpcChangeBools(isEnabled, isBlock);
                }
                else
                {
                    CmdChangeBools(isEnabled, isBlock);
                }
            }
            if (userInterface.Num)
            {
                weaponIndex = userInterface.Number - 1;
                ChangeWeapon(weaponIndex);
                if (isServer)
                {
                    RpcChangeWeaponIndex(weaponIndex);
                }
                else
                {
                    CmdChangeWeaponIndex(weaponIndex);
                }
            }
        }

        weapons[weaponIndex].isEnabled = isEnabled;
        weapons[weaponIndex].isBlock = isBlock;
        if (!weapons[weaponIndex].isFireArm)
            hp.Block = userInterface.Block > 0;
    }

    [Command]
    void CmdChangeBools(bool newIsEnabled, bool newIsBlock)
    {
        isEnabled = newIsEnabled;
        isBlock = newIsBlock;
        RpcChangeBools(newIsEnabled, newIsBlock);
    }

    [ClientRpc]
    void RpcChangeBools(bool newIsEnabled, bool newIsBlock)
    {
        if (!isLocalPlayer)
        {
            isEnabled = newIsEnabled;
            isBlock = newIsBlock;
        }
    }

    [Command]
    void CmdChangeWeaponIndex(int newWeaponIndex)
    {
        weaponIndex = newWeaponIndex;
        ChangeWeapon(weaponIndex);
        RpcChangeWeaponIndex(weaponIndex);
    }

    [ClientRpc]
    void RpcChangeWeaponIndex(int newWeaponIndex)
    {
        if (!isLocalPlayer)
        {
            weaponIndex = newWeaponIndex;
            ChangeWeapon(weaponIndex);
        }
    }
}
