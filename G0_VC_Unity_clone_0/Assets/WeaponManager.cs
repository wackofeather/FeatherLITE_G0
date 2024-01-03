using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] Player_Inventory inventory;
    [SerializeField] Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        inventory.GetCurrentWeapon().test_animator = animator;
    }

    // Update is called once per frame
    void Update()
    {
        inventory.GetCurrentWeapon().UseWeapon();
        inventory.GetCurrentWeapon().Scope();
        inventory.GetCurrentWeapon().Animate();
        
    }
}
