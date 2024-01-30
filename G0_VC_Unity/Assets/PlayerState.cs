using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerBase
{
    public class BasePlayerState
    {
        protected PlayerBase player;
        protected PlayerStateMachine stateMachine;

        public BasePlayerState(PlayerBase player, PlayerStateMachine playerStateMachine)
        {
            this.player = player;
            this.stateMachine = playerStateMachine;
        }

        public virtual void EnterState()
        {
            
        }
        public virtual void ExitState()
        {

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

        public virtual void AnimationTriggerEvent()
        {

        }
    }
}

