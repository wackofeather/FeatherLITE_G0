using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_UI_Manager : UI_Manager
{
    [SerializeField] Image GrappleIndicator;
    [SerializeField] Color NoGrappleColor;
    [SerializeField] Color GrappleColor;

    new public static Game_UI_Manager instance { get; set; }

    public void UpdateGrappleIndicator(bool canGrapple)
    {
        if (canGrapple) GrappleIndicator.color = GrappleColor;
        else GrappleIndicator.color = NoGrappleColor;
    }

    public override void ConstructSingleton()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
}
