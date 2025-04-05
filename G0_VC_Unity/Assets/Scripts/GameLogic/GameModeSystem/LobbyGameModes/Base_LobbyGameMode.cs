using System.Collections.Generic;
using Steamworks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public abstract class Base_LobbyGameMode:NetworkBehaviour 
{
    public GameObject MenuPrefabButton;
    public Transform MenuVector;
    public VerticalLayoutGroup VerticalLayoutGroup;
    [HideInInspector] public List<GameObject> Buttons;
    public int GameModeKey;

    private void Start()
    {
        Debug.Log("jkajajaka");
        Lobby_GeneralManager.LobbyManager_Instance.CurrentGameMode_Int.OnValueChanged += OnGameModeSwitch;

        GameMode_Initialize_ForGame();
    }
    public virtual void GameMode_MemberJoined(Friend friend)
    {

    }
    public virtual void GameMode_MemberLeave(Friend friend)
    {

    }
    public virtual void GameMode_Initialize_ForGame()
    {

    }

    public virtual void OnGameModeSwitch(int previousValue ,int currentValue)
    {
        Debug.Log("chrenshaw" + Lobby_GeneralManager.LobbyManager_Instance.CurrentGameMode_Int);
    }
    public void OnDisable()
    {
        
    }

  
    public virtual void ClearButtons()
    {
        foreach (GameObject button in Buttons)
        {
            Destroy(button);
        }
        Buttons.Clear();
    }

    public virtual void UpdateUIList()
    {
       
    }


}
