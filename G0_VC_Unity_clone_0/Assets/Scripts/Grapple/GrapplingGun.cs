using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingGun : MonoBehaviour {

    [Header("Rope Logic")]

    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, Playercamera;
    public Rigidbody player;
    private float maxDistance = 100f;
    private SpringJoint joint;
    [SerializeField] InputActionReference Grapple;
    [Range(0f, 100f)]
    [SerializeField] float grappleSpeed;
    [SerializeField] GameObject grappleHand;

    [Header("Rope Graphics")]

    private Spring spring;
    private LineRenderer lr;
    private Vector3 currentGrapplePosition;
    public int quality;
    public float damper;
    public float strength;
    public float velocity;
    public float waveCount;
    public float waveHeight;
    public AnimationCurve affectCurve;
    public AnimationCurve retractCurve;
    [SerializeField] float retractSpeed;
    [SerializeField] Transform grappleTangentWeight;
    private Vector3 cachedGrappleTangent_local_pos;
    private bool isGrappling;
    [Range(0,1)]
    [SerializeField] float lookAwayLeniency;
    public AnimationCurve tangentpullbackCurve_angle;
    public AnimationCurve tangentpullbackCurve_distance;
    private Vector3 tangent_cache;
    public GameObject test;



    //method overloading with multiple versions of bool
    private bool CanGrapple(out RaycastHit hit)
    {
        if (isGrappling)
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
    public bool CanGrapple()
    {
        if (isGrappling)
        {
            return false;
        }
        else return Physics.Raycast(Playercamera.position, Playercamera.forward, maxDistance, whatIsGrappleable);
    }


    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        spring = new Spring();
        spring.SetTarget(0);
    }

    private void Start()
    {
        isGrappling = false;
        cachedGrappleTangent_local_pos = grappleTangentWeight.localPosition;
    }

    void Update() {
        //if (isGrappling && Physics.Linecast(gunTip.position, Vector3.Lerp(gunTip.position, Playercamera.position, 0.9f), grappleBreakers)) Debug.Log("PLUH");
        if (Grapple.action.triggered && !isGrappling) {
            StartGrapple();
        }

        test.transform.position = tangent_cache;

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
            Vector3 relativeVelocity = Playercamera.InverseTransformVector(player.velocity);
            if (relativeVelocity.z < 0) relativeVelocity.z = 0f;
            player.velocity = Playercamera.rotation * relativeVelocity;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 1f;
            joint.minDistance = 0.25f; //distanceFromPoint * 0.25f;

            //Adjust these values to fit your game.
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            isGrappling = true;

            grappleHand.SetActive(true);

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
            
            float ClampedDistance = (player.position - grapplePoint).magnitude / maxDistance;
            float Angle = Vector3.Angle(Playercamera.transform.forward, (grapplePoint - Playercamera.position));

            RaycastHit hit;
            Physics.Raycast(Playercamera.position, (grapplePoint - Playercamera.position), out hit, maxDistance, whatIsGrappleable);
            //Debug.Log(grapplePoint - hit.point);

            if (Angle > Playercamera.GetComponent<Camera>().fieldOfView * (1 + lookAwayLeniency)) break;

            if ((grapplePoint - hit.point).magnitude >= 0.1f) break;
            
            //if (hit.point != grapplePoint) Debug.Log("PLUHHHSHS");

            if (Grapple.action.WasReleasedThisFrame()) break;
            

            if (Vector3.Distance(player.position, grapplePoint) > 0.25f)
            {
                /*joint.maxDistance -= grappleSpeed;
                joint.minDistance -= grappleSpeed;*/
                
                joint.maxDistance -= grappleSpeed * Time.deltaTime;
            }

            if (lr.positionCount == 0)
            {
                spring.SetVelocity(velocity);
                lr.positionCount = quality + 1;
            }

            spring.SetDamper(damper);
            spring.SetStrength(strength);
            spring.Update(Time.deltaTime);

            var up = Quaternion.LookRotation((grapplePoint - gunTip.position).normalized) * Vector3.up;

            currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);

            for (var i = 0; i < quality + 1; i++)
            {

                var delta = i / (float)quality;
                var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);

                lr.SetPosition(i, Vector3.Lerp(gunTip.position, currentGrapplePosition, delta) + offset);
            }

            grappleHand.transform.position = lr.GetPosition(quality);

            /*else
            {
                break;
            }*/
            yield return null;
        }
        StopGrapple();
        yield break;
    }

    public void StopGrapple()
    {
        Destroy(joint);


        spring.Reset();
        spring.SetVelocity(velocity);
        isGrappling = false;
        grappleHand.SetActive(false);
    }

    public bool IsGrappling() {
        return joint != null;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }






   





    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!isGrappling)
        {
            currentGrapplePosition = gunTip.position;
            spring.Reset();
            if (lr.positionCount > 0)
                lr.positionCount = 0;
            return;
        }



       /* if (lr.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lr.positionCount = quality + 1;
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        var grapplePoint = GetGrapplePoint();
        var gunTipPosition = gunTip.position;
        var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);

        for (var i = 0; i < quality + 1; i++)
        {
            var delta = i / (float)quality;
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
                         affectCurve.Evaluate(delta);

            lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
        }

        grappleHand.transform.position = currentGrapplePosition;*/
    }


}