using Steamworks;
using Steamworks.Data;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class FFA_LobbyGameMode : Base_LobbyGameMode
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void UpdateUIList()
    {
        base.UpdateUIList();
        GameObject Button;
        foreach (Friend member in Lobby_GeneralManager.LobbyManager_Instance.memberList)
        {
            //Button = Instantiate(MenuPrefabButton);
            //Button.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ConstructButton(members);
            //Buttons.Add(Button);
            if (!ObjectPoolingScript.ObjectPoolingScript_Instance.ActiveObjects.Find(x => x.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ButtonId == member.Id))
            {
                Debug.Log("muahaha" + ObjectPoolingScript.ObjectPoolingScript_Instance);
                Button = ObjectPoolingScript.ObjectPoolingScript_Instance.GetPooled_PlayerThumbnailObject(-1);
                ////ColorBlock colorBlock = Button.GetComponentInChildren<UnityEngine.UI.Button>().colors;
                ////colorBlock.normalColor = randomColor;
                ////Button.GetComponentInChildren<UnityEngine.UI.Button>().colors = colorBlock;
                //Button.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ConstructTeamButton(friend);
                
                Button.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ConstructFriendButton(member);
                Button.transform.SetParent(MenuVector.transform);
            }
        }
        VerticalLayoutGroup.CalculateLayoutInputVertical();


    }
    public override void GameMode_Initialize_ForGame()
    {
        base.GameMode_Initialize_ForGame();
        if(Lobby_GeneralManager.LobbyManager_Instance.CurrentGameMode_Int.Value == 0)
        {
            UpdateUIList();
        }
    }
    public override void OnGameModeSwitch(int previousValue, int currentValue)
    {
        base.OnGameModeSwitch(previousValue, currentValue);

        Debug.Log("lake");
        ObjectPoolingScript.ObjectPoolingScript_Instance.Repool();




        UpdateUIList();
        
    }
    public override void GameMode_MemberJoined(Friend friend)
    {
        base.GameMode_MemberJoined(friend);
        UpdateUIList();
    }
    public override void GameMode_MemberLeave(Friend friend)
    {
        base.GameMode_MemberLeave(friend);
        UpdateUIList();
        
    }
    //public void OnEnable()
    //{
    //    UpdateUIList();
    //}

    //public void Start()
    //{
    //    UpdateUIList();
    //}
}
