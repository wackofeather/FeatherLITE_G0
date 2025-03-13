using System.Linq;
using NUnit.Framework;
using Steamworks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Lobby_Scene_Manager : UI_Manager
{

    public GameObject MenuPrefabButton;
    public Transform MenuVector;
    public VerticalLayoutGroup VerticalLayoutGroup;
    public List<GameObject> Buttons;
    public static Lobby_Scene_Manager LobbyUIManager_Instance { get; set; }

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public override void ChildAwake()
    {
        base.ChildAwake();
        
        CounstructLobbyManagerSingleton();
    }
    public void CounstructLobbyManagerSingleton()
    {
        if (LobbyUIManager_Instance != null && LobbyUIManager_Instance != this)
        {
            Destroy(this);
        }
        else
        {
            LobbyUIManager_Instance = this;
        }
    }
}
