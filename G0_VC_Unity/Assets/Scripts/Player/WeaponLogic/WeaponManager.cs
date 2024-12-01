using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class WeaponManager : MonoBehaviour
{
    [SerializeField] Player_Inventory inventory;




    // Update is called once per frame
    void Update()
    {


        //Debug.Log(inventory.GetCurrentWeapon());
        //if (inventory.GetCurrentWeapon() != null) inventory.GetCurrentWeapon().Weapon_Update();
        
    }
}
