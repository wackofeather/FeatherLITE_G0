using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grappling : NetworkBehaviour
{
    [SerializeField] InputActionReference Grapple;
    [SerializeField] Transform GrapplePoint;
    [SerializeField] Transform Camera;
    [SerializeField] UnityEngine.LineRenderer lineRenderer;
    [SerializeField] Transform grappleHand;
    [SerializeField] Rigidbody player;

    [SerializeField] float grappleSpeed;

    [SerializeField] LayerMask grappleLayerMask;

    public Vector3 grappleHit;
    public bool isGrappling;


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

        //Debug.Log(isGrappling);
        if (Grapple.action.triggered)
        {
            if (Physics.Raycast(Camera.position, Camera.forward, out RaycastHit hit, 100f, grappleLayerMask))
            {
                if (!isGrappling)
                {
                    /*Vector3 relVel_vector = player.rotation * player.velocity;
                    player.velocity = new Vector3(relVel_vector.x, 0, relVel_vector.z);*/
                    //Debug.Log(player.rotation * new Vector3(1, 0, 1);
                    //player.velocity = player.rotation * new Vector3(1,0,1);


                    grappleHand.gameObject.SetActive(true);
                    lineRenderer.enabled = true;
                    grappleHit = hit.point;

                    Vector3 relativeVelocity = Camera.transform.InverseTransformVector(player.velocity);

                    //relativeVelocity.x *= 0.5f;

                    //relativeVelocity.y *= 0.5f;

                    if (relativeVelocity.z < 0) relativeVelocity.z = 0f;

                    player.velocity = Camera.rotation * relativeVelocity;

                    isGrappling = true;
                }
                //Vector3 endPos = line.GetPosition(line.positionCount - 1);
            }
        }
        if (isGrappling)
        {
            if (Grapple.action.WasReleasedThisFrame())
            {
                grappleHand.gameObject.SetActive(false);
                lineRenderer.enabled = false;
                isGrappling = false;
            }
        }
    }

    private void LateUpdate()
    {
        ///if IsOwner

        DrawRope();
    }

    private void FixedUpdate()
    {
        MoveToRope();
    }

    void MoveToRope()
    {
        if (!isGrappling) return;

        player.AddForce((grappleHit - player.position) * grappleSpeed);
    }

    void DrawRope()
    {
        if (!isGrappling) return;

        lineRenderer.SetPosition(0, GrapplePoint.position);
        //Debug.Log(lineRenderer.positionCount);
        lineRenderer.SetPosition(1, grappleHit);
        grappleHand.position = grappleHit;
    }

    IEnumerator GrappleCoroutine(RaycastHit hit)
    {
        isGrappling = true;
        lineRenderer.gameObject.SetActive(true);

        while (true)
        {
            if (Grapple.action.WasReleasedThisFrame())
            {
                break;
            }

            lineRenderer.SetPosition(0, GrapplePoint.position);
            lineRenderer.SetPosition(1, hit.point);
            Debug.Log(lineRenderer.GetPosition(0) - GrapplePoint.position);
            yield return 0;
        }

        isGrappling = false;
        lineRenderer.gameObject.SetActive(false);

        yield break;
    }
}
