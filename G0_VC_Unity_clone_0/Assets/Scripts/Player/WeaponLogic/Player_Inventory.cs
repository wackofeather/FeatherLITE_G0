using Steamworks.Ugc;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class Player_Inventory : NetworkBehaviour
{
    [SerializeField] WeaponLookup WeaponLookup;
    [SerializeField] int MaxWeapons;
    public List<WeaponClass> Weapon_Inventory = new List<WeaponClass>();
    //private GameObject currentWeapon;
    private int current_Index;
    // [SerializeField] WeaponClass testWeapon;
    //public List<WeaponClass> Test_List;
    public InputActionReference SwitchWeapon;
    [SerializeField] WeaponManager WeaponManager;

    private WeaponClass currentWeapon;
    private Animator currentAnimator;

    [SerializeField] PlayerStateMachine player;

    public GameObject player_WeaponMesh;

    private Dictionary<float, GameObject> weapon_Dict = new Dictionary<float, GameObject>();

    [SerializeField] GameObject GunParent;
    [SerializeField] string viewport_Layer;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) return;



        IntitializeWeapons(GunParent);
        for (int i = 0; i < WeaponLookup.weaponLookup.Count; i++)
        {
            GiveWeapon(WeaponLookup.weaponLookup[i]);
            Debug.Log("ahhhhh");
        }
        //  currentWeapon = Weapon_Inventory[0];
        //ChangeCurrentWeapon_INDEX(0);
    }
    // Start is called before the first frame update
    void Awake()
    {
        //currentWeapon = Weapon_Inventory[0];

    }

    private void Update()
    {
        //ChangeCurrentWeapon((int)SwitchWeapon.action.ReadValue<float>());;
        // Debug.Log(Weapon_Inventory.Count);
        // if ( != 0) Debug.Log(SwitchWeapon.action.ReadValue<float>());
    }

    public void ChangeCurrentWeapon(int direction)
    {
        if (direction == 0 | Weapon_Inventory.Count == 1) return;
        direction = Mathf.Clamp(direction, -1, 1);
        for (int i = 1; i < Weapon_Inventory.Count; i++)
        {
            int index = Mathf.Abs((current_Index + i * direction)) % Weapon_Inventory.Count;

            if ((Weapon_Inventory[index]) == null) return;

            current_Index = index;
            changeWeapon_Internal(Weapon_Inventory[index]);

        }
    }



    public void GiveWeapon(WeaponClass weapon_class)
    {
        if (Weapon_Inventory.Count >= MaxWeapons)
        {
            return; //eventually do swapping weapons
        }

        current_Index = Weapon_Inventory.Count - 1;

        WeaponClass weapon_copy = Instantiate(weapon_class);

        weapon_copy.player = player;
        weapon_copy.inventory = this;

        Weapon_Inventory.Add(weapon_copy);//weapon.clone(Weapon_Inventory.Count + 1));
        changeWeapon_Internal(weapon_copy);

        /* for (int i = 0; i < Weapon_Inventory.Count; i++)
         {
             if (Weapon_Inventory[i] == null)
             {
                 Weapon_Inventory[i] = weapon;
                 ChangeCurrentWeapon_INDEX(i);
                 break;
             }
         }*/
    }


    public void ChangeCurrentWeapon_INDEX(int index)
    {
        //Debug.Log("ahhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhsksjskshskshskshsks");
        changeWeapon_Internal(Weapon_Inventory[index]);
        current_Index = index;

    }

    public void changeWeapon_Internal(WeaponClass weapon)
    {
        if (currentWeapon != null)
        {
            currentWeapon.ExitWeapon();
            weapon_Dict[currentWeapon.key].SetActive(false);
        }

        currentWeapon = weapon;
        currentAnimator = weapon_Dict[currentWeapon.key].GetComponentInChildren<Animator>();
        weapon_Dict[currentWeapon.key].SetActive(true);
        currentAnimator.enabled = true;
        currentWeapon.EnterWeapon();

    }

    public void IntitializeWeapons(GameObject parent)
    {
        foreach (WeaponClass weaponclass in WeaponLookup.weaponLookup)
        {
            //Debug.Log(weaponclass.key);
            GameObject gun = Instantiate(weaponclass.weaponData.weaponMesh);
            gun.transform.parent = parent.transform;
            gun.transform.localPosition = Vector3.zero;
            SetLayerWITHChildren(gun.transform, viewport_Layer);
            Animator weapon_animator = gun.AddComponent<Animator>();
            //weapon_animator = player.player_VP_GUN_anim_template;
            weapon_animator.runtimeAnimatorController = weaponclass.weaponData.VM_GUN_animatorOverrideController;
            gun.SetActive(false);
            weapon_Dict.Add(weaponclass.key, gun);
        }
    }



    public WeaponClass GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public Animator GetCurrentWeaponAnimator()
    {
        return currentAnimator;
    }

    void SetLayerWITHChildren(Transform root, string layer)
    {
        var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
        root.gameObject.layer = LayerMask.NameToLayer(layer);
        foreach (var child in children)
        {
            //Debug.Log(child.name);
            child.gameObject.layer = LayerMask.NameToLayer(layer);
        }
    }
}
