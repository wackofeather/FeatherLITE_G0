using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponClass : MonoBehaviour
{
    [SerializeField] protected InputActionReference fireInput;
    [SerializeField] protected InputActionReference scope;
    [Header("ViewModel")]
    [SerializeField] protected AnimatorOverrideController VM_Player_animatorOverrideController;
    [SerializeField] protected AnimatorOverrideController VM_Weapon_animatorOverrideController;
    [Header("Exterior")]
    [SerializeField] protected AnimatorOverrideController EXT_Player_animatorOverrideController;
    [SerializeField] protected AnimatorOverrideController EXT_Weapon_animatorOverrideController;
    [SerializeField] protected bool isShooting;
    [SerializeField] protected bool isScoping;
    public Animator test_animator;

    public virtual void UseWeapon(PlayerStateMachine player) { }

    public virtual void Scope(PlayerStateMachine player) { }

    public virtual void Animate() { }
    
}
