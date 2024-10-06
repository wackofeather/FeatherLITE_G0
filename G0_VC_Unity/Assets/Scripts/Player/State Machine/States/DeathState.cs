using UnityEngine;

public class DeathState : BasePlayerState
{

    public DeathState(PlayerStateMachine player) : base(player)
    {
        key = 5;
    }

    public override void EnterState()
    {

        player.health = 0;

        player.inventory.isScoping = false;
        player.inventory.isShooting = false;

        base.EnterState();
        

    }

    public override void Update()
    {
        return;
    }

    public override void LateUpdate()
    {
        return;
    }

    public override void FixedUpdate()
    {
        return;
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();



        player.player_EXT_ARM_anim_controller.SetBool("Scoping", player.inventory.isScoping);

        player.player_EXT_ARM_anim_controller.SetBool("Firing", player.inventory.isShooting);

        player.inventory.EXT_GetCurrentWeaponAnimator().SetBool("Scoping", player.inventory.isScoping);

        player.inventory.EXT_GetCurrentWeaponAnimator().SetBool("Firing", player.inventory.isShooting);

        if (!player.IsOwner) return;

        player.player_VP_ARM_anim_controller.SetBool("Scoping", player.inventory.isScoping);

        player.player_VP_ARM_anim_controller.SetBool("Firing", player.inventory.isShooting);

        player.inventory.VP_GetCurrentWeaponAnimator().SetBool("Scoping", player.inventory.isScoping);

        player.inventory.VP_GetCurrentWeaponAnimator().SetBool("Firing", player.inventory.isShooting);
    }
}
