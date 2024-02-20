using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerScript : NetworkBehaviour
{

    public PlayerStateMachine StateMachine;
    public RegularState RegularState;
    public GrapplingState GrapplingState;
    public MeleeState MeleeState;


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






    [DoNotSerialize] public Vector3 inputVector;

    [DoNotSerialize] public float xRotation = 0;
    [DoNotSerialize] public float yRotation = 0;

    [DoNotSerialize] public Vector3 putTogetherVelocity;







    /// <summary>
    /// grapple
    /// </summary>



    public GrapplingGun grapplingGunScript;





    public UnityEngine.LineRenderer lineRenderer;




    public bool CanMove;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            foreach (GameObject t in OwnerOnlyObjects) t.SetActive(false);
            foreach (Transform child in Viewport) child.gameObject.SetActive(false);
            //this.enabled = false;
           // Debug.Log("bruhsushsh");
        }
        else
        {
            foreach (GameObject t in DummyOnlyObjects) t.SetActive(false);
            //foreach (Transform child in Exterior) child.gameObject.SetActive(false);
            //Debug.Log("host joined");
            Exterior.GetComponent<ExteriorShadowSwitch>().ShadowsOnly(true);




        }

    }













    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;


        StateMachine.Initialize(RegularState);
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

        inputVector = move.action.ReadValue<Vector3>();
        float mouseX = Input.GetAxis("Mouse X") * mouseSens;

        float mouseY = Input.GetAxis("Mouse Y") * mouseSens;



        xRotation -= mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -90, 90);


        Rotatables.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        PlayerCamera.localRotation = Quaternion.Euler(xRotation,  yRotation, 0);

        ///if isOwner
        ///

        StateMachine.CurrentPlayerState.Update();

        //Debug.Log(rb.velocity.magnitude);

    }

    private void FixedUpdate()
    {
       

        if (!IsOwner | !CanMove) return;

        StateMachine.CurrentPlayerState.FixedUpdate();

    }


}
