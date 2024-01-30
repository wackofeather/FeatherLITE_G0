using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerBase : NetworkBehaviour
{
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected InputActionReference move;
    [SerializeField] protected InputActionReference fire;
    [SerializeField] protected InputActionReference scope;
    [SerializeField] protected InputActionReference pause;
    [SerializeField] protected InputActionReference Unpause;

    [SerializeField] protected Transform Rotatables;
    [SerializeField] protected Transform PlayerCamera;
    [SerializeField] protected Transform CameraHolder;
    [SerializeField] protected Transform Exterior;
    [SerializeField] protected Transform Viewport;

    [SerializeField] protected List<GameObject> OwnerOnlyObjects;
    [SerializeField] protected List<GameObject> DummyOnlyObjects;

    [SerializeField] protected float speed;
    [SerializeField] protected float mouseSens;
    [SerializeField] protected float accel;
    [SerializeField] protected float tooFastaccel;
    [SerializeField] protected float BreakNeckSpeed;






    protected Vector3 inputVector;

    protected float xRotation = 0;
    protected float yRotation = 0;

    protected Vector3 putTogetherVelocity;







    /// <summary>
    /// grapple
    /// </summary>



    [SerializeField] protected GrapplingGun grapplingGunScript;





    [SerializeField] protected UnityEngine.LineRenderer lineRenderer;




    public bool CanMove;


    [Space(5)]
    [Header("GRAPPLE STUFF")]
    [Space(5)]


    [Header("Rope Logic")]

    protected Vector3 grapplePoint;
    protected LayerMask whatIsGrappleable;
    protected Transform Playercamera;
    protected Rigidbody player;
    protected float maxDistance = 100f;
    protected SpringJoint joint;
    [SerializeField] protected InputActionReference Grapple;
    [Range(0f, 100f)]
    [SerializeField] protected float grappleSpeed;
    [SerializeField] protected float jointDamper;
    [SerializeField] protected float jointSpring;
    [SerializeField] protected float jointMassScale;

    [Header("Rope Graphics")]


    protected int quality;
    protected float damper;
    protected float strength;
    protected float velocity;
    protected float waveCount;
    protected float waveHeight;
    [SerializeField] protected float ropeSpeed;
    protected AnimationCurve affectCurve;
    protected AnimationCurve retractCurve;
    [SerializeField] protected float retractSpeed;
    protected bool isGrappling;
    [Range(0, 1)]
    [SerializeField] protected float lookAwayLeniency;
    protected Spring spring;
    [SerializeField] protected LineRenderer VIEWPORT_lr;
    [SerializeField] protected LineRenderer EXTERIOR_lr;
    protected Vector3 EXTERIOR_currentGrapplePosition;
    protected Vector3 VIEWPORT_currentGrapplePosition;
    [SerializeField] protected Transform exterior_gunTip;
    [SerializeField] protected Transform viewport_gunTip;
    [SerializeField] protected GameObject VIEWPORT_grappleHand;
    [SerializeField] protected GameObject EXTERIOR_grappleHand;

    [Header("VERY IMPORTANT")]
    [SerializeField] float ViewportFOV;

    protected bool allowedToGrapple;






    protected bool CanGrapple(out RaycastHit hit)
    {
        if (isGrappling | !allowedToGrapple)
        {
            hit = default;
            return false;
        }
        else
        {
            RaycastHit hitinfo;
            bool canGrapple = Physics.Raycast(Playercamera.position, Playercamera.forward, out hitinfo, maxDistance, whatIsGrappleable);
            hit = hitinfo;
            return canGrapple;
        }

    }
    protected bool CanGrapple()
    {
        if (isGrappling)
        {
            return false;
        }
        else return Physics.Raycast(Playercamera.position, Playercamera.forward, maxDistance, whatIsGrappleable);
    }



}
