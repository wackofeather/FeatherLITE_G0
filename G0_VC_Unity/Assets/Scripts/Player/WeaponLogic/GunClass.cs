using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[CreateAssetMenu]
public class GunClass : WeaponClass
{
    [HideInInspector] float maxAmmo_Mag;
    [SerializeField] float maxAmmo_Inventory;
    [Space]
    [SerializeField] float BPS;
    


    public override void Weapon_Update()
    {
        player.player_EXT_ARM_anim_controller.SetBool("Scoping", inventory.isScoping);

        player.player_EXT_ARM_anim_controller.SetBool("Firing", inventory.isShooting);

        inventory.EXT_GetCurrentWeaponAnimator().SetBool("Scoping", inventory.isScoping);

        inventory.EXT_GetCurrentWeaponAnimator().SetBool("Firing", inventory.isShooting);

        if (!player.IsOwner) return;


        base.Weapon_Update();

        player.player_VP_ARM_anim_controller.SetBool("Scoping", inventory.isScoping);

        player.player_VP_ARM_anim_controller.SetBool("Firing", inventory.isShooting);

        inventory.VP_GetCurrentWeaponAnimator().SetBool("Scoping", inventory.isScoping);

        inventory.VP_GetCurrentWeaponAnimator().SetBool("Firing", inventory.isShooting);


        if (shootingTimer > 0) return;




        if (weaponData.fireInput.action.IsPressed() && !player.isMelee) inventory.isShooting = true;
        else inventory.isShooting = false;

        if (weaponData.scope.action.IsPressed() && !player.isMelee) StartScope();
        else StopScope();
        //Debug.Log(isShooting);
    }


    public void StartScope() { inventory.isScoping = true; }
    public void StopScope() { inventory.isScoping = false; }

}
