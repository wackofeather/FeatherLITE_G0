using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class WeaponManager : NetworkBehaviour
{
    [SerializeField] Player_Inventory inventory;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) return;

    }

    // Update is called once per frame
    void Update()
    {


        //Debug.Log(inventory.GetCurrentWeapon());
        if (inventory.GetCurrentWeapon() != null) inventory.GetCurrentWeapon().Weapon_Update();
        
    }
}
