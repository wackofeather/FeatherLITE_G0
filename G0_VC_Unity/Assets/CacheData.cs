using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class CacheStruct
{
    public Collider collider;
    public WeaponClass weapon;
}

public class CacheData : MonoBehaviour
{
    public List<CacheStruct> MyList = new List<CacheStruct>();
    Dictionary<Collider, WeaponClass> myDict = new Dictionary<Collider, WeaponClass>();

    void Awake()
    {
        foreach (var kvp in MyList)
        {
            myDict[kvp.collider] = kvp.weapon;
        }
    }

    public WeaponClass GetCacheWeapon(Collider col)
    {
        return myDict[col];
    }
}
