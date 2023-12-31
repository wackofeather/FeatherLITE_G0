using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class GunClass : WeaponClass
{
    [SerializeField] float maxAmmo_Mag;
    [SerializeField] float maxAmmo_Inventory;
    [Space]
    [SerializeField] float BPS;

    public override void UseWeapon()
    {
        base.UseWeapon();
        if (FireInput.action.IsPressed()) Debug.Log("pew pew");
    }
}
