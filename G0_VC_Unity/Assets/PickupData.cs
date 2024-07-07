using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupData : MonoBehaviour, IInteractable
{
    private bool isCoroutineRunning;
    private int frameTimer;
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
    public void LookInteract()
    {
        if (isCoroutineRunning == false) StartCoroutine(LookInteractCoroutine()); 
        else
        {
            frameTimer = 1;
        }
    }

    IEnumerator LookInteractCoroutine()
    {
        isCoroutineRunning = true;
        frameTimer = 1;
        while (true)
        {
            if (frameTimer == 0)
            {
                Game_UI_Manager.instance.UpdateWeaponPickUI(null);
                isCoroutineRunning = false;
                break;
            }
            if (frameTimer == 1)
            {
                Game_UI_Manager.instance.UpdateWeaponPickUI(correspondingWeapon.weaponData.gunPickUpImage);
                frameTimer = 0;
            }
            yield return null;
        }
        
        
        
        
        
        
        yield break;
    }
}
