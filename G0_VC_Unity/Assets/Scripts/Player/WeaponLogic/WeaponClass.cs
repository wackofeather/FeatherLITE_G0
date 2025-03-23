using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class WeaponClass : ScriptableObject , IWeaponable
{

/*    public T weaponTYPEData<T>(T param)
    {
        return param;
    }*/
    public WeaponData weaponData;
    public float key;



    //just the dynamic data is declared here
    [HideInInspector] public PlayerStateMachine player;
    [HideInInspector] public Player_Inventory inventory;
    protected float shootingTimer;
    protected float currentAmmo;

    public virtual void Weapon_Init()
    {
        
    }
    public virtual void Weapon_Update() 
    {


        if (!player.networkInfo._isOwner) return;

        if (shootingTimer > 0) shootingTimer -= Time.deltaTime;


        
    }

    public virtual void EnterWeapon()
    {

        player.player_EXT_ARM_anim_controller.runtimeAnimatorController = weaponData.EXT_ARM_animatorOverrideController;
        player.player_EXT_ARM_anim_controller.SetTrigger("SwitchWeapon");
        inventory.EXT_GetCurrentWeaponAnimator().SetTrigger("SwitchWeapon");

        if (!player.networkInfo._isOwner) return;


        shootingTimer = weaponData.enterStateTime;

        inventory.isShooting = false;
        inventory.isScoping = false;

        player.player_VP_ARM_anim_controller.runtimeAnimatorController = weaponData.VM_ARM_animatorOverrideController;
        player.player_VP_ARM_anim_controller.SetTrigger("SwitchWeapon");

        //player.player_VP_GUN_anim_controller.runtimeAnimatorController = weaponData.VM_GUN_animatorOverrideController;
        inventory.VP_GetCurrentWeaponAnimator().SetTrigger("SwitchWeapon");

        //inventory.player_WeaponMesh.GetComponent<MeshFilter>().sharedMesh = weaponData.weaponMesh.GetComponentInChildren<MeshFilter>().sharedMesh;
        //inventory.player_WeaponMesh.GetComponent<MeshRenderer>().sharedMaterial = weaponData.weaponMesh.GetComponentInChildren<MeshRenderer>().sharedMaterial;

        

        
        
        //Debug.Log("weapon switched");
    }

    public virtual void ExitWeapon() 
    {

        player.player_EXT_ARM_anim_controller.ResetTrigger("SwitchWeapon");
        inventory.EXT_GetCurrentWeaponAnimator().ResetTrigger("SwitchWeapon");

        if (!player.networkInfo._isOwner) return;

        player.player_VP_ARM_anim_controller.ResetTrigger("SwitchWeapon");
        inventory.VP_GetCurrentWeaponAnimator().ResetTrigger("SwitchWeapon");

        //player.inventory.isShooting = false;
    }
    
}
