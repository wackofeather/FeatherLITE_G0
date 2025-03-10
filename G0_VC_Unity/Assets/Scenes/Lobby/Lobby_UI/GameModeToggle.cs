using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Steamworks;
using Steamworks.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameModeToggle : MonoBehaviour
{
    public ToggleGroup toggleGroup;
    public GameMode_ScreenUI_Controller controller;
    private int currentGameMode;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void GameModeController()
    {
        Toggle activeToggle = toggleGroup.ActiveToggles().FirstOrDefault();
        if (activeToggle != null)
        {
            if(activeToggle.name == "TD toggle")
            {
                currentGameMode = 1;
            }
            else
            {
                currentGameMode = 0;
            }
            controller.OnGameModeSwitch(currentGameMode);
            //if (activeToggle.name == "TD toggle")
            //{
            //    //slider.gameObject.SetActive(true);
            //    //TeamSetting(); // Call TeamSetting once when the toggle is active
            //    //Lobby_Scene_Manager.LobbyUIManager_Instance.UpdateTeamListUI(ListClass);
            //}
            //else
            //{
            //    slider.gameObject.SetActive(false);
            //}
        }
        else
        {
            Debug.Log("No active toggle");
        }
    }
    void Start()
    {
        currentGameMode = controller.currentScreen;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
