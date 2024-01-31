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

    public override void UseWeapon()
    {
        base.UseWeapon();
        if (fireInput.action.IsPressed()) isShooting = true;
        else isShooting = false;
        Debug.Log(isShooting);
    }

    public override void Scope()
    {
        base.Scope();

        if (scope.action.IsPressed()) StartScope();
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
