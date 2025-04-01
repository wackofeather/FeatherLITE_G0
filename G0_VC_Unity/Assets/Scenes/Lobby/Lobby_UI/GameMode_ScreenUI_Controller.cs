using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GameMode_ScreenUI_Controller : Screen_UI_Controller
{

    public void OnGameModeSwitch(int newGameMode)
    {
       SwitchScreens(newGameMode);
       if(Lobby_GeneralManager.LobbyManager_Instance.IsHost)
        {
            Lobby_GeneralManager.LobbyManager_Instance.CurrentGameMode_Int.Value = newGameMode;
        }
    }
    public override void SwitchScreens(int newScreen)
    {
        ScreenDict[currentScreen].DisableScreen();
        ScreenDict[newScreen].EnableScreen();
        currentScreen = newScreen;
        Lobby_GeneralManager.LobbyManager_Instance.CurrentGameMode = ScreenDict[newScreen].gameObject.GetComponentInChildren<Base_LobbyGameMode>();
    }

    private void Start()
    {
        Lobby_GeneralManager.LobbyManager_Instance.CurrentGameMode = ScreenDict[currentScreen].gameObject.GetComponentInChildren<Base_LobbyGameMode>();
    }

    private void Update()
    {
        if(!Lobby_GeneralManager.LobbyManager_Instance.IsHost)
        {
            
            if (Lobby_GeneralManager.LobbyManager_Instance.CurrentGameMode_Int.Value != currentScreen)
            {
                SwitchScreens(Lobby_GeneralManager.LobbyManager_Instance.CurrentGameMode_Int.Value);
            }
        }
    }
}
