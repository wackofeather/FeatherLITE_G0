//using Steamworks.Ugc;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Inventory : MonoBehaviour
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

    [System.NonSerialized] public WeaponClass currentWeapon;
    private Animator VP_currentAnimator;
    private Animator EXT_currentAnimator;

    public PlayerStateMachine player;

    public GameObject player_WeaponMesh;

    private Dictionary<float, GameObject> VP_weapon_Dict = new Dictionary<float, GameObject>();
    private Dictionary<float, GameObject> EXT_weapon_Dict = new Dictionary<float, GameObject>();
    private Dictionary<float, WeaponClass> EXT_classDict = new Dictionary<float, WeaponClass>();

    [SerializeField] GameObject VP_GunParent;
    [SerializeField] GameObject EXT_GunParent;
    [SerializeField] string viewport_Layer;
    [SerializeField] string exterior_layer;

    [System.NonSerialized] public float internal_CurrentWeapon;


    [HideInInspector] public bool isShooting;
    [HideInInspector] public bool isScoping;
    [HideInInspector] public bool isReloading;


    Vector3 OwnerGunTip;

    public void Start()
    {


        EXT_IntitializeWeapons(EXT_GunParent);

        if (player.networkInfo._isOwner)
        {
            VP_IntitializeWeapons(VP_GunParent);
            
            for (int i = 0; i < WeaponLookup.weaponLookup.Count; i++)
            {
                GiveWeapon(WeaponLookup.weaponLookup[i]);
                Debug.Log("ahhhhh");
            }

            foreach (WeaponClass weaponclass in WeaponLookup.weaponLookup)
            {
                SkinnedMeshRenderer[] ext_meshes = EXT_weapon_Dict[weaponclass.key].GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (SkinnedMeshRenderer renderer in ext_meshes) renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
            foreach (WeaponClass weaponclass in WeaponLookup.weaponLookup)
            {
                SkinnedMeshRenderer[] vp_meshes = VP_weapon_Dict[weaponclass.key].GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (SkinnedMeshRenderer renderer in vp_meshes) renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Camera cam = player.PlayerCamera.GetComponent<Camera>();

            float oldFOV = cam.fieldOfView;

            cam.fieldOfView = player.ViewportFOV;

            // Project the world point to the viewport
            Vector3 viewportPoint = cam.WorldToViewportPoint(player.viewport_gunTip.position);

            // Calculate the distance from the camera to the world point
            float distance = Vector3.Distance(cam.transform.position, player.viewport_gunTip.position);

            // Change the FOV of the camera to a hypothetical value

            cam.fieldOfView = oldFOV;

            // Convert the viewport point back to the world with the new FOV
            OwnerGunTip = cam.ViewportToWorldPoint(new Vector3(viewportPoint.x, viewportPoint.y, distance));
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
       // Debug.Log(IsOwner);
        if (!player.networkInfo._isOwner)
        {
            if (internal_CurrentWeapon == 0) return;
            if (GetCurrentWeapon() == null)
            {
                changeWeapon_Internal(EXT_classDict[internal_CurrentWeapon]);
            }
            if (GetCurrentWeapon().key != internal_CurrentWeapon)
            {
                changeWeapon_Internal(EXT_classDict[internal_CurrentWeapon]);
            }
            //Debug.Log(isScoping);
        }

        //ChangeCurrentWeapon((int)SwitchWeapon.action.ReadValue<float>());;
        // Debug.Log(Weapon_Inventory.Count);
        // if ( != 0) Debug.Log(SwitchWeapon.action.ReadValue<float>());
    }

    public void ChangeCurrentWeapon(int direction)
    {
        if (direction == 0 | Weapon_Inventory.Count == 1) return;
        direction = Mathf.Clamp(direction, -1, 1);
        int shiftMultiplier = 0;

        if (direction > 0) shiftMultiplier = 1;
        if (direction < 0) shiftMultiplier = Weapon_Inventory.Count - 1;

       // Debug.Log(shiftMultiplier);
        for (int i = 1; i < Weapon_Inventory.Count; i++)
        {
            int index = (current_Index + i * shiftMultiplier) % Weapon_Inventory.Count;

            if ((Weapon_Inventory[index]) == null) return;

            current_Index = index;
            changeWeapon_Internal(Weapon_Inventory[index]);

            break;
        }
    }



    public void GiveWeapon(WeaponClass weapon_class)
    {
        if (Weapon_Inventory.Count >= MaxWeapons)
        {
            Weapon_Inventory.RemoveAt(current_Index);
            Destroy(currentWeapon);

        }
        else if (Weapon_Inventory.Count == 0)
        {
            current_Index = 0;
        }
        else
        {
            current_Index = Weapon_Inventory.Count - 1;
        }



        WeaponClass weapon_copy = Instantiate(weapon_class);

        weapon_copy.player = player;
        weapon_copy.inventory = this;

        Weapon_Inventory.Insert(current_Index, weapon_copy);//weapon.clone(Weapon_Inventory.Count + 1));
        weapon_copy.Weapon_Init();
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
            VP_weapon_Dict[currentWeapon.key].SetActive(false);
            EXT_weapon_Dict[currentWeapon.key].SetActive(false);
        }

        currentWeapon = weapon;
        if (player.networkInfo._isOwner)
        {
            VP_currentAnimator = VP_weapon_Dict[currentWeapon.key].GetComponentInChildren<Animator>();
            VP_weapon_Dict[currentWeapon.key].SetActive(true);
            VP_currentAnimator.enabled = true;
        }


        EXT_currentAnimator = EXT_weapon_Dict[currentWeapon.key].GetComponentInChildren<Animator>();
        EXT_weapon_Dict[currentWeapon.key].SetActive(true);
        EXT_currentAnimator.enabled = true;

        currentWeapon.EnterWeapon();

    }

    public void VP_IntitializeWeapons(GameObject parent)
    {
        foreach (WeaponClass weaponclass in WeaponLookup.weaponLookup)
        {
            //Debug.Log(weaponclass.key);
            GameObject gun = Instantiate(weaponclass.weaponData.Proxy_Prefab, parent.transform);
            gun.transform.parent = parent.transform;
            gun.transform.localPosition = Vector3.zero;
            SetLayerWITHChildren(gun.transform, viewport_Layer);
            Animator weapon_animator = gun.GetComponent<WeaponProxy>().animator_Object.AddComponent<Animator>();
            //weapon_animator = player.player_VP_GUN_anim_template;
            weapon_animator.runtimeAnimatorController = weaponclass.weaponData.VM_GUN_animatorOverrideController;
            gun.SetActive(false);
            VP_weapon_Dict.Add(weaponclass.key, gun);
        }
    }
    public void EXT_IntitializeWeapons(GameObject parent)
    {

        foreach (WeaponClass weaponclass in WeaponLookup.weaponLookup)
        {
            //Debug.Log(weaponclass.key);
            GameObject gun = Instantiate(weaponclass.weaponData.Proxy_Prefab);
            gun.transform.parent = parent.transform;
            gun.transform.localPosition = Vector3.zero;
            SetLayerWITHChildren(gun.transform, exterior_layer);
            Animator weapon_animator = gun.GetComponent<WeaponProxy>().animator_Object.AddComponent<Animator>();
            //weapon_animator = player.player_VP_GUN_anim_template;
            weapon_animator.runtimeAnimatorController = weaponclass.weaponData.EXT_GUN_animatorOverrideController;
            gun.SetActive(false);
            EXT_weapon_Dict.Add(weaponclass.key, gun);


            WeaponClass weapon_copy = Instantiate(weaponclass);

            weapon_copy.player = player;
            weapon_copy.inventory = this;

            EXT_classDict.Add(weaponclass.key, weapon_copy);

            
        }
    }



    public WeaponClass GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public Animator VP_GetCurrentWeaponAnimator()
    {
        return VP_currentAnimator;
    }

    public Animator EXT_GetCurrentWeaponAnimator()
    {
        return EXT_currentAnimator;
    }

    public GameObject VP_GetProxy()
    {
        return VP_weapon_Dict[currentWeapon.key];
    }

    public GameObject EXT_GetProxy()
    {
        return EXT_weapon_Dict[currentWeapon.key];
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

    public Vector3 GunTip()
    {
        if (player.networkInfo._isOwner)
        {
            return OwnerGunTip;
        }
        else return player.exterior_gunTip.position;

    }

}
