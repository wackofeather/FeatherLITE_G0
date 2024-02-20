using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BasePlayerState
{
    protected PlayerStateMachine player;

    public float key;

    public BasePlayerState(PlayerStateMachine _player)
    {
        this.player = _player;
    }

    public virtual void EnterState()
    {
        AnimationTriggerEvent();
    }
    public virtual void ExitState()
    {
        //Debug.Log("exit state");
    }
    public virtual void Update()
    {
        if (!player.IsOwner)
        {
            //Debug.Log(player.internal_CurrentState);
            if (player.internal_CurrentState != key) player.ChangeState(player.stateDictionary[player.internal_CurrentState]);
            return;
        }

        player.inputVector = player.move.action.ReadValue<Vector3>();
        float mouseX = Input.GetAxis("Mouse X") * player.mouseSens;

        float mouseY = Input.GetAxis("Mouse Y") * player.mouseSens;



        player.xRotation -= mouseY;
        player.yRotation += mouseX;

        player.xRotation = Mathf.Clamp(player.xRotation, -90, 90);


        player.Rotatables.localRotation = Quaternion.Euler(player.xRotation, player.yRotation, 0);
        player.PlayerCamera.localRotation = Quaternion.Euler(player.xRotation, player.yRotation, 0);

        if (player.melee.action.triggered && player.CurrentPlayerState != player.MeleeState) player.ChangeState(player.MeleeState);
    }
    public virtual void FixedUpdate()
    {

    }

    public virtual void LateUpdate()
    {

    }

    public virtual void AnimationTriggerEvent()
    {
        player.player_VP_anim_controller.SetBool("Grappling", player.isGrappling);
        //player.player_anim_controller.SetBool("Scoping", player.isScoping);
        player.player_VP_anim_controller.SetBool("Melee", player.isMelee);
    }

    public virtual void Start_Init()
    {
        //probably a useless function, just use the initialization in each state
    }

    public virtual void OnCollisionEnter(Collision col)
    {

    }

    public virtual void OnCollisionExit(Collision col) 
    { 
    
    }
}


