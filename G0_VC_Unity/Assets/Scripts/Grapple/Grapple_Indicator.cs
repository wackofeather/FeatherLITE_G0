using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grapple_Indicator : MonoBehaviour
{
    [SerializeField] GrapplingGun grappleScript;
    [SerializeField] Image GrappleIndicator;
    [SerializeField] Color NoGrappleColor;
    [SerializeField] Color GrappleColor;


    void Update()
    {
        if (grappleScript.CanGrapple()) GrappleIndicator.color = GrappleColor;
        else GrappleIndicator.color = NoGrappleColor;
    }
}
