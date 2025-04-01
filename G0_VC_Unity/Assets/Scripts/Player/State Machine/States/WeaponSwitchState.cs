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
        ogVelocity = player.rb.linearVelocity;
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    

    public override void Update()
    {
        base.Update();



        timer -= Time.deltaTime;
        Game_UI_Manager.game_instance.SetCountdownText(timer);



        if (!player.IsLookingAtInteractable(player.lookAtObject)) player.ChangeState(player.RegularState);
        if (!player.interact.action.IsPressed())
        {
            player.ChangeState(player.RegularState);
        } 
        if (!player.isInteracting) player.ChangeState(player.RegularState);

   
        

        if (timer <= 0)
        {
            player.inventory.GiveWeapon(player.weapon_pickingUp);
            player.ChangeState(player.RegularState);
            return;
        }
        
    }


    public override void FixedUpdate()
    {
        base.FixedUpdate();

        player.rb.AddForce(-player.rb.linearVelocity * player.slowDownRate);
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
        Game_UI_Manager.game_instance.SetCountdownText(0);
    }
}
