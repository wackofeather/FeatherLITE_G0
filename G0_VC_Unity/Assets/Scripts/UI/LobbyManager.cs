using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using Steamworks.Data;
using Steamworks.ServerList;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LobbyManager:GeneralManager
{
    public List<Friend> memberList;
    
    public string preGameScene;
    public NetworkVariable<float> countDown = new NetworkVariable<float>();
    public static LobbyManager LobbyManager_Instance { get; set; }

    public List<MapButton> MapButtonList = new List<MapButton>();

    public Base_LobbyGameMode CurrentGameMode;

    public NetworkVariable<int> CurrentGameMode_Int = new NetworkVariable<int>();

    public int totalVotes;
    // Start is called once before the first execution of Update after the MonoBehaviour is create


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsServer)
        {
            countDown.Value = 31;
        }
    }
    private void Update()
    {
        if (IsServer)
        {
            CurrentGameMode_Int.Value = CurrentGameMode.GameModeKey;
            countDown.Value -= Time.deltaTime;
            if (countDown.Value <= 0)
            {

                SteamLobbyManager.currentLobby.SetData("Map",GetVotedMap().MapName);

                GoToGame();
            }
        }
        else
        {
            if (SteamLobbyManager.currentLobby.GetData("Map") != "Lobby")
            {
                GoToGame();
            }


        }
    }
    public MapData GetVotedMap()
    {
        MapButton winningMap = MapButtonList[0];
        foreach (MapButton mapButton in MapButtonList)
        {
            if(mapButton.voteCount.Value > winningMap.voteCount.Value)
            {
                winningMap = mapButton;
         
            }
        }
        return winningMap.mapData;
    }

    public void TotalMapVotes()
    {
            totalVotes = MapButtonList[0].voteCount.Value + MapButtonList[1].voteCount.Value;
        
            if (totalVotes == memberList.Count)
            {
            Debug.Log("hoolalala" + totalVotes);
            countDown.Value = 6;
                CancelInvoke("TotalMapVotes");
            }
        
    }    

    public void GoToGame()
    {
        NetworkManager.SceneManager.LoadScene(preGameScene, LoadSceneMode.Single);
    }
    public void VoteFor()
    {

    }
    public void ReloadMemberList()
    {
        memberList = SteamLobbyManager.currentLobby.Members.ToList<Friend>();
        Debug.Log("memberList" + memberList.Count);
    }
    public override void _Start()
    {
        base._Start();
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave+= OnLobbyMemberLeave;
        InvokeRepeating("TotalMapVotes", 0, 1);
    }

    private void OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {
        ReloadMemberList();
        CurrentGameMode.GameMode_MemberJoined(friend);
    }
    private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
    {
        ReloadMemberList();
        CurrentGameMode.GameMode_MemberLeave(friend);
    }
    public override void _Awake()
    {
        base._Awake();

        CounstructLobbyManagerSingleton();
        ReloadMemberList();
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        
    }
    public void CounstructLobbyManagerSingleton()
    {
        if (LobbyManager_Instance != null && LobbyManager_Instance != this)
        {
            Destroy(this);
        }
        else
        {
            LobbyManager_Instance = this;
        }
    }
}
