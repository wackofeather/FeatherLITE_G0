using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MeleeState : BasePlayerState
{
    private Vector3 meleeVector;
    private float timer;
    private float timerVal;

    private Vector3 startingVelocity;
    private Vector3 endVelocity;

    private float meleeDistance;

    private Camera cam;
    private float camFOV;

    private float viewportCamFOV;



    public MeleeState(PlayerStateMachine player) : base(player)
    {
        key = 3;
        cam = player.PlayerCamera.GetComponent<Camera>();
        camFOV = cam.fieldOfView;
        timerVal = player.meleeAnim.length; //0.75f * player.meleeAnim.length;

        viewportCamFOV = player.ViewportFOV;
    }





    public override void AnimationTriggerEvent()
    {
        if (!player.IsOwner) return;

        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {        
        
        if (!player.IsOwner) return;




        
        player.isMelee = true;


        timer = timerVal;

        player.isGrappling = false;
        player.isScoping = false;

        //Vector3 vectorweight = new Vector3(player.inputVector.x * player.meleeWeight_Input + player.PlayerCamera.transform.InverseTransformDirection(player.rb.velocity).x * player.meleeWeight_Velocity, 0, 0);
        //Debug.Log(vectorweight);

        Vector3 vectorweight = new Vector3(player.inputVector.x * player.meleeWeight_Input,0,0);

        meleeVector = player.PlayerCamera.transform.forward + (player.PlayerCamera.transform.rotation * vectorweight);




        startingVelocity = player.rb.velocity;
        endVelocity = meleeVector.normalized * player.meleeSpeed;

        base.EnterState();

        //player.rb.velocity = meleeRotation.normalized * Vector3.forward * player.meleeSpeed;
        //Debug.Log("whahahahahahaahahah");
        //player.rb.AddForce(meleeRotation.normalized * Vector3.forward * player.meleeSpeed, ForceMode.VelocityChange);
    }

    public override void ExitState()
    {
        
        
        if (!player.IsOwner) return;

        cam.fieldOfView = camFOV;
        for (int i = 0; i < player.ViewportRenderers.Count; i++)
        {
            player.ViewportRenderers[i].settings.cameraSettings.cameraFieldOfView = viewportCamFOV;
           // player.ViewportRenderers[i].SetDirty();
        }



        player.isMelee = false;

        base.ExitState();

       
    }

    public override void FixedUpdate()
    {
        if (!player.IsOwner) return;

        base.FixedUpdate();

        float meleeProgress = player.meleeCurve.Evaluate(1 - timer);
        Vector3 velocityVector;


        if (meleeProgress <= 1) velocityVector = startingVelocity + (endVelocity - startingVelocity) * player.meleeCurve.Evaluate(1 - timer);
        else velocityVector = endVelocity * meleeProgress;

        player.rb.velocity = velocityVector;

        //player.rb.AddForce(velocityVector, ForceMode.VelocityChange);

        //if (Physics.Raycast(player.rb.position, new Vector3(meleeRotation.x, meleeRotation.y, meleeRotation.z), player.rb.velocity.magnitude))
        //if ((player.meleeSpeed > player.speed) && (player.PlayerCamera.transform.InverseTransformVector(player.rb.velocity).z < player.BreakNeckSpeed)) player.rb.velocity = ((meleeRotation.normalized * Vector3.forward * (((player.meleeSpeed - player.speed) * (timer / timerVal)) + player.speed - 1f)) - player.rb.velocity) * 40 * Time.fixedDeltaTime;
    }

    public override void Update()
    {
        

        base.Update();


        if (!player.IsOwner) return;

        float fov_calc = viewportCamFOV * (1 + player.meleeFOV_curve.Evaluate(1 - timer));

        for (int i  = 0; i < player.ViewportRenderers.Count; i++)
        {
            player.ViewportRenderers[i].settings.cameraSettings.cameraFieldOfView = fov_calc;
           // player.ViewportRenderers[i].SetDirty();
        }

        cam.fieldOfView = camFOV * (1 + player.meleeFOV_curve.Evaluate(1 - timer));

        timer -= Time.deltaTime;



        if (timer < 0) player.ChangeState(player.RegularState);
    }

    public override void OnCollisionEnter(Collision col)
    {
        base.OnCollisionEnter(col);

        if (!player.IsOwner) return;

        if (col.contacts.Length > 1)
        {
            player.ChangeState(player.RegularState);
            return;
        }

        if (Vector3.Angle((col.contacts[0].point - player.rb.position), meleeVector) < 90) player.ChangeState(player.RegularState);
    }




    public static float IntegrateCurve(AnimationCurve curve, float startTime, float endTime, int steps)
    {
        float val = 0;
        for (int i = 0; i < steps; i++)
        {
            val += curve.Evaluate(startTime + (endTime - startTime) * i/steps) * ( (endTime - startTime) / steps );
        }
        return val;
    }

}

