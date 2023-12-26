using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Inventory : MonoBehaviour
{
    [SerializeField] int MaxWeapons;
    public List<GameObject> Weapon_Inventory;
    private GameObject currentWeapon;
    private int current_Index;
    [SerializeField] GameObject testWeapon;


    // Start is called before the first frame update
    void Awake()
    {
        GiveWeapon(testWeapon);
        GiveWeapon(testWeapon);
        //currentWeapon = Weapon_Inventory[0];

    }

    private void Update()
    {
        //Debug.Log(current_Index);
    }

    public void ChangeCurrentWeapon(int direction)
    {
        if (direction == 0) return;
        for (int i = 1;  i < Weapon_Inventory.Count; i++)
        {
            int index = Mathf.Abs((currentWeapon.GetComponent<WeaponClass>().ArraySpot() + i * direction)) % Weapon_Inventory.Count;

            if ((Weapon_Inventory[index]) == null) return;

            current_Index = index;
            currentWeapon = Weapon_Inventory[index];
        }
    }



    public void GiveWeapon(GameObject weapon)
    {
        if (Weapon_Inventory.Count >= MaxWeapons)
        {
            return; //eventually do swapping weapons
        }

        current_Index = Weapon_Inventory.Count + 1; 
        Weapon_Inventory.Add(weapon);//weapon.clone(Weapon_Inventory.Count + 1));

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
        currentWeapon = Weapon_Inventory[index];

    }
}
