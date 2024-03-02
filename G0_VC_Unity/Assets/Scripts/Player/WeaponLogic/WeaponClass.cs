using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;


public class WeaponClass : ScriptableObject //, IWeaponable
{
    public float key;
    [SerializeField] protected WeaponData weaponData;
    protected bool isShooting;
    protected bool isScoping;
    [HideInInspector] public PlayerStateMachine player;
    [HideInInspector] public Player_Inventory inventory;

    public virtual void Weapon_Update() { }

    public virtual void EnterWeapon()
    {
/*        player.player_VP_anim_controller.runtimeAnimatorController = weaponData.VM_animatorOverrideController;
        player.player_VP_anim_controller.SetTrigger("SwitchWeapon");
        Debug.Log("weapon switched");*/
    }

    public virtual void ExitWeapon() 
    {
      //  player.player_VP_anim_controller.ResetTrigger("SwitchWeapon");
    }
    
}
