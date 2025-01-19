using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Game_UI_Manager : UI_Manager
{

    new public static Game_UI_Manager instance { get; set; }







    //GRAPPLE_INDICATOR
    [SerializeField] Image GrappleIndicator;
    [SerializeField] Color NoGrappleColor;
    [SerializeField] Color GrappleColor;

    public void UpdateGrappleIndicator(bool canGrapple)
    {
        if (canGrapple) GrappleIndicator.color = GrappleColor;
        else GrappleIndicator.color = NoGrappleColor;
    }

    //WEAPONPICKUP
    [SerializeField] Image WeaponPickUpImageObject;
    public void UpdateWeaponPickUI(Sprite sprite)
    {
        if (sprite == null) WeaponPickUpImageObject.color = new Color(0, 0, 0, 0);
        else
        {
            WeaponPickUpImageObject.sprite = sprite;
            WeaponPickUpImageObject.color = new Color(1, 1, 1, 1);
        }
    }




    //PLACEHOLDER LOADINGBAR
    [SerializeField] TextMeshProUGUI WeaponPickUpCountdown;
    public void SetCountdownText(float countDownNumber)
    {
        WeaponPickUpCountdown.SetAllDirty();
        if (countDownNumber == 0)
        {
            float Test = -1;
            WeaponPickUpCountdown.text = Test.ToString();
            Debug.Log("CountdownStopped");
        } 
        else WeaponPickUpCountdown.text = countDownNumber.ToString();
        //WeaponPickUpCountdown.ForceMeshUpdate(true);
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
