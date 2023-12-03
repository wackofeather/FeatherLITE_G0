using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingGun : MonoBehaviour {
    
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, Playercamera;
    public Rigidbody player;
    [SerializeField] float maxDistance = 100f;
    private SpringJoint joint;
    [SerializeField] InputActionReference Grapple;
    [Range(0f, 100f)]
    [SerializeField] float grappleSpeed;
    [SerializeField] float grappleTime;
    [SerializeField] AnimationCurve grappleSpeedCurve;
    [SerializeField] GameObject grappleHand;
    [HideInInspector] public bool isGrappling;
    [SerializeField] LineRenderer grappleLine;
    [SerializeField] Transform GrappleStart;
    void Update() {
        if (Grapple.action.triggered) {
            StartGrapple();
        }
        if (grappleLine.enabled)
        {
            grappleLine.SetPosition(0, GrappleStart.position);
            grappleLine.SetPosition(1, grapplePoint);
        }

    }



    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple() {

        if (!isGrappling) StartCoroutine(GrappleCoroutine());
        
        
       /* RaycastHit hit;
        if (Physics.Raycast(Playercamera.position, Playercamera.forward, out hit, maxDistance, whatIsGrappleable)) {
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

            StartCoroutine(GrappleCoroutine());
        }*/
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>

    public IEnumerator GrappleCoroutine()
    {
        isGrappling = true;
        grappleHand.gameObject.SetActive(true);
        grappleLine.enabled = true;
        Quaternion grappleDirection = Playercamera.transform.rotation;
        float grappleDistance = 0;
        float timer = 0;
        RaycastHit hit;
        while (true)
        {
            if (timer / grappleTime > 1 | Grapple.action.WasReleasedThisFrame() | Physics.Raycast(Playercamera.position, grappleDirection * Vector3.forward, grappleDistance * 0.4f, whatIsGrappleable))
            {
                StopGrapple();
                yield break;
            }
            else if (Physics.Raycast(Playercamera.position, grappleDirection * Vector3.forward, out hit, grappleDistance, whatIsGrappleable))
            {
                grapplePoint = hit.point;
                grappleHand.transform.position = hit.point;
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
                break;
            }

            timer += Time.deltaTime;
            grappleDistance = maxDistance * grappleSpeedCurve.Evaluate(timer / grappleTime);
            grappleDirection = Quaternion.Lerp(grappleDirection, Playercamera.transform.rotation, 0.25f);
            grappleHand.transform.position = player.transform.position + grappleDirection * Vector3.forward * grappleDistance;
            grapplePoint = grappleHand.transform.position;
            grappleLine.SetPosition(0, GrappleStart.position);
            grappleLine.SetPosition(1, grapplePoint);
            yield return null;
        }

        while (true)
        {
            if (Grapple.action.WasReleasedThisFrame())
            {
                break;
            }
            if (Vector3.Distance(player.position, grapplePoint) > 0.25f)
            {
                /*joint.maxDistance -= grappleSpeed;
                joint.minDistance -= grappleSpeed;*/
                
                joint.maxDistance -= grappleSpeed * Time.deltaTime;
            }
/*            else
            {
                break;
            }*/
            grappleHand.transform.position = grapplePoint;
            grappleLine.SetPosition(0, GrappleStart.position);
            grappleLine.SetPosition(1, grapplePoint);
            yield return null;
        }
        StopGrapple();
    }

    void StopGrapple()
    {
        grappleHand.gameObject.SetActive(false);
        Destroy(joint);
        grappleLine.enabled = false;
        isGrappling = false;
    }










/*    public bool IsGrappling() {
        return joint != null;
    }*/

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }
}
