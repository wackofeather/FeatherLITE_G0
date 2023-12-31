using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponClass : MonoBehaviour
{
    [SerializeField] protected InputActionReference FireInput;
    [Header("ViewModel")]
    [SerializeField] protected AnimatorOverrideController VM_Player_animatorOverrideController;
    [SerializeField] protected AnimatorOverrideController VM_Weapon_animatorOverrideController;
    [Header("Exterior")]
    [SerializeField] protected AnimatorOverrideController EXT_Player_animatorOverrideController;
    [SerializeField] protected AnimatorOverrideController EXT_Weapon_animatorOverrideController;


    public virtual void UseWeapon() { }
    
}
