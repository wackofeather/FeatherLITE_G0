using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using static UnityEngine.GraphicsBuffer;
using System.Drawing;
public class GrapplingGun : NetworkBehaviour {


    //method overloading with multiple versions of bool

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

    void Awake()
    {

        spring = new Spring();
        spring.SetTarget(0);

    }

    private void Start()
    {


        if (!IsOwner)
        {
            VIEWPORT_lr.gameObject.SetActive(false);
            return;
        }

        VIEWPORT_lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        EXTERIOR_lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;


        isGrappling = false;

    }

    void Update() 
    {
        if (!IsOwner)
        {
            return;
        }

        if (Grapple.action.triggered && !isGrappling) StartGrapple();

    }

    void LateUpdate()
    {
        DrawRope();
    }




    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple() {
        RaycastHit hit;
        if (CanGrapple(out hit)) 
        {
            grapplePoint = hit.point;
            Vector3 relativeVelocity = Playercamera.InverseTransformVector(player.linearVelocity);
            if (relativeVelocity.z < 0) relativeVelocity.z = 0f;
            player.linearVelocity = Playercamera.rotation * relativeVelocity;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 1f;
            joint.minDistance = 0.25f; //distanceFromPoint * 0.25f;

            //Adjust these values to fit your game.
            joint.spring = jointSpring;
            joint.damper = jointDamper;
            joint.massScale = jointMassScale;

            isGrappling = true;

            StartCoroutine(GrappleCoroutine());
        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>

    public IEnumerator GrappleCoroutine()
    {
        while (true)
        {
            if (!isGrappling) yield break;

            float ClampedDistance = (player.position - grapplePoint).magnitude / maxDistance;
            float Angle = Vector3.Angle(Playercamera.transform.forward, (grapplePoint - Playercamera.position));

            RaycastHit hit;
            Physics.Raycast(Playercamera.position, (grapplePoint - Playercamera.position), out hit, maxDistance, whatIsGrappleable);


            if (Angle > Playercamera.GetComponent<Camera>().fieldOfView * (1 + lookAwayLeniency)) break;

            if ((grapplePoint - hit.point).magnitude >= 0.1f) break;
            

            if (Grapple.action.WasReleasedThisFrame()) break;
            

            if (Vector3.Distance(player.position, grapplePoint) > 0.25f)
            {
                //if (Vector3.Distance(player.position, grapplePoint) < joint.maxDistance - grappleSpeed * Time.deltaTime) joint.maxDistance = Vector3.Distance(player.position, grapplePoint);
                joint.maxDistance -= grappleSpeed * Time.deltaTime;
            }

            yield return null;
        }
        StopGrapple();
        yield break;
    }

    public void StopGrapple()
    {
        if (!isGrappling) return;

        Destroy(joint);


        spring.Reset();
        spring.SetVelocity(velocity);
        isGrappling = false;
    }

    public bool IsGrappling() {
        return isGrappling;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }






   





    void DrawRope()
    {

        if (IsOwner)
        {
            VIEWPORT_grappleHand.SetActive(isGrappling);
            EXTERIOR_grappleHand.SetActive(isGrappling);
            if (!isGrappling)
            {
                EXTERIOR_currentGrapplePosition = exterior_gunTip.position;
                VIEWPORT_currentGrapplePosition = viewport_gunTip.position;
                spring.Reset();
                if (VIEWPORT_lr.positionCount > 0)
                    VIEWPORT_lr.positionCount = 0;
                if (EXTERIOR_lr.positionCount > 0)
                    EXTERIOR_lr.positionCount = 0;
                return;
            }

            if (EXTERIOR_lr.positionCount == 0)
            {
                spring.SetVelocity(velocity);
                EXTERIOR_lr.positionCount = quality + 1;
                VIEWPORT_lr.positionCount = quality + 1;
            }

            spring.SetDamper(damper);
            spring.SetStrength(strength);
            spring.Update(Time.deltaTime);


            Vector3 VIEWPORT_up = Quaternion.LookRotation((grapplePoint - viewport_gunTip.position).normalized) * Vector3.up;

            VIEWPORT_currentGrapplePosition = Vector3.Lerp(VIEWPORT_currentGrapplePosition, grapplePoint, Time.deltaTime * ropeSpeed);

            ///NOTE: to optimize try using localPos and caching the converted gun tip so no need for calculation every frame. You have to use local pos so it works properly when reused over time tho


            Camera cam = Playercamera.GetComponent<Camera>();

            float oldFOV = cam.fieldOfView;

            cam.fieldOfView = ViewportFOV;

            // Project the world point to the viewport
            Vector3 viewportPoint = cam.WorldToViewportPoint(viewport_gunTip.position);

            // Calculate the distance from the camera to the world point
            float distance = Vector3.Distance(cam.transform.position, viewport_gunTip.position);

            // Change the FOV of the camera to a hypothetical value

            cam.fieldOfView = oldFOV;

            // Convert the viewport point back to the world with the new FOV
            Vector3 convertedGunTip = cam.ViewportToWorldPoint(new Vector3(viewportPoint.x, viewportPoint.y, distance));

            /*            // Print the results
                        Debug.Log("Old world point: " + currentGrapplePosition);
                        Debug.Log("New world point: " + convertedGrapplePosition);*/

            // Restore the original FOV of the camera



            for (var i = 0; i < quality + 1; i++)
            {

                var delta = i / (float)quality;
                var offset = VIEWPORT_up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);

                VIEWPORT_lr.SetPosition(i, Vector3.Lerp(convertedGunTip, VIEWPORT_currentGrapplePosition, delta) + offset);
            }


            VIEWPORT_grappleHand.transform.position = VIEWPORT_lr.GetPosition(quality);

        }
        else
        {
            EXTERIOR_grappleHand.SetActive(isGrappling);
            if (!isGrappling)
            {
                EXTERIOR_currentGrapplePosition = exterior_gunTip.position;
                spring.Reset();
                if (VIEWPORT_lr.positionCount > 0)
                    VIEWPORT_lr.positionCount = 0;
                if (EXTERIOR_lr.positionCount > 0)
                    EXTERIOR_lr.positionCount = 0;
                return;
            }

            if (EXTERIOR_lr.positionCount == 0)
            {
                spring.SetVelocity(velocity);
                EXTERIOR_lr.positionCount = quality + 1;
            }

            spring.SetDamper(damper);
            spring.SetStrength(strength);
            spring.Update(Time.deltaTime);

        }

        Vector3 EXTERIOR_up = Quaternion.LookRotation((grapplePoint - exterior_gunTip.position).normalized) * Vector3.up;

        EXTERIOR_currentGrapplePosition = Vector3.Lerp(VIEWPORT_currentGrapplePosition, grapplePoint, Time.deltaTime * ropeSpeed);

        for (var i = 0; i < quality + 1; i++)
        {

            var delta = i / (float)quality;
            var offset = EXTERIOR_up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);

            EXTERIOR_lr.SetPosition(i, Vector3.Lerp(exterior_gunTip.position, EXTERIOR_currentGrapplePosition, delta) + offset);
        }
        
        EXTERIOR_grappleHand.transform.position = EXTERIOR_lr.GetPosition(quality);

    }


}
