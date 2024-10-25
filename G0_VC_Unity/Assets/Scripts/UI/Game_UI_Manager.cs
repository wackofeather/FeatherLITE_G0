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

    new public static Game_UI_Manager instance { get; set; }


    




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
        player.GetComponent<PlayerStateMachine>().extHealthBar.transform.LookAt(new Vector3(NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerStateMachine>().PlayerCamera.transform.position.x, player.GetComponent<PlayerStateMachine>().extHealthBar.transform.position.y, NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerStateMachine>().PlayerCamera.transform.position.z));
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

    //s
}
