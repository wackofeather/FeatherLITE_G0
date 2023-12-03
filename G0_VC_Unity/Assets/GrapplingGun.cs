using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingGun : MonoBehaviour {
    
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

    void Update() {
        if (Grapple.action.triggered) {
            StartGrapple();
        }

    }



    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple() {
        RaycastHit hit;
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

            grappleHand.SetActive(true);

            StartCoroutine(GrappleCoroutine());
        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple() {
        grappleHand.SetActive(false);
        Destroy(joint);
    }

    public IEnumerator GrappleCoroutine()
    {
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
            yield return null;
        }
        StopGrapple();
    }



    public bool IsGrappling() {
        return joint != null;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }
}
