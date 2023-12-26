using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using static UnityEngine.GraphicsBuffer;
using System.Drawing;

public class GrapplingGun : NetworkBehaviour {

    [Header("Rope Logic")]

    public Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform Playercamera;
    public Rigidbody player;
    private float maxDistance = 100f;
    private SpringJoint joint;
    [SerializeField] InputActionReference Grapple;
    [Range(0f, 100f)]
    [SerializeField] float grappleSpeed;
    [SerializeField] float jointDamper;
    [SerializeField] float jointSpring;
    [SerializeField] float jointMassScale;

    [Header("Rope Graphics")]


    public int quality;
    public float damper;
    public float strength;
    public float velocity;
    public float waveCount;
    public float waveHeight;
    [SerializeField] float ropeSpeed;
    public AnimationCurve affectCurve;
    public AnimationCurve retractCurve;
    [SerializeField] float retractSpeed;
    public bool isGrappling;
    [Range(0,1)]
    [SerializeField] float lookAwayLeniency;    
    private Spring spring;
    [SerializeField] LineRenderer lr;
    private Vector3 currentGrapplePosition;
    private Camera ropeconvertCamera;
    [SerializeField] Transform exterior_gunTip;
    [SerializeField] Transform viewport_gunTip;
    private Transform ropeGunTip;
    private float ropeScaleFactor;
    [SerializeField] GameObject exterior_grappleHand;
    [SerializeField] GameObject viewport_grappleHand;
    private GameObject grappleHand;
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
        
        spring = new Spring();
        spring.SetTarget(0);
        float fov_cache = Playercamera.GetComponent<Camera>().fieldOfView;
        Playercamera.GetComponent<Camera>().fieldOfView = 40f;
        ropeconvertCamera = Playercamera.GetComponent<Camera>();
        Playercamera.GetComponent<Camera>().fieldOfView = fov_cache;

        ropeScaleFactor = 1 / Mathf.Sqrt(fov_cache / 40f);
    }

    private void Start()
    {
        if (!IsOwner)
        {
            return;
        }

        isGrappling = false;

        if (IsOwner)
        {
            Debug.Log("yipeeeeeeeeeeeeeeeeeeee");
            lr.gameObject.layer = LayerMask.NameToLayer("VIEWPORT");
            ropeGunTip = viewport_gunTip;
            grappleHand = viewport_grappleHand;
        }
        else
        {
            lr.gameObject.layer = LayerMask.NameToLayer("EXTERIOR");
            ropeGunTip = exterior_gunTip;
            grappleHand = exterior_grappleHand;
        }
        grappleHand.SetActive(false);

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
            joint.spring = jointSpring;
            joint.damper = jointDamper;
            joint.massScale = jointMassScale;

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
        Destroy(joint);


        spring.Reset();
        spring.SetVelocity(velocity);
        isGrappling = false;
        grappleHand.SetActive(false);
    }

    public bool IsGrappling() {
        return isGrappling;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }






   





    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!isGrappling)
        {
            currentGrapplePosition = ropeGunTip.position;
            spring.Reset();
            if (lr.positionCount > 0)
                lr.positionCount = 0;
            return;
        }

        if (lr.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lr.positionCount = quality + 1;
        }


        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        var up = Quaternion.LookRotation((grapplePoint - ropeGunTip.position).normalized) * Vector3.up;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * ropeSpeed);
        if (IsOwner)
        {
            Camera cam = Playercamera.GetComponent<Camera>();

            float oldFOV = cam.fieldOfView;



            // Project the world point to the viewport
            Vector3 viewportPoint = cam.WorldToViewportPoint(currentGrapplePosition);

            // Calculate the distance from the camera to the world point
            float distance = Vector3.Distance(cam.transform.position, currentGrapplePosition);

            // Change the FOV of the camera to a hypothetical value
            cam.fieldOfView = 40;

            // Convert the viewport point back to the world with the new FOV
            Vector3 convertedGrapplePosition = cam.ViewportToWorldPoint(new Vector3(viewportPoint.x, viewportPoint.y, distance));

/*            // Print the results
            Debug.Log("Old world point: " + currentGrapplePosition);
            Debug.Log("New world point: " + convertedGrapplePosition);*/

            // Restore the original FOV of the camera
            cam.fieldOfView = oldFOV;





















            /*            Vector3 localPos = cam.transform.InverseTransformPoint(currentGrapplePosition);

                        // Convert the FOV from degrees to radians
                        float fov = Mathf.Deg2Rad * 60f;

                        // Calculate the viewport coordinates of the target using the formula
                        float x = 0.5f + 0.5f * localPos.x / localPos.z * 1f / Mathf.Tan(fov / 2f);
                        float y = 0.5f + 0.5f * localPos.y / localPos.z * 1f / Mathf.Tan(fov / 2f);

                        // Create a vector with the viewport coordinates
                        Vector3 viewportPos = new Vector3(x, y, 0f);*//*

            Vector4 worldPos = new Vector4(currentGrapplePosition.x, currentGrapplePosition.y, currentGrapplePosition.z, 1); // get the world position of the point as a Vector4
            Matrix4x4 projMatrix = cam.projectionMatrix; // get the projection matrix of the camera
            Vector4 ndcPos = projMatrix * worldPos; // multiply the world position by the projection matrix
            ndcPos /= ndcPos.w; // divide by the w component to get the NDC




            float horizontalFOV = 90f; // desired horizontal FOV in degrees
            float aspectRatio = 16f / 9f; // camera's aspect ratio
            float verticalFOV = 2f * Mathf.Atan(Mathf.Tan(horizontalFOV * Mathf.Deg2Rad / 2f) / aspectRatio) * Mathf.Rad2Deg; // calculated vertical FOV in degrees
            Matrix4x4 invProjMatrix = Matrix4x4.Perspective(verticalFOV, aspectRatio, cam.nearClipPlane, cam.farClipPlane).inverse; // get the inverse projection matrix of the hypothetical camera
            Vector4 worldPos_1 = invProjMatrix * ndcPos; // multiply the NDC by the inverse projection matrix
            worldPos_1 *= ndcPos.w; // multiply by the w component to get the world position
            Vector3 convertedGrapplePoint = worldPos_1;

            Debug.Log(convertedGrapplePoint);




            *//*  ropeconvertCamera.transform.position = Playercamera.transform.position;

              //Debug.Log(ropeconvertCamera.transform.eulerAngles - Playercamera.transform.eulerAngles);
              ropeconvertCamera.transform.rotation = Playercamera.transform.rotation;
              Vector2 grapplePoint_viewport = Playercamera.gameObject.GetComponent<Camera>().WorldToViewportPoint(currentGrapplePosition);
              float distance = Vector3.Distance(Playercamera.gameObject.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(grapplePoint_viewport.x, grapplePoint_viewport.y, 0)), currentGrapplePosition);
              Vector2 new_viewportVector = new Vector2(grapplePoint_viewport.x + ropeScaleFactor - 1, grapplePoint_viewport.y + ropeScaleFactor - 1);
              Debug.Log(ropeconvertCamera.fieldOfView);
              Vector3 convertedGrapplePoint = ropeconvertCamera.ViewportToWorldPoint(new Vector3(new_viewportVector.x, new_viewportVector.y, distance));
  */
            for (var i = 0; i < quality + 1; i++)
            {

                var delta = i / (float)quality;
                var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);

                lr.SetPosition(i, Vector3.Lerp(ropeGunTip.position, convertedGrapplePosition, delta) + offset);
            }


            grappleHand.transform.position = lr.GetPosition(quality);
            Debug.Log(grappleHand.transform.position - convertedGrapplePosition);
            return;
        }

        

        for (var i = 0; i < quality + 1; i++)
        {

            var delta = i / (float)quality;
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);

            lr.SetPosition(i, Vector3.Lerp(ropeGunTip.position, currentGrapplePosition, delta) + offset);
        }

        grappleHand.transform.position = lr.GetPosition(quality);







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
