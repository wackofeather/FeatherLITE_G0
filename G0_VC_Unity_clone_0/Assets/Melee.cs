using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Melee : NetworkBehaviour
{
    [SerializeField] GrapplingGun grappleScript;
    [SerializeField] PlayerScript playerScript;
    [SerializeField] InputActionReference MeleeAction;
    private float timer;
    private void Update()
    {
        if (!IsOwner) return;

        if (MeleeAction.action.IsPressed())
        {
            timer = 3;
            grappleScript.StopGrapple();
            grappleScript.allowedToGrapple = false;
            playerScript.CanMove = false;
        }

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            if (!grappleScript.allowedToGrapple) grappleScript.allowedToGrapple = true;
            if (!playerScript.CanMove) playerScript.CanMove = true;
        }
    }

}
