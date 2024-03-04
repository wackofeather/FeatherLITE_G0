using Steamworks.Ugc;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Inventory : NetworkBehaviour
{
    [SerializeField] WeaponLookup WeaponLookup;
    [SerializeField] int MaxWeapons;
    public List<WeaponClass> Weapon_Inventory = new List<WeaponClass>();
    //private GameObject currentWeapon;
    private int current_Index;
    // [SerializeField] WeaponClass testWeapon;
    //public List<WeaponClass> Test_List;
    public InputActionReference SwitchWeapon;
    [SerializeField] WeaponManager WeaponManager;

    private WeaponClass currentWeapon;

    [SerializeField] PlayerStateMachine player;

    public GameObject player_WeaponMesh;



    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) return;

        for (int i = 0; i < WeaponLookup.weaponLookup.Count; i++)
        {
            GiveWeapon(WeaponLookup.weaponLookup[i]);
            Debug.Log("ahhhhh");
        }
      //  currentWeapon = Weapon_Inventory[0];
        //ChangeCurrentWeapon_INDEX(0);
    }
    // Start is called before the first frame update
    void Awake()
    {
        //currentWeapon = Weapon_Inventory[0];

    }

    private void Update()
    {
        //ChangeCurrentWeapon((int)SwitchWeapon.action.ReadValue<float>());;
       // Debug.Log(Weapon_Inventory.Count);
       // if ( != 0) Debug.Log(SwitchWeapon.action.ReadValue<float>());
    }

    public void ChangeCurrentWeapon(int direction)
    {
        if (direction == 0 | Weapon_Inventory.Count == 1) return;
        direction = Mathf.Clamp(direction, -1, 1);
        for (int i = 1;  i < Weapon_Inventory.Count; i++)
        {
            int index = Mathf.Abs((current_Index + i * direction)) % Weapon_Inventory.Count;

            if ((Weapon_Inventory[index]) == null) return;

            current_Index = index;
            changeWeapon_Internal(Weapon_Inventory[index]);

        }
    }



    public void GiveWeapon(WeaponClass weapon_class)
    {
        if (Weapon_Inventory.Count >= MaxWeapons)
        {
            return; //eventually do swapping weapons
        }

        current_Index = Weapon_Inventory.Count - 1;

        WeaponClass weapon_copy = Instantiate(weapon_class);

        weapon_copy.player = player;
        weapon_copy.inventory = this;

        Weapon_Inventory.Add(weapon_copy);//weapon.clone(Weapon_Inventory.Count + 1));
        changeWeapon_Internal(weapon_copy);

        /* for (int i = 0; i < Weapon_Inventory.Count; i++)
         {
             if (Weapon_Inventory[i] == null)
             {
                 Weapon_Inventory[i] = weapon;
                 ChangeCurrentWeapon_INDEX(i);
                 break;
             }
         }*/
    }


    public void ChangeCurrentWeapon_INDEX(int index)
    {
        //Debug.Log("ahhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhsksjskshskshskshsks");
        changeWeapon_Internal(Weapon_Inventory[index]);
        current_Index = index;

    }

    public void changeWeapon_Internal(WeaponClass weapon)
    {
        if (currentWeapon != null) currentWeapon.ExitWeapon();
        currentWeapon = weapon;
        currentWeapon.EnterWeapon();
    }


    public WeaponClass GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
