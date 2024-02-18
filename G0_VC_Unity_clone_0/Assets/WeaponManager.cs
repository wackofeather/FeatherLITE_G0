using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] Player_Inventory inventory;
    [SerializeField] Animator animator;
    [SerializeField] PlayerStateMachine player;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) return;
            
        inventory.GetCurrentWeapon().test_animator = animator;

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        inventory.GetCurrentWeapon().UseWeapon(player);
        inventory.GetCurrentWeapon().Scope(player);
        inventory.GetCurrentWeapon().Animate();
        
    }
}
