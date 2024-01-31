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
    public new RegularState RegularState;
    public new GrapplingState GrapplingState;
    public new MeleeState MeleeState;

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


            StateMachine = new PlayerStateMachine();
            RegularState = new RegularState(this as PlayerBase, StateMachine);
            GrapplingState = new GrapplingState(this as PlayerBase, StateMachine);
            MeleeState = new MeleeState(this as PlayerBase, StateMachine);

        }

    }













    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        spring = new Spring();
        spring.SetTarget(0);

        if (!IsOwner)
        {
            VIEWPORT_lr.gameObject.SetActive(false);
            return;
        }

        VIEWPORT_lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        EXTERIOR_lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;


        isGrappling = false;

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

    }


}
