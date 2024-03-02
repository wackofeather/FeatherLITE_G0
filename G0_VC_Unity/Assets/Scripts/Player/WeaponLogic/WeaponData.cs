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

    public class WeaponLogic : IWeaponable
    {
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

    public virtual IWeaponable GetClass()
    {
        return null;
    }

}
