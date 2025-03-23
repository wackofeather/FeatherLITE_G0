using UnityEngine;

public class ReloadState : BasePlayerState
{
    public ReloadState(PlayerStateMachine player) : base(player)
    {
        key = 6;
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();

        if (player.networkInfo._isOwner) player.inventory.isReloading = false;
        player.isInteracting = false;
        ///Debug.Log("ahhhhhhhhhhhhhhhh");
    }

    public override void FixedUpdate()
    {

        base.FixedUpdate();

        if (!player.networkInfo._isOwner) { return; }









        ///if owner

        if (player.inputVector == Vector3.zero) return;

        if (player.rb.linearVelocity.magnitude > player.BreakNeckSpeed)
        {
            //Debug.Log("ahhhhhhhhhhhhhh");
            Vector3 inputVelocity = player.inputVector * player.speed;
            Vector3 relativeVelocity = player.PlayerCamera.transform.InverseTransformVector(player.rb.linearVelocity);



            if ((Mathf.Abs(inputVelocity.x) > Mathf.Abs(relativeVelocity.x)) | (Mathf.Sign(inputVelocity.x) != Mathf.Sign(relativeVelocity.x))) player.putTogetherVelocity.x = (inputVelocity.x - relativeVelocity.x);
            else player.putTogetherVelocity.x = 0;
            if ((Mathf.Abs(inputVelocity.y) > Mathf.Abs(relativeVelocity.y)) | (Mathf.Sign(inputVelocity.y) != Mathf.Sign(relativeVelocity.y))) player.putTogetherVelocity.y = (inputVelocity.y - relativeVelocity.y);
            else player.putTogetherVelocity.y = 0;
            if ((Mathf.Abs(inputVelocity.z) > Mathf.Abs(relativeVelocity.z)) | (Mathf.Sign(inputVelocity.z) != Mathf.Sign(relativeVelocity.z))) player.putTogetherVelocity.z = (inputVelocity.z - relativeVelocity.z);
            else
            {
                player.putTogetherVelocity.z = 0;
                //Debug.Log("bwaaragagahagahsfghjaklsjsjskjsjsjsj");
            }

            player.rb.AddForce(player.PlayerCamera.transform.rotation * player.putTogetherVelocity * player.tooFastaccel);
        }
        else player.rb.AddForce((player.PlayerCamera.transform.rotation * player.inputVector * player.speed/2 - player.rb.linearVelocity) * player.accel);
    }

    public override void Update()
    {


        base.Update();


        if (!player.networkInfo._isOwner) return;

        //Debug.Log(player.quality);


        if (player.grappleWiggle_Timer > 0) if (player.CanGrapple()) player.ChangeState(player.GrapplingState);

        //player.InteractCheck();

    }
}
