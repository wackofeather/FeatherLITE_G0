using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerScript : PlayerBase, IPlayerInterface
{

    public PlayerStateMachine StateMachine;
    public RegularState RegularState;
    public GrapplingState GrapplingState;
    public MeleeState MeleeState;

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

            RegularState = new RegularState(this, StateMachine);
            GrapplingState = new GrapplingState(this, StateMachine);
            MeleeState = new MeleeState(this, StateMachine);

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



        ///if isOwner
        ///

        StateMachine.CurrentPlayerState.Update();

        //Debug.Log(rb.velocity.magnitude);

    }

    private void FixedUpdate()
    {
       

        if (!IsOwner | !CanMove) return;

        StateMachine.CurrentPlayerState.FixedUpdate();

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
