using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerScript : NetworkBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference fire;
    [SerializeField] InputActionReference scope;

    [SerializeField] Transform Rotatables;
    [SerializeField] Transform PlayerCamera;
    [SerializeField] Transform CameraHolder;
    [SerializeField] Transform Exterior;
    [SerializeField] Transform Viewport;

    [SerializeField] List<GameObject> OwnerOnlyObjects;
    [SerializeField] List<GameObject> DummyOnlyObjects;

    [SerializeField] float speed;
    [SerializeField] float mouseSens;
    [SerializeField] float accel;
    [SerializeField] float tooFastaccel;
    [SerializeField] float MaxSpeed;

    [SerializeField] float grappleLeniency;




    Vector3 inputVector;

    float xRotation = 0;
    float yRotation = 0;

    private Vector3 putTogetherVelocity;







    /// <summary>
    /// grapple
    /// </summary>




    [SerializeField] InputActionReference Grapple;
    [SerializeField] Transform GrapplePoint;
    [SerializeField] UnityEngine.LineRenderer lineRenderer;
    [SerializeField] Rigidbody grappleHand;

    [SerializeField] float grappleSpeed;

    [SerializeField] LayerMask grappleLayerMask;

    public Vector3 grappleHit;
    public bool isGrappling;
    private SoftJointLimit grappleRopeLength;



    private ConfigurableJoint grappleJoint;

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
            foreach (Transform child in Exterior) child.gameObject.SetActive(false);
        }
        /*if (IsOwner)
        {
            PlayerCamera = GameObject.Find("PlayerCamera").transform;
            PlayerCamera.parent = gameObject.transform;
            PlayerCamera.transform.localPosition = CameraHolder.transform.localPosition;
        }*/
    }












    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            if (isGrappling)
            {
                if (!lineRenderer.enabled) lineRenderer.enabled = true;
            }
            if (!isGrappling)
            {
                if (lineRenderer.enabled) lineRenderer.enabled = false;
            }
            return;
        }


        
        ///if isOwner

        inputVector = move.action.ReadValue<Vector3>();
        float mouseX = Input.GetAxis("Mouse X") * mouseSens;

        float mouseY = Input.GetAxis("Mouse Y") * mouseSens;

        //Debug.Log(PlayerCamera.transform.forward + " , " + inputVector + " , " + Vector3.Scale(PlayerCamera.transform.forward, inputVector));

        xRotation -= mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -90, 90);

        /*        Exterior.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
                Viewport.localRotation = Quaternion.Euler(xRotation, yRotation, 0);*/
        Rotatables.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        PlayerCamera.localRotation = Quaternion.Euler(xRotation, yRotation, 0);









        ///GRAPPLE
        ///




        //Debug.Log(isGrappling);
       /* if (Grapple.action.triggered)
        {
            if (Physics.Raycast(PlayerCamera.position, PlayerCamera.forward, out RaycastHit hit, 100f, grappleLayerMask))
            {
                if (!isGrappling)
                {
                    *//*Vector3 relVel_vector = player.rotation * player.velocity;
                    player.velocity = new Vector3(relVel_vector.x, 0, relVel_vector.z);*//*
                    //Debug.Log(player.rotation * new Vector3(1, 0, 1);
                    //player.velocity = player.rotation * new Vector3(1,0,1);


                    grappleHand.gameObject.SetActive(true);
                    lineRenderer.enabled = true;
                    grappleHit = hit.point;
                    grappleHand.position = grappleHit;

                    Vector3 relativeVelocity = PlayerCamera.InverseTransformVector(rb.velocity);

                    //relativeVelocity.x *= 0.5f;

                    //relativeVelocity.y *= 0.5f;

                    if (relativeVelocity.z < 0) relativeVelocity.z = 0f;

                    rb.velocity = PlayerCamera.rotation * relativeVelocity;

                    grappleJoint = gameObject.AddComponent<ConfigurableJoint>();
                    grappleJoint.configuredInWorldSpace = true;
                    grappleJoint.autoConfigureConnectedAnchor = false;
                    grappleJoint.anchor = Vector3.zero;
                    grappleJoint.connectedAnchor = grappleHit;
                    grappleRopeLength.bounciness = 0;
                    grappleRopeLength.limit = (grappleHit - rb.position).magnitude;
                    grappleJoint.linearLimit = grappleRopeLength;
                    

                    grappleJoint.xMotion = ConfigurableJointMotion.Limited;
                    grappleJoint.yMotion = ConfigurableJointMotion.Limited;
                    grappleJoint.zMotion = ConfigurableJointMotion.Limited;

                    isGrappling = true;
                }
                //Vector3 endPos = line.GetPosition(line.positionCount - 1);
            }
        }
        if (isGrappling)
        {
            if (Grapple.action.WasReleasedThisFrame())
            { 
                Destroy(GetComponent<ConfigurableJoint>());
                grappleHand.gameObject.SetActive(false);
                lineRenderer.enabled = false;
                isGrappling = false;
            }
        }*/

    }

    private void FixedUpdate()
    {
        //MoveToRope();

        if (!IsOwner) return;

        //Debug.Log(PlayerCamera.transform.forward);
        if (inputVector != Vector3.zero)
        {

            //if (!grapplingScript.isGrappling) rb.AddForce((PlayerCamera.transform.rotation * inputVector * speed - rb.velocity) * accel);


            ///Vector3 velocityVector = PlayerCamera.transform.rotation * inputVector * speed * Time.fixedDeltaTime + rb.velocity;
            ///rb.velocity = new Vector3(Mathf.Clamp(velocityVector.x, -MaxSpeed, MaxSpeed), Mathf.Clamp(velocityVector.y, -MaxSpeed, MaxSpeed), Mathf.Clamp(velocityVector.z, -MaxSpeed, MaxSpeed));
            ///Vector3 relativeVelocity = PlayerCamera.transform.InverseTransformVector(rb.velocity);
            ///
            if (!isGrappling)
            {
                if (rb.velocity.magnitude > MaxSpeed)
                {
                    Vector3 inputVelocity = inputVector * speed;
                    Vector3 relativeVelocity = PlayerCamera.transform.InverseTransformVector(rb.velocity);



                    if ((Mathf.Abs(inputVelocity.x) > Mathf.Abs(relativeVelocity.x) + grappleLeniency) | (Mathf.Sign(inputVelocity.x) != Mathf.Sign(relativeVelocity.x))) putTogetherVelocity.x = (inputVelocity.x - relativeVelocity.x);
                    else putTogetherVelocity.x = 0;
                    if ((Mathf.Abs(inputVelocity.y) > Mathf.Abs(relativeVelocity.y) + grappleLeniency) | (Mathf.Sign(inputVelocity.y) != Mathf.Sign(relativeVelocity.y))) putTogetherVelocity.y = (inputVelocity.y - relativeVelocity.y);
                    else putTogetherVelocity.y = 0;
                    if ((Mathf.Abs(inputVelocity.z) > Mathf.Abs(relativeVelocity.z) + grappleLeniency) | (Mathf.Sign(inputVelocity.z) != Mathf.Sign(relativeVelocity.z))) putTogetherVelocity.z = (inputVelocity.z - relativeVelocity.z);
                    else
                    {
                        putTogetherVelocity.z = 0;
                        Debug.Log("bwaaragagahagahsfghjaklsjsjskjsjsjsj");
                    }

                    rb.AddForce(PlayerCamera.transform.rotation * putTogetherVelocity * tooFastaccel);

                }
                else rb.AddForce((PlayerCamera.transform.rotation * inputVector * speed - rb.velocity) * accel);
            }

            if (isGrappling)
            {
                if (rb.velocity.magnitude < MaxSpeed) rb.AddForce((PlayerCamera.transform.rotation * inputVector * speed));
            }

           



/*            if (rb.velocity.magnitude <= MaxSpeed && !grapplingScript.isGrappling) rb.AddForce((PlayerCamera.transform.rotation * inputVector * speed - rb.velocity) * accel);
            else
            {
                Vector3 inputVelocity = inputVector * speed;
                Vector3 relativeVelocity = PlayerCamera.transform.InverseTransformVector(rb.velocity);

                

                if ((Mathf.Abs(inputVelocity.x) > Mathf.Abs(relativeVelocity.x)) | (Mathf.Sign(inputVelocity.x) != Mathf.Sign(relativeVelocity.x))) putTogetherVelocity.x = (inputVelocity.x - relativeVelocity.x);
                else putTogetherVelocity.x = 0;
                if ((Mathf.Abs(inputVelocity.y) > Mathf.Abs(relativeVelocity.y)) | (Mathf.Sign(inputVelocity.y) != Mathf.Sign(relativeVelocity.y))) putTogetherVelocity.y = (inputVelocity.y - relativeVelocity.y);
                else putTogetherVelocity.y = 0;
                if ((Mathf.Abs(inputVelocity.z) > Mathf.Abs(relativeVelocity.z)) | (Mathf.Sign(inputVelocity.z) != Mathf.Sign(relativeVelocity.z))) putTogetherVelocity.z = (inputVelocity.z - relativeVelocity.z);
                else 
                {
                    putTogetherVelocity.z = 0;
                    Debug.Log("bwaaragagahagahsfghjaklsjsjskjsjsjsj");
                }





*//*                if (inputVelocity.y > relativeVelocity.y) putTogetherVelocity.y = (inputVelocity.y - relativeVelocity.y);
                else putTogetherVelocity.y = 0;
                if (inputVelocity.z > relativeVelocity.z) putTogetherVelocity.z = (inputVelocity.z - relativeVelocity.z);
                else putTogetherVelocity.z = 0;*//*

                rb.AddForce(PlayerCamera.transform.rotation * putTogetherVelocity * tooFastaccel);

            }*/



            ///if (rb.velocity.magnitude <= MaxSpeed && !grapplingScript.isGrappling) rb.AddForce((PlayerCamera.transform.rotation * inputVector * speed - rb.velocity) * accel);
            ///else rb.AddForce((PlayerCamera.transform.rotation * inputVector * speed - rb.velocity) * tooFastaccel);


            //rb.velocity = Vector3.ClampMagnitude((PlayerCamera.transform.rotation * inputVector * speed + rb.velocity), MaxSpeed);

        }
        //////////////////////////////rb.velocity = Vector3.ClampMagnitude((PlayerCamera.transform.rotation * inputVector * speed + rb.velocity), MaxSpeed);
    }


    private void LateUpdate()
    {
        ///if IsOwner

        //DrawRope();
    }









    void MoveToRope()
    {
        if (!isGrappling) return;

        Debug.Log(rb.velocity);

        //grappleJoint.connectedAnchor = gameObject.transform.position;

        /*if (grappleRopeLength.limit > 3) grappleRopeLength.limit -= 1f;
        grappleJoint.linearLimit = grappleRopeLength;*/

        if (grappleRopeLength.limit > 5)
        {
            rb.AddForce((grappleHit - rb.position).normalized * grappleSpeed);
            grappleRopeLength.limit = (grappleHit - rb.position).magnitude;
            grappleJoint.linearLimit = grappleRopeLength;
        }


    }

    void DrawRope()
    {
        if (!isGrappling) return;

        lineRenderer.SetPosition(0, GrapplePoint.position);
        //Debug.Log(lineRenderer.positionCount);
        lineRenderer.SetPosition(1, grappleHit);
        grappleHand.position = grappleHit;
    }
}
