using Unity.VisualScripting;
using UnityEngine;

public class GameMode_ScreenUI_Controller : Screen_UI_Controller
{
    public void OnGameModeSwitch(int newGameMode)
    {
       SwitchScreens(newGameMode);
    }
    public override void SwitchScreens(int newScreen)
    {
        ScreenDict[currentScreen].DisableScreen();
        ScreenDict[newScreen].EnableScreen();
        currentScreen = newScreen;
        LobbyManager.LobbyManager_Instance.CurrentGameMode = ScreenDict[newScreen].gameObject.GetComponentInChildren<Base_LobbyGameMode>();
    }

    private void Start()
    {
        LobbyManager.LobbyManager_Instance.CurrentGameMode = ScreenDict[currentScreen].gameObject.GetComponentInChildren<Base_LobbyGameMode>();
    }
    private void Update()
    {
        if(!LobbyManager.instance.IsHost)
        {
            if (LobbyManager.LobbyManager_Instance.CurrentGameMode_Int.Value != currentScreen)
            {
                SwitchScreens(currentScreen);
            }
        }
    }
}
