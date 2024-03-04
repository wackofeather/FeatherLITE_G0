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
        
        base.Weapon_Update();

        player.player_VP_anim_controller.SetBool("Scoping", isScoping);

        player.player_VP_anim_controller.SetBool("Firing", isShooting);


        if (shootingTimer > 0) return;




        if (weaponData.fireInput.action.IsPressed() && !player.isMelee) isShooting = true;
        else isShooting = false;

        if (weaponData.scope.action.IsPressed() && !player.isMelee) StartScope();
        else StopScope();
        //Debug.Log(isShooting);
    }


    public void StartScope() { isScoping = true; }
    public void StopScope() { isScoping = false; }

}
