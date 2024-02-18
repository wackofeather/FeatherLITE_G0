using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static PlayerBase;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using Unity.Netcode;

using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerStateMachine : NetworkBehaviour
{
    [System.NonSerialized] public PlayerStateMachine StateMachine;
    [System.NonSerialized] public RegularState RegularState;
    [System.NonSerialized] public GrapplingState GrapplingState;
    [System.NonSerialized] public MeleeState MeleeState;

    public Dictionary<float, BasePlayerState> stateDictionary = new Dictionary<float, BasePlayerState>();

    [System.NonSerialized] public Vector3 inputVector;

    [System.NonSerialized] public float xRotation = 0;
    [System.NonSerialized] public float yRotation = 0;

    [System.NonSerialized] public Vector3 putTogetherVelocity;

    public Rigidbody rb;
    public InputActionReference move;
    public InputActionReference fire;
    public InputActionReference scope;
    public InputActionReference pause;
    public InputActionReference Unpause;
    public InputActionReference melee;
    public InputActionReference testInput;

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


    [Header("Rope Logic")]

    [System.NonSerialized] public Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    //public Transform Playercamera;
    public float maxDistance = 100f;
    [System.NonSerialized] public SpringJoint joint;
    public InputActionReference Grapple;
    [Range(0f, 100f)]
    public float grappleSpeed;
    public float jointDamper;
    public float jointSpring;
    public float jointMassScale;

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
    public ScriptableRendererFeature rendererData;

    [Header("Melee")]

    public AnimationClip meleeAnim;
    public float meleeSpeed;
    public AnimationCurve meleeCurve;
    public AnimationCurve meleeFOV_curve;
    public bool whichMelee;
    [Range(0,1)] public float meleeWeight_Input;
    [Range(0, 1)] public float meleeWeight_Velocity;


    [Header("Animation")]
    public Animator player_anim_controller;
    public bool isGrappling;
    public bool isScoping;
    public bool isMelee;

    [Header("VERY IMPORTANT")]
    public float ViewportFOV;

    [System.NonSerialized] public bool allowedToGrapple;

    [System.NonSerialized] public float internal_CurrentState;

    [System.NonSerialized] public float grappleWiggle_Timer;

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

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();



        if (!IsOwner)
        {
            foreach (GameObject t in OwnerOnlyObjects) t.SetActive(false);
            foreach (Transform child in Viewport) child.gameObject.SetActive(false);
            //this.enabled = false;
            //Debug.Log("bruhsushsh");


        }
        else
        {
            foreach (GameObject t in DummyOnlyObjects) t.SetActive(false);
            //foreach (Transform child in Exterior) child.gameObject.SetActive(false);
           // Debug.Log("host joined");
            Exterior.GetComponent<ExteriorShadowSwitch>().ShadowsOnly(true);





        }


        StateMachine = this; //gameObject.AddComponent<PlayerStateMachine>();
        RegularState = new RegularState(this);
        GrapplingState = new GrapplingState(this);
        MeleeState = new MeleeState(this);
        stateDictionary.Add(RegularState.key, RegularState);
        stateDictionary.Add(GrapplingState.key, GrapplingState);
        stateDictionary.Add(MeleeState.key, MeleeState);
        //RegularState.Start_Init();
        //GrapplingState.Start_Init();
        //MeleeState.Start_Init();

        VIEWPORT_lr.positionCount = 0;
        EXTERIOR_lr.positionCount = 0;

        Initialize(RegularState);


        if (!IsOwner) Debug.Log("initialized states");

        if (!IsOwner)
        {
            VIEWPORT_lr.gameObject.SetActive(false);
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
        //if (internal_CurrentState == 0) Debug.Log("wahahahahahahahah");
        // if (!IsOwner) Debug.Log(CurrentPlayerState == null);
        CurrentPlayerState.Update();


        if (Grapple.action.triggered)
        {
            grappleWiggle_Timer = 1;
        }
        else
        {
            grappleWiggle_Timer -= Time.deltaTime;
        }

        if (testInput.action.triggered) whichMelee = !whichMelee;
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


    private void OnCollisionEnter(Collision collision)
    {
        CurrentPlayerState.OnCollisionEnter(collision);
    }


}
