using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupData : MonoBehaviour, IInteractable
{
    [SerializeField] WeaponClass correspondingWeapon;

    public WeaponClass GetWeapon()
    {
        return correspondingWeapon;
    }

    public void Interact(PlayerStateMachine player)
    {
        player.weapon_pickingUp = correspondingWeapon;
        player.lookAtObject = gameObject;
        player.ChangeState(player.WeaponSwitchState);
    }
}
