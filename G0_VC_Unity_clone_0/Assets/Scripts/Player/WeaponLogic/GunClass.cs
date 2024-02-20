using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class GunClass : WeaponClass
{
    [SerializeField] float maxAmmo_Mag;
    [SerializeField] float maxAmmo_Inventory;
    [Space]
    [SerializeField] float BPS;


    public GunClass(PlayerStateMachine player, Player_Inventory inventory, WeaponData weaponData) : base(player, inventory, weaponData)
    {

    }

    public override void Weapon_Update()
    {
        base.Weapon_Update();
        Debug.Log("ahhhhhhhhhhhh");
        if (weaponData.fireInput.action.IsPressed() && !player.isMelee) isShooting = true;
        else isShooting = false;

        if (weaponData.scope.action.IsPressed() && !player.isMelee) StartScope();
        else StopScope();
        //Debug.Log(isShooting);

        player.player_VP_anim_controller.SetBool("Scoping", isScoping);

        player.player_VP_anim_controller.SetBool("Firing", isShooting);
    }


    public void StartScope() { isScoping = true; }
    public void StopScope() { isScoping = false; }
}
