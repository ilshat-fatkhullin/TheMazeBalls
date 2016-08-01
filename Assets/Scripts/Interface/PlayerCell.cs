using UnityEngine;
using System.Collections;

public class PlayerCell {

    public string Nickname;
    public int Exp;
    public bool IsLocalPlayer;

    public PlayerCell(string nickname, int exp, bool isLocalPlayer)
    {
        Nickname = nickname;
        Exp = exp;
        IsLocalPlayer = isLocalPlayer;
    }
}
