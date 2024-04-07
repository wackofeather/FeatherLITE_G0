using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;


public class WeaponClass : ScriptableObject , IWeaponable
{
    public WeaponData weaponData;
    public float key;



    //just the dynamic data is declared here
    
    protected bool isShooting;
    protected bool isScoping;
    [HideInInspector] public PlayerStateMachine player;
    [HideInInspector] public Player_Inventory inventory;
    protected float shootingTimer;
    public virtual void Weapon_Update() 
    {
        if (shootingTimer > 0) shootingTimer -= Time.deltaTime;
        //Debug.Log(isShooting);

        if (!player.isMelee) inventory.ChangeCurrentWeapon((int)inventory.SwitchWeapon.action.ReadValue<float>());
    }

    public virtual void EnterWeapon()
    {
        isShooting = false;
        isScoping = false;

        player.player_VP_ARM_anim_controller.runtimeAnimatorController = weaponData.VM_ARM_animatorOverrideController;
        player.player_VP_ARM_anim_controller.SetTrigger("SwitchWeapon");

        //player.player_VP_GUN_anim_controller.runtimeAnimatorController = weaponData.VM_GUN_animatorOverrideController;
        inventory.GetCurrentWeaponAnimator().SetTrigger("SwitchWeapon");

        inventory.player_WeaponMesh.GetComponent<MeshFilter>().sharedMesh = weaponData.weaponMesh.GetComponentInChildren<MeshFilter>().sharedMesh;
        inventory.player_WeaponMesh.GetComponent<MeshRenderer>().sharedMaterial = weaponData.weaponMesh.GetComponentInChildren<MeshRenderer>().sharedMaterial;

        shootingTimer = weaponData.enterStateTime;

        
        
        Debug.Log("weapon switched");
    }

    public virtual void ExitWeapon() 
    {
        player.player_VP_ARM_anim_controller.ResetTrigger("SwitchWeapon");
        inventory.GetCurrentWeaponAnimator().ResetTrigger("SwitchWeapon");
    }
    
}
