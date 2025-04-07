using System.Collections;
using UnityEngine;

public class DeathState : BasePlayerState
{
    bool verifyingDeath;
    float verifyDeathTimer;
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
            player.PlayerCamera.gameObject.SetActive(false);
        }
        else player.StartCoroutine(VerifyDeath(3));


        base.EnterState();

        
    }

    IEnumerator VerifyDeath(float timerTime)
    {
        verifyingDeath = true;
        verifyDeathTimer = timerTime;

        while (verifyDeathTimer > 0)
        {

            if (!verifyingDeath) yield break;
            if (player.internal_CurrentState == key) { verifyingDeath = false; yield break; }

            verifyDeathTimer -= Time.deltaTime;
            yield return null;
        }

        player.ChangeState(player.stateDictionary[player.internal_CurrentState]);

        verifyingDeath = false;
        yield break;
    }

    public override void Update()
    {
        if (!player.networkInfo._isOwner)
        {
            player.playerNetwork.ConsumeState();

            if (player.internal_CurrentState != key && !verifyingDeath) player.ChangeState(player.stateDictionary[player.internal_CurrentState]);

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

    public override void ExitState()
    {
        base.ExitState();

        if (player.networkInfo._isOwner) player.PlayerCamera.gameObject.SetActive(true);
    }
}
