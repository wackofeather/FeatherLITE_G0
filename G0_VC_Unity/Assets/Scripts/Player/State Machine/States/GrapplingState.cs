using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering.Universal;

public class GrapplingState : BasePlayerState
{

    RaycastHit hit;
    Vector3 local_grapplePoint;
    public GrapplingState(PlayerStateMachine player) : base(player)
    {
        key = 2;
    }
    public override void InitializeState()
    {
        base.InitializeState();

        if (player.networkInfo._isOwner)
        {
            player.EXTERIOR_grappleHand.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
        else
        {
            player.VIEWPORT_grappleHand.SetActive(false);
        }
    }
    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        


        player.EXTERIOR_currentGrapplePosition = player.exterior_gunTip.position;

        player.EXTERIOR_grappleHand.SetActive(true);
        
        player.EXTERIOR_grappleHand.transform.position = player.exterior_gunTip.position;
        player.EXTERIOR_lr.positionCount = player.quality + 1;
        player.spring.SetVelocity(player.velocity);

        if (!player.networkInfo._isOwner)
        {
            player.isGrappling = true;
            base.EnterState();
            return;
        }
        
        

        player.VIEWPORT_currentGrapplePosition = player.viewport_gunTip.position;

        hit = player.GrappleCheck();

        if (player.movingGrapplableLayers == (player.movingGrapplableLayers | (1 << hit.transform.gameObject.layer))) local_grapplePoint = hit.transform.gameObject.transform.InverseTransformPoint(hit.point);
        player.grapplePoint = hit.point;
        Vector3 relativeVelocity = player.PlayerCamera.InverseTransformVector(player.rb.velocity);
        if (relativeVelocity.z < 0) relativeVelocity.z = 0f;
        player.rb.velocity = player.PlayerCamera.rotation * relativeVelocity;
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

        base.EnterState();


    }

    public override void ExitState()
    {
        

        if (!player.networkInfo._isOwner)
        {
            player.EXTERIOR_grappleHand.transform.position = player.exterior_gunTip.position;
            player.EXTERIOR_grappleHand.SetActive(false);
           // player.lineRenderer.enabled = false;
            player.EXTERIOR_currentGrapplePosition = player.exterior_gunTip.position;
            player.spring.Reset();
            player.VIEWPORT_lr.positionCount = 0;
            player.EXTERIOR_lr.positionCount = 0;
            player.isGrappling = false;
            base.ExitState();
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

        player.grappleWiggle_Timer = 0;

        base.ExitState();

    }

    public override void FixedUpdate()
    {

        base.FixedUpdate();

        if (!player.networkInfo._isOwner) return;

        if (Vector3.Distance(player.rb.position, player.grapplePoint) > 0.25f)
        {
            //if (Vector3.Distance(player.position, grapplePoint) < joint.maxDistance - grappleSpeed * Time.deltaTime) joint.maxDistance = Vector3.Distance(player.position, grapplePoint);
            player.joint.maxDistance -= player.grappleSpeed * Time.fixedDeltaTime;
        }

    }



    //if (player.rb.velocity.magnitude < player.BreakNeckSpeed) player.rb.AddForce((player.PlayerCamera.transform.rotation * player.inputVector * player.speed));

    public override void Update()
    {
        base.Update();

        if (!player.networkInfo._isOwner) return;

        if (player.movingGrapplableLayers == (player.movingGrapplableLayers | (1 << hit.transform.gameObject.layer))) 
        {
            player.grapplePoint = hit.transform.gameObject.transform.TransformPoint(local_grapplePoint);
            player.joint.connectedAnchor = player.grapplePoint;
        }
            
            
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

        if (player.networkInfo._isOwner)
        {



            Vector3 VIEWPORT_up = Quaternion.LookRotation((player.grapplePoint - player.viewport_gunTip.position).normalized) * Vector3.up;

            player.VIEWPORT_currentGrapplePosition = Vector3.Lerp(player.VIEWPORT_currentGrapplePosition, player.grapplePoint, Time.deltaTime * player.ropeSpeed);

            ///NOTE: to optimize try using localPos and caching the converted gun tip so no need for calculation every frame. You have to use local pos so it works properly when reused over time tho

            Vector3 convertedGunTip = player.FOVtranslate(player.viewport_gunTip, player.ViewportFOV, player.PlayerCamera.GetComponent<Camera>().fieldOfView);

    /*        Camera cam = player.PlayerCamera.GetComponent<Camera>();

            float oldFOV = cam.fieldOfView;

            cam.fieldOfView = player.ViewportFOV;

            // Project the world point to the viewport
            Vector3 viewportPoint = cam.WorldToViewportPoint(player.viewport_gunTip.position);

            // Calculate the distance from the camera to the world point
            ///////////////////////////////////////////////////////////float distance = Vector3.Distance(cam.transform.position, player.viewport_gunTip.position);
            float distance = Vector3.Project(player.viewport_gunTip.position - cam.transform.position, cam.transform.forward).magnitude;

            // Change the FOV of the camera to a hypothetical value

            cam.fieldOfView = oldFOV;

            // Convert the viewport point back to the world with the new FOV
            Vector3 convertedGunTip = cam.ViewportToWorldPoint(new Vector3(viewportPoint.x, viewportPoint.y, distance));*/

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

        player.EXTERIOR_currentGrapplePosition = Vector3.Lerp(player.EXTERIOR_currentGrapplePosition, player.grapplePoint, Time.deltaTime * player.ropeSpeed);

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
            float Angle = Vector3.Angle(player.PlayerCamera.transform.forward, (player.grapplePoint - player.PlayerCamera.position));

            RaycastHit hit;
            Physics.Raycast(player.PlayerCamera.position, (player.grapplePoint - player.PlayerCamera.position), out hit, player.maxDistance, player.whatIsGrappleable);


            if (Angle > player.PlayerCamera.GetComponent<Camera>().fieldOfView * (1 + player.lookAwayLeniency)) { Debug.Log("Too far look away break"); break; }

            if (((player.grapplePoint - hit.point).magnitude >= 0.7f) && hit.collider != null) { Debug.Log(hit.collider.gameObject.name); break; }


            if (player.Grapple.action.WasReleasedThisFrame()) 
            {
                Debug.Log("released Input break");
                break;
            }

            yield return null;
        }
        //Debug.Log("exitingigngigngingigngign");
        player.ChangeState(player.RegularState);
        yield break;
    }
}




// adfghjk//blahahahaha
