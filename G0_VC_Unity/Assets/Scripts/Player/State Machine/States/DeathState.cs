using UnityEngine;

public class DeathState : BasePlayerState
{

    public DeathState(PlayerStateMachine player) : base(player)
    {
        key = 5;
    }

    public override void EnterState()
    {

        //player.health = 0;
        if (player.networkInfo._isOwner)
        {
            player.inventory.isScoping = false;
            player.inventory.isShooting = false;
        }


        base.EnterState();
        

    }

    public override void Update()
    {
        if (!player.networkInfo._isOwner)
        {
            player.playerNetwork.ConsumeState();

            if (player.internal_CurrentState != key) player.ChangeState(player.stateDictionary[player.internal_CurrentState]);

            if (player.inventory.GetCurrentWeapon() != null) { player.inventory.GetCurrentWeapon().Weapon_Update(); }
            return;
        }
    }

/*    public override void LateUpdate()
    {
        return;
    }*/

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();



        player.player_EXT_ARM_anim_controller.SetBool("Scoping", player.inventory.isScoping);

        player.player_EXT_ARM_anim_controller.SetBool("Firing", player.inventory.isShooting);

        player.inventory.EXT_GetCurrentWeaponAnimator().SetBool("Scoping", player.inventory.isScoping);

        player.inventory.EXT_GetCurrentWeaponAnimator().SetBool("Firing", player.inventory.isShooting);

        if (!player.networkInfo._isOwner) return;

        player.player_VP_ARM_anim_controller.SetBool("Scoping", player.inventory.isScoping);

        player.player_VP_ARM_anim_controller.SetBool("Firing", player.inventory.isShooting);

        player.inventory.VP_GetCurrentWeaponAnimator().SetBool("Scoping", player.inventory.isScoping);

        player.inventory.VP_GetCurrentWeaponAnimator().SetBool("Firing", player.inventory.isShooting);
    }
}
