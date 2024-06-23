using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class WeaponSwitchState : BasePlayerState
{
    float timer;
    Vector3 ogVelocity;
    public WeaponSwitchState(PlayerStateMachine player) : base(player)
    {
        key = 4;
    }


    public override void EnterState()
    {
        base.EnterState();
        player.isInteracting = true;
        timer = player.weapon_pickingUp.weaponData.pickUpTime;
        ogVelocity = player.rb.velocity;
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    

    public override void Update()
    {
        base.Update();

        if (!player.IsLookingAtInteractable(player.lookAtObject)) player.ChangeState(player.RegularState);
        if (!player.interact.action.IsPressed()) player.ChangeState(player.RegularState);
        if (!player.isInteracting) player.ChangeState(player.RegularState);

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            player.inventory.GiveWeapon(player.weapon_pickingUp);
            player.ChangeState(player.RegularState);
        }
    }


    public override void FixedUpdate()
    {
        player.rb.AddForce(-player.rb.velocity * player.slowDownRate);
    }

    public override void LateUpdate()
    {
        //nothing, just void override for now
    }

    public override void ExitState()
    {
        base.ExitState();
        player.rb.AddForce(Vector3.ClampMagnitude(ogVelocity, player.maxPushSpeed) * player.pushFactor, ForceMode.Impulse);
        player.isInteracting = false;
        player.InteractCoolDownTimer = player.pickupCooldownTime;
        player.hasPickedUpInteractButton = false;
    }
}
