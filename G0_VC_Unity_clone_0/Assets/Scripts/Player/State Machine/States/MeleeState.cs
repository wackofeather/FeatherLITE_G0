using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeleeState : BasePlayerState
{
    private Quaternion meleeRotation;
    private float timer;
    private float timerVal;

    private Vector3 startingVelocity;
    private Vector3 endVelocity;

    public MeleeState(PlayerStateMachine player) : base(player)
    {
        key = 3;
    }

    public override void AnimationTriggerEvent()
    {
        if (!player.IsOwner) return;

        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {        
        
        if (!player.IsOwner) return;


        base.EnterState();



        player.player_anim_controller.SetTrigger("Melee");


        timer = player.meleeAnim.length;
        timerVal = player.meleeAnim.length;

        player.isGrappling = false;
        player.isScoping = false;


        meleeRotation = player.Playercamera.rotation;


        startingVelocity = player.rb.velocity;
        endVelocity = meleeRotation.normalized * Vector3.forward * player.meleeSpeed;
        //player.rb.velocity = meleeRotation.normalized * Vector3.forward * player.meleeSpeed;
        //Debug.Log("whahahahahahaahahah");
        //player.rb.AddForce(meleeRotation.normalized * Vector3.forward * player.meleeSpeed, ForceMode.VelocityChange);
    }

    public override void ExitState()
    {
        
        
        if (!player.IsOwner) return;


        base.ExitState();

        

        player.player_anim_controller.ResetTrigger("Melee");

       
    }

    public override void FixedUpdate()
    {
        if (!player.IsOwner) return;

        base.FixedUpdate();


        player.rb.velocity = Vector3.Lerp(startingVelocity, endVelocity, player.meleeCurve.Evaluate(1-timer));
        //if ((player.meleeSpeed > player.speed) && (player.PlayerCamera.transform.InverseTransformVector(player.rb.velocity).z < player.BreakNeckSpeed)) player.rb.velocity = ((meleeRotation.normalized * Vector3.forward * (((player.meleeSpeed - player.speed) * (timer / timerVal)) + player.speed - 1f)) - player.rb.velocity) * 40 * Time.fixedDeltaTime;
    }

    public override void Update()
    {
        

        base.Update();


        if (!player.IsOwner) return;

        timer -= Time.deltaTime;



        if (timer < 0) player.ChangeState(player.RegularState);
    }
}

