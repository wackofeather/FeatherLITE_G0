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
        //if (!player.networkInfo._isOwner) Debug.Log(player.inventory.GetCurrentWeapon());
        player.player_EXT_ARM_anim_controller.SetFloat("Y_Look", player.updown_Blendconstant);
        player.inventory.EXT_GetCurrentWeaponAnimator().SetFloat("Y_Look", player.updown_Blendconstant);

        if (!player.networkInfo._isOwner)
        {
            //player.playerNetwork.ConsumeState();
            
            if (player.internal_CurrentState != key) player.ChangeState(player.stateDictionary[player.internal_CurrentState]);

            if (player.inventory.GetCurrentWeapon() != null) { player.inventory.GetCurrentWeapon().Weapon_Update(); }
            return;
        }

        player.inputVector = player.move.action.ReadValue<Vector3>();
        float mouseX = player.look.action.ReadValue<Vector2>().x;//Input.GetAxis("Mouse X") * player.mouseSens;

        float mouseY = player.look.action.ReadValue<Vector2>().y;//Input.GetAxis("Mouse Y") * player.mouseSens;

        Vector2 closerRecoil = Vector2.Lerp(player.appliedRecoil, player.totalRecoil, 20 * Time.deltaTime);
        Vector2 recoil_ToAdd = closerRecoil - player.appliedRecoil;
        player.xRotation -= mouseY + recoil_ToAdd.y;
        player.yRotation += mouseX + recoil_ToAdd.x;
        player.appliedRecoil = closerRecoil;
        if (Vector2.Distance(player.appliedRecoil, player.totalRecoil) < 0.01f)
        {
            player.totalRecoil -= player.appliedRecoil;
            player.appliedRecoil -= player.appliedRecoil;
        }
        player.xRotation = Mathf.Clamp(player.xRotation, -90, 90);


        player.Rotatables.localRotation = Quaternion.Euler(player.xRotation, player.yRotation, 0);
        player.Exterior.localRotation = Quaternion.Euler(0, player.yRotation, 0);
        player.PlayerCamera.localRotation = Quaternion.Euler(player.xRotation, player.yRotation, 0);
        player.updown_Blendconstant = (player.xRotation + 90) / 180;

        if (player.melee.action.triggered && player.CurrentPlayerState != player.MeleeState) player.ChangeState(player.MeleeState);

        //player.playerNetwork.TransmitState();
        if (!player.isMelee && player.inventory.SwitchWeapon.action.triggered) player.inventory.ChangeCurrentWeapon((int)player.inventory.SwitchWeapon.action.ReadValue<float>());
        if (player.inventory.GetCurrentWeapon() != null) { player.inventory.GetCurrentWeapon().Weapon_Update(); }
    }
    public virtual void FixedUpdate()
    {
        if (player.playerNetwork != null)
        {
            if (!player.networkInfo._isOwner)
            {
                player.playerNetwork.FixedConsumeState();
            }
        }

        //Debug.LogWarning(player.rb.linearVelocity.magnitude);

    }

    public virtual void LateUpdate()
    {
        if (player.playerNetwork != null)
        {
            if (!player.networkInfo._isOwner)
            {
                player.playerNetwork.ConsumeState();
            }
            else player.playerNetwork.TransmitState();
        }
        if (!player.networkInfo._isOwner)
        {
            //Debug.LogAssertion("hoorah");
            Game_UI_Manager.game_instance.UpdateDummyHealth(player);
            return;
        }
        Game_UI_Manager.game_instance.UpdateGrappleIndicator(player.CanGrapple());
        Game_UI_Manager.game_instance.UpdateHealth(player.health);
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
        
        if (!player.networkInfo._isOwner) return;

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

    public virtual void OnCollisionStay(Collision col)
    {

    }

    public virtual void OnCollisionExit(Collision col) 
    { 
    
    }

    public virtual void OnBumpPlayer(Collider col)
    {

    }

}


