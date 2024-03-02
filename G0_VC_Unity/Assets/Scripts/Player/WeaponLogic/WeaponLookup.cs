using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponLookup : ScriptableObject
{
    public List<WeaponClass> weaponLookup;

    public WeaponClass getWeapon(float key)
    {
        foreach (var weapon in weaponLookup)
        {
            if (weapon.key == key) return weapon;
        }

        return null;
    }
}
