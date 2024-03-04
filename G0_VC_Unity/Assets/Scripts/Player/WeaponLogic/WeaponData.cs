using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class WeaponData : ScriptableObject
{
    //[SerializeField] public WeaponClass weaponClass;
    public InputActionReference fireInput;
    public InputActionReference scope;
    public GameObject weaponMesh;
    [Header("ViewModel")]
    public AnimatorOverrideController VM_animatorOverrideController;
    [Header("Exterior")]
    public AnimatorOverrideController EXT_Player_animatorOverrideController;
    public AnimatorOverrideController EXT_Weapon_animatorOverrideController;

    public float enterStateTime;

/*    public class WeaponLogic : IWeaponable
    {
        protected bool isShooting;
        protected bool isScoping;
        public virtual void Weapon_Update(PlayerStateMachine player, Player_Inventory inventory) { }

        public virtual void EnterWeapon(PlayerStateMachine player, Player_Inventory inventory)
        {
            *//*        player.player_VP_anim_controller.runtimeAnimatorController = weaponData.VM_animatorOverrideController;
                    player.player_VP_anim_controller.SetTrigger("SwitchWeapon");
                    Debug.Log("weapon switched");*//*
        }

        public virtual void ExitWeapon(PlayerStateMachine player, Player_Inventory inventory)
        {
            //  player.player_VP_anim_controller.ResetTrigger("SwitchWeapon");
        }
    }

    public virtual IWeaponable GetClass()
    {
        return null;
    }*/

}
