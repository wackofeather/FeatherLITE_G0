using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[CreateAssetMenu]
public class GunClass : WeaponClass
{
    public GunData gunData;
    [HideInInspector] float maxAmmo_Mag;
    [SerializeField] GameObject bullet;

    public override void EnterWeapon()
    {
        base.EnterWeapon();

    }


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




        if (weaponData.fireInput.action.IsPressed() && !player.isMelee) inventory.StartCoroutine(shootCoroutine());
        else inventory.isShooting = false;

        if (weaponData.scope.action.IsPressed() && !player.isMelee) StartScope();
        else StopScope();
        //Debug.Log(isShooting);
    }


    public void StartScope() { inventory.isScoping = true; }
    public void StopScope() { inventory.isScoping = false; }

    public virtual IEnumerator shootCoroutine()
    {
        inventory.isShooting = true;
        while (true)
        {
            if (!weaponData.fireInput.action.IsPressed()) break;
            if (player.isMelee) break;
            yield return new WaitForSeconds(1/weaponData.BPS);
        }
        inventory.isShooting = false;
        yield break;
    }
}
