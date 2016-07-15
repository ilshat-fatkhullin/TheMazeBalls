using UnityEngine;
using System.Collections;

public class UserInterface : MonoBehaviour {

    public float MouseX, MouseY, Horizontal, Vertical, Fire, Block;
    public int Number = 1;
    public bool Jump;
    public bool Num;
    public bool Tab;
    public bool Controllable = true;

    void Update () {
        Num = false;
        if (Controllable)
        {
            MouseX = Input.GetAxis("Mouse X");
            MouseY = Input.GetAxis("Mouse Y");
            Horizontal = Input.GetAxis("Horizontal");
            Vertical = Input.GetAxis("Vertical");
            Fire = Input.GetAxis("Fire1");
            Block = Input.GetAxis("Fire2");
            Jump = Input.GetAxis("Jump") > 0;
            Tab = Input.GetKey(KeyCode.Tab);
            if (Input.GetKey(KeyCode.Alpha1))
            {
                Num = true;
                Number = 1;
            }
            if (Input.GetKey(KeyCode.Alpha2))
            {
                Num = true;
                Number = 2;
            }
            if (Input.GetKey(KeyCode.Alpha3))
            {
                Num = true;
                Number = 3;
            }
            if (Input.GetKey(KeyCode.Alpha4))
            {
                Num = true;
                Number = 4;
            }
            if (Input.GetKey(KeyCode.Alpha5))
            {
                Num = true;
                Number = 5;
            }
            if (Input.GetKey(KeyCode.Alpha6))
            {
                Num = true;
                Number = 6;
            }
            if (Input.GetKey(KeyCode.Alpha7))
            {
                Num = true;
                Number = 7;
            }
            if (Input.GetKey(KeyCode.Alpha8))
            {
                Num = true;
                Number = 8;
            }
            if (Input.GetKey(KeyCode.Alpha9))
            {
                Num = true;
                Number = 9;
            }
        }
        else
        {
            MouseX = 0;
            MouseY = 0;
            Horizontal = 0;
            Vertical = 0;
            Fire = 0;
            Block = 0;
            Jump = false;
            Tab = false;
        }
    }
}
