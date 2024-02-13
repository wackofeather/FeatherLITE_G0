using System.Threading;
using UnityEngine;

public class RegularState : BasePlayerState
{
    public RegularState(PlayerStateMachine player) : base(player)
    {
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
        ///Debug.Log("ahhhhhhhhhhhhhhhh");
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!player.IsOwner) return;

        if (player.inputVector == Vector3.zero) return;

        if (player.rb.velocity.magnitude > player.BreakNeckSpeed)
        {
            //Debug.Log("ahhhhhhhhhhhhhh");
            Vector3 inputVelocity = player.inputVector * player.speed;
            Vector3 relativeVelocity = player.PlayerCamera.transform.InverseTransformVector(player.rb.velocity);



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
            Debug.Log("nababajinidsjhdjhdjhdsjkfhsdfhsadfhasdkjfhasdfhadsjfjladsfhjladslf");
        }
        else player.rb.velocity = (player.PlayerCamera.transform.rotation * player.inputVector * player.speed - player.rb.velocity) * player.accel * Time.fixedDeltaTime; //player.rb.AddForce((player.PlayerCamera.transform.rotation * player.inputVector * player.speed - player.rb.velocity) * player.accel);
    }

    public override void Update()
    {
        base.Update();
        //Debug.Log(player.quality);
        if (player.Grapple.action.triggered)
        {
            if (player.CanGrapple()) player.ChangeState(player.GrapplingState);
        }
        
    }
}

