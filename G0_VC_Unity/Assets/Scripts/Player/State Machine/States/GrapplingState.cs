using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Profiling;


public class GrapplingState : BasePlayerState
{
    public GrapplingState(PlayerStateMachine player) : base(player)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();

        player.EXTERIOR_currentGrapplePosition = player.exterior_gunTip.position;

        player.EXTERIOR_grappleHand.SetActive(true);
        
        player.EXTERIOR_grappleHand.transform.position = player.exterior_gunTip.position;
        player.EXTERIOR_lr.positionCount = player.quality + 1;
        player.spring.SetVelocity(player.velocity);

        if (!player.IsOwner)
        {
            return;
        }

        player.VIEWPORT_currentGrapplePosition = player.viewport_gunTip.position;

        RaycastHit hit = player.GrappleCheck();
        player.grapplePoint = hit.point;
        Vector3 relativeVelocity = player.Playercamera.InverseTransformVector(player.rb.velocity);
        if (relativeVelocity.z < 0) relativeVelocity.z = 0f;
        player.rb.velocity = player.Playercamera.rotation * relativeVelocity;
        player.joint = player.gameObject.AddComponent<SpringJoint>();
        player.joint.autoConfigureConnectedAnchor = false;
        player.joint.connectedAnchor = player.grapplePoint;

        float distanceFromPoint = Vector3.Distance(player.rb.position, player.grapplePoint);

        //The distance grapple will try to keep from grapple point. 
        player.joint.maxDistance = distanceFromPoint * 1f;
        player.joint.minDistance = 0.25f; //distanceFromPoint * 0.25f;

        //Adjust these values to fit your game.
        player.joint.spring = player.jointSpring;
        player.joint.damper = player.jointDamper;
        player.joint.massScale = player.jointMassScale;

        player.isGrappling = true;

        player.VIEWPORT_grappleHand.SetActive(true);
        player.VIEWPORT_grappleHand.transform.position = player.viewport_gunTip.position;
        //player.lineRenderer.enabled = true;
        player.VIEWPORT_lr.positionCount = player.quality + 1;

        player._StartCoroutine(GrappleCoroutine());

        
    }

    public override void ExitState()
    {
        base.ExitState();
        if (!player.IsOwner)
        {
            player.EXTERIOR_grappleHand.transform.position = player.exterior_gunTip.position;
            player.EXTERIOR_grappleHand.SetActive(false);
           // player.lineRenderer.enabled = false;
            player.EXTERIOR_currentGrapplePosition = player.exterior_gunTip.position;
            player.spring.Reset();
            player.VIEWPORT_lr.positionCount = 0;
            player.EXTERIOR_lr.positionCount = 0;
            return;
        }
        player._StopCoroutine(GrappleCoroutine());

        player._Destroy(player.joint);

        //player.lineRenderer.enabled = false;
        player.EXTERIOR_currentGrapplePosition = player.exterior_gunTip.position;
        player.VIEWPORT_currentGrapplePosition = player.viewport_gunTip.position;
        player.spring.Reset();
        player.VIEWPORT_lr.positionCount = 0;
        player.EXTERIOR_lr.positionCount = 0;


        player.EXTERIOR_grappleHand.transform.position = player.exterior_gunTip.position;
        player.VIEWPORT_grappleHand.transform.position = player.viewport_gunTip.position;
        player.EXTERIOR_grappleHand.SetActive(false);
        player.VIEWPORT_grappleHand.SetActive(false);

        player.isGrappling = false;
        
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

    }



    //if (player.rb.velocity.magnitude < player.BreakNeckSpeed) player.rb.AddForce((player.PlayerCamera.transform.rotation * player.inputVector * player.speed));

    public override void Update()
    {
        base.Update();

        
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        DrawRope();
    }

    public void DrawRope()
    {
        player.spring.SetDamper(player.damper);
        player.spring.SetStrength(player.strength);
        player.spring.Update(Time.deltaTime);

        if (player.IsOwner)
        {



            Vector3 VIEWPORT_up = Quaternion.LookRotation((player.grapplePoint - player.viewport_gunTip.position).normalized) * Vector3.up;

            player.VIEWPORT_currentGrapplePosition = Vector3.Lerp(player.VIEWPORT_currentGrapplePosition, player.grapplePoint, Time.deltaTime * player.ropeSpeed);

            ///NOTE: to optimize try using localPos and caching the converted gun tip so no need for calculation every frame. You have to use local pos so it works properly when reused over time tho


            Camera cam = player.Playercamera.GetComponent<Camera>();

            float oldFOV = cam.fieldOfView;

            cam.fieldOfView = player.ViewportFOV;

            // Project the world point to the viewport
            Vector3 viewportPoint = cam.WorldToViewportPoint(player.viewport_gunTip.position);

            // Calculate the distance from the camera to the world point
            float distance = Vector3.Distance(cam.transform.position, player.viewport_gunTip.position);

            // Change the FOV of the camera to a hypothetical value

            cam.fieldOfView = oldFOV;

            // Convert the viewport point back to the world with the new FOV
            Vector3 convertedGunTip = cam.ViewportToWorldPoint(new Vector3(viewportPoint.x, viewportPoint.y, distance));

            /*            // Print the results
                        Debug.Log("Old world point: " + currentGrapplePosition);
                        Debug.Log("New world point: " + convertedGrapplePosition);*/

            // Restore the original FOV of the camera



            for (var i = 0; i < player.quality + 1; i++)
            {

                var delta = i / (float)player.quality;
                var offset = VIEWPORT_up * player.waveHeight * Mathf.Sin(delta * player.waveCount * Mathf.PI) * player.spring.Value * player.affectCurve.Evaluate(delta);

                player.VIEWPORT_lr.SetPosition(i, Vector3.Lerp(convertedGunTip, player.VIEWPORT_currentGrapplePosition, delta) + offset);
            }


            player.VIEWPORT_grappleHand.transform.position = player.VIEWPORT_lr.GetPosition(player.quality);

        }

        Vector3 EXTERIOR_up = Quaternion.LookRotation((player.grapplePoint - player.exterior_gunTip.position).normalized) * Vector3.up;

        player.EXTERIOR_currentGrapplePosition = Vector3.Lerp(player.VIEWPORT_currentGrapplePosition, player.grapplePoint, Time.deltaTime * player.ropeSpeed);

        for (var i = 0; i < player.quality + 1; i++)
        {

            var delta = i / (float)player.quality;
            var offset = EXTERIOR_up * player.waveHeight * Mathf.Sin(delta * player.waveCount * Mathf.PI) * player.spring.Value * player.affectCurve.Evaluate(delta);

            player.EXTERIOR_lr.SetPosition(i, Vector3.Lerp(player.exterior_gunTip.position, player.EXTERIOR_currentGrapplePosition, delta) + offset);
        }

        player.EXTERIOR_grappleHand.transform.position = player.EXTERIOR_lr.GetPosition(player.quality);
    }



    public IEnumerator GrappleCoroutine()
    {
        while (true)
        {

            float ClampedDistance = (player.rb.position - player.grapplePoint).magnitude / player.maxDistance;
            float Angle = Vector3.Angle(player.Playercamera.transform.forward, (player.grapplePoint - player.Playercamera.position));

            RaycastHit hit;
            Physics.Raycast(player.Playercamera.position, (player.grapplePoint - player.Playercamera.position), out hit, player.maxDistance, player.whatIsGrappleable);


            if (Angle > player.Playercamera.GetComponent<Camera>().fieldOfView * (1 + player.lookAwayLeniency)) break;

            if ((player.grapplePoint - hit.point).magnitude >= 0.1f) break;


            if (player.Grapple.action.WasReleasedThisFrame()) break;


            if (Vector3.Distance(player.rb.position, player.grapplePoint) > 0.25f)
            {
                //if (Vector3.Distance(player.position, grapplePoint) < joint.maxDistance - grappleSpeed * Time.deltaTime) joint.maxDistance = Vector3.Distance(player.position, grapplePoint);
                player.joint.maxDistance -= player.grappleSpeed * Time.deltaTime;
            }

            yield return null;
        }
        Debug.Log("exitingigngigngingigngign");
        player.ChangeState(player.RegularState);
        yield break;
    }
}

