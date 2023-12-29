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
    [SerializeField] InputActionReference pause;
    [SerializeField] InputActionReference Unpause;

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
    [SerializeField] float BreakNeckSpeed;






    Vector3 inputVector;

    float xRotation = 0;
    float yRotation = 0;

    private Vector3 putTogetherVelocity;







    /// <summary>
    /// grapple
    /// </summary>



    [SerializeField] GrapplingGun grapplingGunScript;

    



    [SerializeField] UnityEngine.LineRenderer lineRenderer;
   












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
        }

    }













    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }


    void Update()
    {
        if (!IsOwner)
        {
            if (grapplingGunScript.IsGrappling())
            {
                if (!lineRenderer.enabled) lineRenderer.enabled = true;
            }
            if (!grapplingGunScript.IsGrappling())
            {
                if (lineRenderer.enabled) lineRenderer.enabled = false;
            }
            return;
        }


        
        ///if isOwner

        inputVector = move.action.ReadValue<Vector3>();
        float mouseX = Input.GetAxis("Mouse X") * mouseSens;

        float mouseY = Input.GetAxis("Mouse Y") * mouseSens;



        xRotation -= mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -90, 90);


        Rotatables.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        PlayerCamera.localRotation = Quaternion.Euler(xRotation, yRotation, 0);

        if (pause.action.triggered) Cursor.lockState = CursorLockMode.None;
        if(Unpause.action.triggered) Cursor.lockState = CursorLockMode.Confined;

        //Debug.Log(rb.velocity.magnitude);

    }

    private void FixedUpdate()
    {
       

        if (!IsOwner) return;

        //Debug.Log(grapplingGunScript.IsGrappling());

        if (inputVector != Vector3.zero)
        {
            if (!grapplingGunScript.IsGrappling())
            {
                if (rb.velocity.magnitude > BreakNeckSpeed)
                {
                    Debug.Log("ahhhhhhhhhhhhhh");
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
                        //Debug.Log("bwaaragagahagahsfghjaklsjsjskjsjsjsj");
                    }

                    rb.AddForce(PlayerCamera.transform.rotation * putTogetherVelocity * tooFastaccel);

                }
                else rb.AddForce((PlayerCamera.transform.rotation * inputVector * speed - rb.velocity) * accel);
            }

            if (grapplingGunScript.IsGrappling())
            {
                if (rb.velocity.magnitude < BreakNeckSpeed) rb.AddForce((PlayerCamera.transform.rotation * inputVector * speed));
            }

        }

    }
}
