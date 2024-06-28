using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class WindEffectOverlay : MonoBehaviour
{
    #region Variables
    public Camera overlayCamera;
    public Camera playerCamera;
    public UniversalAdditionalCameraData cameraData;
    public GameObject windEffect;
    public Rigidbody rb;
    public float speedThreshold;
    #endregion

    public void Start()
    {
        //Find the overlay camera
        overlayCamera = GameObject.Find("Overlay Camera").GetComponent<Camera>();
        //Set the player camera
        playerCamera = this.GetComponentInChildren<Camera>();
        //Set the player Rigidbody
        rb = this.GetComponent<Rigidbody>();
        //Because the stack system is a part of URP, We're going to have to reference the URP section of the camera
        cameraData = playerCamera.GetUniversalAdditionalCameraData();
        //Add the overlay Camera to the stack
        cameraData.cameraStack.Add(overlayCamera);
        //Find the Wind Effect
        windEffect = GameObject.Find("Wind Effect");
    }

    public void EnableOverlayEffect()
    {
        //Enables the wind effect
        windEffect.SetActive(true);
    }

    public void DisableOverlayEffect()
    {
        //Disables the wind effect
        windEffect.SetActive(false);
    }

    public void EffectActivator()
    {
        if (rb.velocity.magnitude > speedThreshold && !windEffect.activeInHierarchy)
        {
            EnableOverlayEffect();
        }
        else if (rb.velocity.magnitude < speedThreshold && windEffect.activeInHierarchy)
        {
            DisableOverlayEffect();
        }
    }

    private void Update()
    {
        EffectActivator();
    }

    /* Alternate Method that may or may not work
    public void EnableOverlayEffect()
    {
        cameraData.cameraStack.Add(overlayCamera);
    }

    public void DisableOverlayEffect()
    {
        cameraData.cameraStack.Remove(overlayCamera);
    }
    */
}
