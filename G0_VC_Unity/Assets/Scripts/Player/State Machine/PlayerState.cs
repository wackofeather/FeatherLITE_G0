using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BasePlayerState
{
    protected PlayerStateMachine player;

    public BasePlayerState(PlayerStateMachine _player)
    {
        this.player = _player;
    }

    public virtual void EnterState()
    {
        //Debug.Log("entered state");
    }
    public virtual void ExitState()
    {
        Debug.Log("exit state");
    }
    public virtual void Update()
    {
        player.inputVector = player.move.action.ReadValue<Vector3>();
        float mouseX = Input.GetAxis("Mouse X") * player.mouseSens;

        float mouseY = Input.GetAxis("Mouse Y") * player.mouseSens;



        player.xRotation -= mouseY;
        player.yRotation += mouseX;

        player.xRotation = Mathf.Clamp(player.xRotation, -90, 90);


        player.Rotatables.localRotation = Quaternion.Euler(player.xRotation, player.yRotation, 0);
        player.PlayerCamera.localRotation = Quaternion.Euler(player.xRotation, player.yRotation, 0);
    }
    public virtual void FixedUpdate()
    {

    }

    public virtual void LateUpdate()
    {

    }

    public virtual void AnimationTriggerEvent()
    {

    }
}

