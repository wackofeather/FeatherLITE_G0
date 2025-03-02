using System.Linq;
using NUnit.Framework;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class Lobby_Scene_Manager : UI_Manager
{

    public GameObject MenuPrefabButton;
    public Transform MenuVector;
    public VerticalLayoutGroup VerticalLayoutGroup;
    public static Lobby_Scene_Manager LobbyUIManager_Instance { get; set; }

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void UpdateMemberListUI()
    {
        GameObject Button;
        foreach (Friend members in LobbyManager.LobbyManager_Instance.memberList)
        {
            Debug.Log(members);
            Button = Instantiate(MenuPrefabButton, MenuVector.transform);

        }
        VerticalLayoutGroup.CalculateLayoutInputVertical();
    }
    public override void ChildAwake()
    {

        InvokeRepeating("UpdateMemberListUI", 0, 10);
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
