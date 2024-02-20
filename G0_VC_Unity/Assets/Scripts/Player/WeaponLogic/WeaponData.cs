using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    public WeaponClass weaponClass;
    public InputActionReference fireInput;
    public InputActionReference scope;
    public GameObject weaponMesh;
    [Header("ViewModel")]
    public AnimatorOverrideController VM_animatorOverrideController;
    [Header("Exterior")]
    public AnimatorOverrideController EXT_Player_animatorOverrideController;
    public AnimatorOverrideController EXT_Weapon_animatorOverrideController;

    
}
