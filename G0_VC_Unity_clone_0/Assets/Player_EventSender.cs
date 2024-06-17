using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_EventSender : MonoBehaviour
{
    [SerializeField] PlayerStateMachine playerStateMachine;
    //Grapple UI

    public delegate void AbleGrapple();
    public static event AbleGrapple OnAbleGrapple;

    public delegate void UnableGrapple();
    public static event UnableGrapple OnUnableGrapple;

    private void Update()
    {
/*        if (playerStateMachine.CanGrapple()) GrappleIndicator.color = GrappleColor;
        else GrappleIndicator.color = NoGrappleColor;*/
    }
}
