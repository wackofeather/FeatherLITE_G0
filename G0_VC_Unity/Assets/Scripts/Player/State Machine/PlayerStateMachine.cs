using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static PlayerBase;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using Unity.Netcode;
using UnityEditor.Animations;

public class PlayerStateMachine : NetworkBehaviour
{
    [System.NonSerialized] public PlayerStateMachine StateMachine;
    [System.NonSerialized] public RegularState RegularState;
    [System.NonSerialized] public GrapplingState GrapplingState;
    [System.NonSerialized] public MeleeState MeleeState;


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
    public Transform Playercamera;
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

    [Header("Animation")]
    public Animator player_anim_controller;
    public bool isGrappling;
    public bool isScoping;

    [Header("VERY IMPORTANT")]
    public float ViewportFOV;

    [System.NonSerialized] public bool allowedToGrapple;

    public RaycastHit GrappleCheck()
    {
        RaycastHit hitinfo;
        Physics.Raycast(Playercamera.position, Playercamera.forward, out hitinfo, maxDistance, whatIsGrappleable);
        return hitinfo;

    }
    public bool CanGrapple()
    {
        if (isGrappling)
        {
            return false;
        }
        else return Physics.Raycast(Playercamera.position, Playercamera.forward, maxDistance, whatIsGrappleable);
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


    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            foreach (GameObject t in OwnerOnlyObjects) t.SetActive(false);
            foreach (Transform child in Viewport) child.gameObject.SetActive(false);
            //this.enabled = false;
            Debug.Log("bruhsushsh");
        }
        else
        {
            foreach (GameObject t in DummyOnlyObjects) t.SetActive(false);
            //foreach (Transform child in Exterior) child.gameObject.SetActive(false);
            Debug.Log("host joined");
            Exterior.GetComponent<ExteriorShadowSwitch>().ShadowsOnly(true);



            StateMachine = this; //gameObject.AddComponent<PlayerStateMachine>();
            RegularState = new RegularState(this);
            GrapplingState = new GrapplingState(this);
            MeleeState = new MeleeState(this);

        }


        VIEWPORT_lr.positionCount = 0;
        EXTERIOR_lr.positionCount = 0;
        if (!IsOwner)
        {
            VIEWPORT_lr.gameObject.SetActive(false);
            return;
        }

        VIEWPORT_lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        EXTERIOR_lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;


        isGrappling = false;

        Initialize(RegularState);

    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        isGrappling = false;
        isScoping = false;
    }

    private void Update()
    {
        CurrentPlayerState.Update();

        //animator
        player_anim_controller.SetBool("Grappling", isGrappling);
        player_anim_controller.SetBool("Scoping", isScoping);
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
        CurrentPlayerState.EnterState();
    }

    public void ChangeState(BasePlayerState newState)
    {
        CurrentPlayerState.ExitState();
        CurrentPlayerState = newState;
        CurrentPlayerState.EnterState();
    }
}
