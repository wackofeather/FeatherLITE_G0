using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeleeState : BasePlayerState
{
    private Quaternion meleeRotation;
    private float timer;
    private float timerVal;
    public MeleeState(PlayerStateMachine player) : base(player)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();

        timer = player.meleeAnim.length;
        timerVal = player.meleeAnim.length;

        player.isGrappling = false;
        player.isScoping = false;

        player.player_anim_controller.SetTrigger("Melee");

        meleeRotation = player.Playercamera.rotation;

        //player.rb.velocity = meleeRotation.normalized * Vector3.forward * player.meleeSpeed;
        //Debug.Log("whahahahahahaahahah");
        //player.rb.AddForce(meleeRotation.normalized * Vector3.forward * player.meleeSpeed, ForceMode.VelocityChange);
    }

    public override void ExitState()
    {
        base.ExitState();

        player.player_anim_controller.ResetTrigger("Melee");

       
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();

        timer -= Time.deltaTime;

        if ((player.meleeSpeed > player.speed) && (Mathf.Abs(player.PlayerCamera.transform.InverseTransformVector(player.rb.velocity).z) < player.BreakNeckSpeed)) player.rb.velocity = meleeRotation.normalized * Vector3.forward * (((player.meleeSpeed - player.speed) * (timer/timerVal)) + player.speed - 1f);

        if (timer < 0) player.ChangeState(player.RegularState);
    }
}

