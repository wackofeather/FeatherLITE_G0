using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static PlayerBase;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using Unity.Netcode;

using UnityEngine.Rendering.Universal;

using UnityEngine.VFX;
using Unity.Networking.Transport;
using Unity.Netcode.Transports.UTP;
using System.Net.NetworkInformation;
using static PlayerNetwork;
using Steamworks;

public class PlayerStateMachine : MonoBehaviour
{


    [System.NonSerialized] public PlayerStateMachine StateMachine;
    [System.NonSerialized] public RegularState RegularState;
    [System.NonSerialized] public GrapplingState GrapplingState;
    [System.NonSerialized] public MeleeState MeleeState;
    [System.NonSerialized] public WeaponSwitchState WeaponSwitchState;
    [System.NonSerialized] public DeathState DeathState;

    public Dictionary<float, BasePlayerState> stateDictionary = new Dictionary<float, BasePlayerState>();

    [System.NonSerialized] public Vector3 inputVector;

    [System.NonSerialized] public float xRotation = 0;
    [System.NonSerialized] public float yRotation = 0;

    [System.NonSerialized] public Vector3 putTogetherVelocity;

    public Rigidbody rb;
    public InputActionReference move;
    public InputActionReference look;
    public InputActionReference fire;
    public InputActionReference scope;
    public InputActionReference pause;
    public InputActionReference Unpause;
    public InputActionReference melee;
    public InputActionReference interact;
    public InputActionReference copyCode;

    public Transform Rotatables;
    public Transform PlayerCamera;
    public Transform CameraHolder;
    public Transform Exterior;
    public Transform Viewport;

    public List<GameObject> OwnerOnlyObjects;
    public List<GameObject> DummyOnlyObjects;

    public float speed;
    public float mouseSens;
    public float accel;
    public float tooFastaccel;
    public float BreakNeckSpeed;














    /// <summary>
    /// grapple
    /// </summary>



    public GrapplingGun grapplingGunScript;





    public UnityEngine.LineRenderer lineRenderer;




    public bool CanMove;











    [Space(5)]
    [Header("GRAPPLE STUFF")]
    [Space(5)]

    [SerializeField] private float grappleLenience_Time;

    [Header("Rope Logic")]

    [System.NonSerialized] public Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public LayerMask movingGrapplableLayers;
    //public Transform Playercamera;
    public float maxDistance = 100f;
    [System.NonSerialized] public SpringJoint joint;
    public InputActionReference Grapple;
    [Range(0f, 2000f)]
    public float grappleSpeed;
    public float jointDamper;
    public float jointSpring;
    public float jointMassScale;
    public AnimationCurve jointSpringCurve;

    [Header("Rope Graphics")]


    public int quality;
    public float damper;
    public float strength;
    public float velocity;
    public float waveCount;
    public float waveHeight;
    public float ropeSpeed;
    public AnimationCurve affectCurve;
    public AnimationCurve retractCurve;
    public float retractSpeed;
    [Range(0, 1)]
    public float lookAwayLeniency;
    [System.NonSerialized] public Spring spring;
    public LineRenderer VIEWPORT_lr;
    public LineRenderer EXTERIOR_lr;
    [System.NonSerialized] public Vector3 EXTERIOR_currentGrapplePosition;
    [System.NonSerialized] public Vector3 VIEWPORT_currentGrapplePosition;
    public Transform exterior_gunTip;
    public Transform viewport_gunTip;
    public GameObject VIEWPORT_grappleHand;
    public GameObject EXTERIOR_grappleHand;
    public List<RenderObjects> ViewportRenderers;

    [Header("Melee")]

    public AnimationClip meleeAnim;
    public float meleeSpeed;
    public AnimationCurve meleeCurve;
    public AnimationCurve meleeFOV_curve;
    [Range(0,1)] public float meleeWeight_Input;
    [Range(0, 1)] public float meleeWeight_Velocity;
    public Collider BumpCollider;


    [Header("Animation")]
    public Animator player_VP_ARM_anim_controller;
    public Animator player_EXT_ARM_anim_controller;
    public bool isGrappling;
    public bool isScoping;
    public bool isMelee;
    [HideInInspector] public float updown_Blendconstant;

    [Header("VERY IMPORTANT")]
    public float ViewportFOV;
    public Player_Inventory inventory;
    public string EnemyLayer;
    [System.NonSerialized] public bool allowedToGrapple;

    [System.NonSerialized] public float internal_CurrentState;

    [System.NonSerialized] public float grappleWiggle_Timer;

    [Header("INTERACTABLES")]
    public LayerMask interactableMask;
    [HideInInspector] public bool isInteracting;
    public float interactDistance;
    public float InteractCoolDownTimer;
    [HideInInspector] public bool hasPickedUpInteractButton;

    [Header("WEAPON PICKUP PARAMETERS")]
    public float slowDownRate;
    [Range(0,1)] public float pushFactor;
    public float maxPushSpeed;
    public float pickupCooldownTime;
    [System.NonSerialized] public GameObject lookAtObject;
    [System.NonSerialized] public WeaponClass weapon_pickingUp;


    [Header("SPECIAL UI")]
    public VisualEffect windEffect;
    public float windSpawnRate;


    
    [HideInInspector] public PlayerNetwork playerNetwork;


    [Header("Health")]
    public int health;
    public GameObject DamageCollider;
    public Transform ExtHealthBarLocation;
    [HideInInspector] public GameObject extHealthBar;

    public NetworkInfo networkInfo;

    float deleteTimer = 5;
    

    public RaycastHit GrappleCheck()
    {
        RaycastHit hitinfo;
        Physics.Raycast(PlayerCamera.position, PlayerCamera.forward, out hitinfo, maxDistance, whatIsGrappleable);
        return hitinfo;

    }
    public bool CanGrapple()
    {
        if (isGrappling)
        {
            return false;
        }
        else return Physics.Raycast(PlayerCamera.position, PlayerCamera.forward, maxDistance, whatIsGrappleable);
    }






    [System.NonSerialized] public BasePlayerState CurrentPlayerState;





    public void _Destroy(Object obj)
    {
        Destroy(obj);
    }

    public void _StartCoroutine(IEnumerator obj)
    {
        StartCoroutine(obj);
    }

    public void _StopCoroutine(IEnumerator obj)
    {
        StopCoroutine(obj);
    }

    public void _SetActive(GameObject obj, bool boolean)
    {
        obj.SetActive(boolean);
    }

    private void Awake()
    {
        spring = new Spring();
        spring.SetTarget(0);
    }

    private void Start()
    {
        if (networkInfo._isOwner) { StartCoroutine(Testcoroutine()); }
    }

    IEnumerator Testcoroutine()
    {
        //yield return new WaitForSeconds(1);

        //Game_GeneralManager.instance.AddPlayerServerRPC(NetworkObject.NetworkObjectId, NetworkObject);

        yield break;
    }

    public void Player_OnNetworkSpawn(bool reconnecting)
    {

        Debug.LogWarning("kumalala" + networkInfo._isOwner + "  " + reconnecting);
        if (reconnecting)
        {
            if (!networkInfo._isOwner)
            {
                Debug.LogWarning("yababadoo");
                //Game_UI_Manager.instance.AddHealthBarToPlayer(this);
            }
            else
            {
                foreach (InputActionMap map in move.asset.actionMaps)
                {
                    map.Enable();
                }

                /*GetComponent<PlayerInput>().enabled = false;
                GetComponent<PlayerInput>().enabled = true;*/
            }
            return;
        }
        //base.OnNetworkSpawn();

        //Debug.LogWarning("ahsgajsgdsbdjdfififofofoofofofofofof" + networkInfo._isOwner);

        if (!networkInfo._isOwner)
        {
            foreach (GameObject t in OwnerOnlyObjects) t.SetActive(false);
            foreach (Transform child in Viewport) child.gameObject.SetActive(false);
            //this.enabled = false;
            //Debug.Log("bruhsushsh");


            //gameObject.layer = LayerMask.NameToLayer(EnemyLayer);



        }
        else
        {
            foreach (GameObject t in DummyOnlyObjects) t.SetActive(false);
            //foreach (Transform child in Exterior) child.gameObject.SetActive(false);
           // Debug.Log("host joined");
            Exterior.GetComponent<ExteriorShadowSwitch>().ShadowsOnly(true);
            //StartCoroutine(Testcoroutine());

        }


        StateMachine = this; //gameObject.AddComponent<PlayerStateMachine>();
        RegularState = new RegularState(this);
        GrapplingState = new GrapplingState(this);
        MeleeState = new MeleeState(this);
        WeaponSwitchState = new WeaponSwitchState(this);
        DeathState = new DeathState(this);
        stateDictionary.Add(RegularState.key, RegularState);
        stateDictionary.Add(GrapplingState.key, GrapplingState);
        stateDictionary.Add(MeleeState.key, MeleeState);
        stateDictionary.Add(WeaponSwitchState.key, WeaponSwitchState);
        stateDictionary.Add(DeathState.key, DeathState);
        
        //RegularState.Start_Init();
        //GrapplingState.Start_Init();
        //MeleeState.Start_Init();

        VIEWPORT_lr.positionCount = 0;
        EXTERIOR_lr.positionCount = 0;

        Initialize(RegularState);


        if (!networkInfo._isOwner) Debug.Log("initialized states");

        if (!networkInfo._isOwner)
        {
            VIEWPORT_lr.gameObject.SetActive(false);
            Game_UI_Manager.game_instance.AddHealthBarToPlayer(this);
            return;
        }

        VIEWPORT_lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        EXTERIOR_lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

        isGrappling = false;
        isScoping = false;
        isMelee = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }



    private void Update()
    {
        //Debug.Log(move.action.ReadValue<Vector3>());
        //if (internal_CurrentState == 0) Debug.Log("wahahahahahahahah");
        // if (!IsOwner) Debug.Log(CurrentPlayerState == null);
        CurrentPlayerState.Update();


        if (Grapple.action.triggered)
        {
            grappleWiggle_Timer = grappleLenience_Time;
        }
        else
        {
            grappleWiggle_Timer -= Time.deltaTime;
        }

        if (InteractCoolDownTimer > 0)
        {
            InteractCoolDownTimer -= Time.deltaTime;
        }

        if (hasPickedUpInteractButton == false)
        {
            if (!interact.action.IsPressed()) hasPickedUpInteractButton = true;
        }

        if (copyCode.action.triggered)
        {
            GUIUtility.systemCopyBuffer = SteamLobbyManager.currentLobby.Id.ToString();
        }

        if (playerNetwork != null)
        {
            deleteTimer = 5;
        }
        else
        {
            deleteTimer -= Time.deltaTime;
        }
        if (deleteTimer < 0)
        {
            if (Game_GeneralManager.instance.reconnecting) return;
            Player_OnNetworkDespawn();
            Player_OnDisconnect();

            if (networkInfo._isOwner) SteamLobbyManager.instance.RevertToMenu();

            Destroy(this.gameObject);
            
        }
    }

    private void FixedUpdate()
    {
        CurrentPlayerState.FixedUpdate();
        
    }

    private void LateUpdate()
    {
        CurrentPlayerState.LateUpdate();
    }









    public void Initialize(BasePlayerState startingState)
    {
        foreach (BasePlayerState state in stateDictionary.Values) state.InitializeState(); 
        CurrentPlayerState = startingState;
        internal_CurrentState = startingState.key;
        CurrentPlayerState.EnterState();
    }

    public void ChangeState(BasePlayerState newState)
    {
        CurrentPlayerState.ExitState();
        CurrentPlayerState = newState;
        CurrentPlayerState.EnterState();
    }


    private void OnCollisionStay(Collision collision)
    {
        //Debug.LogAssertion("ahhhhhhhhhhhhhhhh");
        CurrentPlayerState.OnCollisionStay(collision);
    }

/*    private void OnCollisionStay(Collision collision)
    {
        Debug.Log(collision.contactCount);
    }*/












    public void InteractCheck()
    {
        if (((InteractCoolDownTimer > 0) && hasPickedUpInteractButton == false) || isInteracting) return;
        if (interact.action.IsPressed())
        {
            if (Physics.Raycast(PlayerCamera.position, PlayerCamera.forward, out RaycastHit hitInfo, 10000, interactableMask))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj) && (hitInfo.distance < interactDistance))
                {
                    interactObj.Interact(this);
                }
            }
        }
    }

    public bool IsLookingAtInteractable(GameObject obj)
    {
        Physics.Raycast(PlayerCamera.position, PlayerCamera.forward, out RaycastHit raycasthit, 10000, interactableMask);
        
        if (raycasthit.collider == null) return false;
        return (raycasthit.collider.gameObject == obj);
    }

    public Vector3 FOVtranslate(Transform obj, float medianFOV, float endFOV)
    {
        Camera cam = PlayerCamera.GetComponent<Camera>();

        

        cam.fieldOfView = medianFOV;

        // Project the world point to the viewport
        Vector3 viewportPoint = cam.WorldToViewportPoint(obj.position);

        // Calculate the distance from the camera to the world point
        ///////////////////////////////////////////////////////////float distance = Vector3.Distance(cam.transform.position, player.viewport_gunTip.position);
        float distance = Vector3.Project(obj.position - cam.transform.position, cam.transform.forward).magnitude;

        // Change the FOV of the camera to a hypothetical value

        cam.fieldOfView = endFOV;

        // Convert the viewport point back to the world with the new FOV
        return cam.ViewportToWorldPoint(new Vector3(viewportPoint.x, viewportPoint.y, distance));
    }

/*    public void OnClientDisconnect()
    {
        Debug.Log("client disconnected");
    }*/

    public void Player_OnNetworkDespawn()
    {

       // if (IsOwner) Game_GeneralManager.instance.RemovePlayerServerRPC(NetworkObject.NetworkObjectId, NetworkObject);
        Debug.LogAssertion("closingnetwork");

        //Destroy(extHealthBar);

        //base.OnNetworkDespawn();

        //SteamLobbyManager.instance.LeaveLobby();



    }

    public void Player_OnDisconnect()
    {
        Destroy(extHealthBar);
    }





    /* public void SetHealth(int _health)
     {
         health = _health;
     }

     [Rpc(SendTo.SpecifiedInParams)]
     public void SetHealthRPC(int _health, RpcParams _params)
     {
         health = _health;
     }

     public void Damage(int damage)
     {
 *//*        if (health < 0)
         {
             SendDamageOwnerRPC(damage, OwnerClientId, true);

             return;
         }*//*
         health -= damage;
         if (health < 0)
         {
             KillPlayerRPC();
         }
         SendDamageOwnerRPC(damage, OwnerClientId);
     }

     *//*    [Rpc(SendTo.NotMe)]
         void SendDamageRPC(int damage)
         {
             health -= damage;
         }*//*

     [Rpc(SendTo.Owner)]
     void SendDamageOwnerRPC(int damage, ulong damagerID)
     {

         health -= damage;
         SetHealthRPC(health, RpcTarget.Single(damagerID, RpcTargetUse.Temp));
         ulong[] IDS = new ulong[2];
         IDS[0] = damagerID;
         IDS[1] = OwnerClientId;
         RelayDamageRPC(damage, RpcTarget.Not(IDS, RpcTargetUse.Temp));
     }

     [Rpc(SendTo.SpecifiedInParams)]
     void RelayDamageRPC(int damage, RpcParams _params)
     {
         health -= damage;
     }*/

    [Rpc(SendTo.Owner)]
    public void KillPlayerRPC()
    {
        if (CurrentPlayerState != DeathState) Game_GeneralManager.instance.Kill(this);
    }
}
