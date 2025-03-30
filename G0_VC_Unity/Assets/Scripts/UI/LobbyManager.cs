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



public class LobbyManager : GeneralManager
{
    public List<Friend> memberList;

    public string preGameScene;
    public NetworkVariable<float> countDown = new NetworkVariable<float>();

    public int map1 = 0;
    public int map2 = 0;

    public int totalVotes;
    public static LobbyManager LobbyManager_Instance { get; set; }

    public List<MapButton> MapButtonList = new List<MapButton>();

    public Base_LobbyGameMode CurrentGameMode;

    public NetworkVariable<int> CurrentGameMode_Int = new NetworkVariable<int>();


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ReloadMemberList();
        if ( IsHost&& SteamLobbyManager.instance.reconnecting==false)
        {

            countDown.Value = 110;
            totalVotes = 0;
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            //CurrentGameMode_Int.Value = CurrentGameMode.GameModeKey;
            countDown.Value -= Time.deltaTime;
            if (countDown.Value <= 0)
            {
                SteamLobbyManager.currentLobby.SetData("Map", GetVotedMap().MapName);
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
        int winningMap = 0;
        if (map2 > map1)
        {
            winningMap = 1;
        }else 
        {
            winningMap = 0;
        }
        return MapButtonList[winningMap].mapData;
    }

    public void TotalMapVotes()
    {
        
        totalVotes = map1 + map2;

        if (totalVotes == memberList.Count&&IsHost)
        {
            countDown.Value = 10;
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
    }

    public override void _Start()
    {
        base._Start();
        ReloadMemberList();
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
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

    public void UpdateVoteServer(int map1Change, int map2Change)
    {
        if(IsHost)
        {
            Debug.Log("ServerRpc called");
            map1 += map1Change;
            map2 += map2Change;
            Debug.Log("map1" + map1);
            Debug.Log("map2" + map2);
            Debug.Log("totalVotes" + totalVotes);
            TotalMapVotes();
        }
    }



 


}


