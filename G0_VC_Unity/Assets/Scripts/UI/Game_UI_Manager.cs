using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Unity.Netcode;

public class Game_UI_Manager : UI_Manager
{
    [SerializeField] Canvas ScreenCanvas;
    [SerializeField] Canvas WorldCanvas;

    [SerializeField] Image GrappleIndicator;
    [SerializeField] Color NoGrappleColor;
    [SerializeField] Color GrappleColor;

    public static Game_UI_Manager game_instance { get; set; }


    private void Awake()
    {
        ConstructGameSingleton();
    }




    //GRAPPLE_INDICATOR
    public void UpdateGrappleIndicator(bool canGrapple)
    {
        if (canGrapple) GrappleIndicator.color = GrappleColor;
        else GrappleIndicator.color = NoGrappleColor;
    }

    //HEALTH

    [SerializeField] GameObject TextBarPrefab;
    [SerializeField] TextMeshProUGUI HealthText;
    public void UpdateHealth(float _health)
    {
        HealthText.text = _health.ToString();
    }

    public void AddHealthBarToPlayer(PlayerStateMachine player)
    {
        
        player.extHealthBar = Instantiate(TextBarPrefab, WorldCanvas.transform);
    }
    public void UpdateDummyHealth(PlayerStateMachine player)
    {
        player.extHealthBar.GetComponentInChildren<TextMeshProUGUI>().text = player.health.ToString();
        player.extHealthBar.transform.position = player.ExtHealthBarLocation.position;
        player.GetComponent<PlayerStateMachine>().extHealthBar.transform.LookAt(new Vector3(Game_GeneralManager.instance.runtime_playerObj.GetComponent<PlayerStateMachine>().PlayerCamera.transform.position.x, player.extHealthBar.transform.position.y, Game_GeneralManager.instance.runtime_playerObj.GetComponent<PlayerStateMachine>().PlayerCamera.transform.position.z));
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





    public void ConstructGameSingleton()
    {
        if (game_instance != null && game_instance != this)
        {
            Destroy(this);
        }
        else
        {
            game_instance = this;
        }
    }

    //shid
}
