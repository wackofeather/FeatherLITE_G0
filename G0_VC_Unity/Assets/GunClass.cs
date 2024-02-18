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

    public override void UseWeapon(PlayerStateMachine player)
    {
        base.UseWeapon(player);
        if (fireInput.action.IsPressed() && !player.isMelee) isShooting = true;
        else isShooting = false;
        //Debug.Log(isShooting);
    }

    public override void Scope(PlayerStateMachine player)
    {
        base.Scope(player);

        if (scope.action.IsPressed() && !player.isMelee) StartScope();
        else StopScope();

    }


    public override void Animate()
    {
        base.Animate();

        test_animator.SetBool("Scoping", isScoping);

        test_animator.SetBool("Firing", isShooting);
    }

    public void StartScope() { isScoping = true; }
    public void StopScope() { isScoping = false; }
}
