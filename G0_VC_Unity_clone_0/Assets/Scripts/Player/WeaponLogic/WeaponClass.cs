using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;


public class WeaponClass
{
    protected WeaponData weaponData;
    protected bool isShooting;
    protected bool isScoping;
    

    protected PlayerStateMachine player;
    protected Player_Inventory inventory;

    public WeaponClass(PlayerStateMachine _player, Player_Inventory _inventory, WeaponData weaponData)
    {
        this.player = _player;
        this.inventory = _inventory;
        this.weaponData = weaponData;
    }

    public virtual void Weapon_Update() { Debug.Log("ahhhhhhhhhhh"); }

    public virtual void EnterWeapon()
    {
        player.player_VP_anim_controller.runtimeAnimatorController = weaponData.VM_animatorOverrideController;
        player.player_VP_anim_controller.SetTrigger("SwitchWeapon");
        Debug.Log("weapon switched");
    }

    public virtual void ExitWeapon() 
    {
        player.player_VP_anim_controller.ResetTrigger("SwitchWeapon");
    }
    
}
