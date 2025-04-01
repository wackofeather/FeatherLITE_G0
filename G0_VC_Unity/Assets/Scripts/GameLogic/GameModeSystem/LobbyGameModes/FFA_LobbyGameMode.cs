using Steamworks;
using Steamworks.Data;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class FFA_LobbyGameMode : Base_LobbyGameMode
{
    public List<GameObject> ActiveObjects;
    public void RemovedAll()
    {
        foreach (GameObject obj in ActiveObjects)
        {
            ObjectPoolingScript.ObjectPoolingScript_Instance.pooledObjects.Add(obj);
            obj.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ButtonId = 0;
            obj.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().teamClass = null;
            obj.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().tmpText2.text = "";
            obj.transform.SetParent(ObjectPoolingScript.ObjectPoolingScript_Instance.MenuVector);
        }
        ActiveObjects.Clear();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void UpdateUIList()
    {
       base.UpdateUIList();
        GameObject Button;
        foreach (Friend members in Lobby_GeneralManager.LobbyManager_Instance.memberList)
        {
            //Button = Instantiate(MenuPrefabButton);
            //Button.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ConstructButton(members);
            //Buttons.Add(Button);
            if (ActiveObjects.Find(x => x.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ButtonId == members.Id))
            {
                Debug.Log("muahaha" + ActiveObjects.Count);
            }
            else
            {
                Debug.Log("muahaha" + ObjectPoolingScript.ObjectPoolingScript_Instance);
                Button = ObjectPoolingScript.ObjectPoolingScript_Instance.GetPooledObject();
                ActiveObjects.Add(Button);
                ////ColorBlock colorBlock = Button.GetComponentInChildren<UnityEngine.UI.Button>().colors;
                ////colorBlock.normalColor = randomColor;
                ////Button.GetComponentInChildren<UnityEngine.UI.Button>().colors = colorBlock;
                //Button.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ConstructTeamButton(friend);
                Button.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ButtonId = members.Id;
                Button.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ConstructTeamButton(members.Id);
                Button.transform.SetParent(MenuVector.transform, true);
            }
        }
        VerticalLayoutGroup.CalculateLayoutInputVertical();


    }
    private void Start()
    {
        if(Lobby_GeneralManager.LobbyManager_Instance.CurrentGameMode_Int.Value == 0)
        {
            UpdateUIList();
        }
    }
    public override void OnGameModeSwitch(int previousValue, int currentValue)
    {
        base.OnGameModeSwitch(previousValue, currentValue);
        if(currentValue ==0)
        {
            UpdateUIList();
        }
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
