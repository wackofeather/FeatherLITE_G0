using System;
using Steamworks.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Buttons_Server_Helper:MonoBehaviour
{
     public Lobby lobby;
     public TMP_Text TextMeshPro;
    //public string serverId;

    public void ConstructButton(Lobby lobby)
    {
        TextMeshPro.text = lobby.Id.ToString();
    }
    public void OnClick()
    {
        SteamLobbyManager.instance.JoinLobbyAsync(Convert.ToUInt64(lobby.Id));
    }

}
