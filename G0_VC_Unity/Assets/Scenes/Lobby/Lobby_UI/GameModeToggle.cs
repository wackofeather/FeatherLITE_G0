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
using Unity.Netcode;

public class GameModeToggle : NetworkBehaviour
{
    public ToggleGroup toggleGroup;
    public GameMode_ScreenUI_Controller controller;
    private int currentGameMode;

    // This method is called when the network object is spawned
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initialize();
    }

    // Initialization logic
    private void Initialize()
    {
        currentGameMode = controller.currentScreen;
        Debug.Log(IsHost.ToString());
        if (!IsOwner)
        {
            Debug.Log("ishost");
            toggleGroup.gameObject.SetActive(false);
        }
    }

    // This method is called to control the game mode based on the active toggle
    public void GameModeController()
    {
        Toggle activeToggle = toggleGroup.ActiveToggles().FirstOrDefault();
        if (activeToggle != null)
        {
            if (activeToggle.name == "TD toggle")
            {
                currentGameMode = 1;
            }
            else
            {
                currentGameMode = 0;
            }
            controller.OnGameModeSwitch(currentGameMode);
        }
        else
        {
            Debug.Log("No active toggle");
        }
    }

    // Remove the Start method as it is no longer needed
    // private void Start()
    // {
    //     currentGameMode = controller.currentScreen;
    // }
}
