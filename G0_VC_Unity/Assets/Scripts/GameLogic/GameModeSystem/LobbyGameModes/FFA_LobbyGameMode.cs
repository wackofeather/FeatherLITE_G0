using Steamworks;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;

public class FFA_LobbyGameMode : Base_LobbyGameMode
{
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void UpdateUIList()
    {
       base.UpdateUIList();
        ClearButtons();
        GameObject Button;
        foreach (Friend members in LobbyManager.LobbyManager_Instance.memberList)
        {
            Button = Instantiate(MenuPrefabButton, MenuVector.transform);
            Button.GetComponent<Lobby_Player_Buttons_Helpers>().ConstructButton(members);
            Buttons.Add(Button);
        }
        VerticalLayoutGroup.CalculateLayoutInputVertical();
    
    }
    public void OnEnable()
    {
        UpdateUIList();
    }
}
