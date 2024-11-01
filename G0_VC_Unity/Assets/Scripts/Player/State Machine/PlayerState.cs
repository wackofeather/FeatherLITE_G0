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
    public virtual void InitializeState()
    {

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

        player.player_EXT_ARM_anim_controller.SetFloat("Y_Look", player.updown_Blendconstant);
        player.inventory.EXT_GetCurrentWeaponAnimator().SetFloat("Y_Look", player.updown_Blendconstant);

        if (!player.IsOwner)
        {
            //player.playerNetwork.ConsumeState();
            
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
        player.Exterior.localRotation = Quaternion.Euler(0, player.yRotation, 0);
        player.PlayerCamera.localRotation = Quaternion.Euler(player.xRotation, player.yRotation, 0);
        player.updown_Blendconstant = (player.xRotation + 90) / 180;

        if (player.melee.action.triggered && player.CurrentPlayerState != player.MeleeState) player.ChangeState(player.MeleeState);

        //player.playerNetwork.TransmitState();

        if (player.inventory.GetCurrentWeapon() != null) player.inventory.GetCurrentWeapon().Weapon_Update();
    }
    public virtual void FixedUpdate()
    {
        if (!player.IsOwner)
        {
            player.playerNetwork.FixedConsumeState();
        }
    }

    public virtual void LateUpdate()
    {
        if (!player.IsOwner) 
        {
            player.playerNetwork.ConsumeState();
            Game_UI_Manager.instance.UpdateDummyHealth(player);
            return;
        }
        player.playerNetwork.TransmitState();
        Game_UI_Manager.instance.UpdateGrappleIndicator(player.CanGrapple());
        Game_UI_Manager.instance.UpdateHealth(player.health);
    }

    public virtual void AnimationTriggerEvent()
    {
        player.player_EXT_ARM_anim_controller.SetBool("Grappling", player.isGrappling);
        //player.player_anim_controller.SetBool("Scoping", player.isScoping);
        player.player_EXT_ARM_anim_controller.SetBool("Melee", player.isMelee);

        if (player.inventory.EXT_GetCurrentWeaponAnimator() != null)
        {
            player.inventory.EXT_GetCurrentWeaponAnimator().SetBool("Grappling", player.isGrappling);

            player.inventory.EXT_GetCurrentWeaponAnimator().SetBool("Melee", player.isMelee);
        }
        
        if (!player.IsOwner) return;

        player.player_VP_ARM_anim_controller.SetBool("Grappling", player.isGrappling);
        //player.player_anim_controller.SetBool("Scoping", player.isScoping);
        player.player_VP_ARM_anim_controller.SetBool("Melee", player.isMelee);


        //Debug.Log(player.inventory.GetCurrentWeaponAnimator());

        if (player.inventory.VP_GetCurrentWeaponAnimator() != null)
        {
            player.inventory.VP_GetCurrentWeaponAnimator().SetBool("Grappling", player.isGrappling);

            player.inventory.VP_GetCurrentWeaponAnimator().SetBool("Melee", player.isMelee);
        }

       
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


