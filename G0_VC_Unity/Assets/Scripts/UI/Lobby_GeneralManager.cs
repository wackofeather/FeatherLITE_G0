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



public class Lobby_GeneralManager : GeneralManager
{
    public List<Friend> memberList;

    public string preGameScene;
    public NetworkVariable<float> countDown = new NetworkVariable<float>();

    public static Lobby_GeneralManager LobbyManager_Instance { get; set; }

    public List<MapButton> MapButtonList = new List<MapButton>();

    public Base_LobbyGameMode CurrentGameMode;

    public NetworkVariable<int> CurrentGameMode_Int = new NetworkVariable<int>();

    public MapButton myVote;

    public Dictionary<ulong, string> MapVotes;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ReloadMemberList();
        if (IsHost && SteamLobbyManager.instance.reconnecting==false)
        {

            countDown.Value = 110;

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
                SteamLobbyManager.currentLobby.SetData("Map", GetVotedMap());
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

        if (myVote != null) { ServerSendMyVote_RPC(SteamClient.SteamId, myVote.mapData.MapName); }
    }

    [Rpc(SendTo.Server)]
    public void ServerSendMyVote_RPC(ulong voter, string mapName)
    {
        
        if (MapVotes.ContainsKey(voter))
        {
            MapVotes[voter] = mapName;
        }
        else
        {
            MapVotes.Add(voter, mapName);
            
            if (MapVotes.Count == SteamLobbyManager.currentLobby.MemberCount && countDown.Value > 9.1) { countDown.Value = 9; Debug.Log("god fucking damn it"); }
        }

        foreach (MapButton mapButton in MapButtonList)
        {
            mapButton.voteCount.Value = MapVotes.Values.Count(v => v == mapButton.mapData.MapName);
        }
    }

    public string GetVotedMap()
    {
        string MapVoted = MapVotes.Values.GroupBy(value => value).OrderByDescending(group => group.Count()).FirstOrDefault()?.Key;
        if (MapVoted == null) return MapVoted;
        else return "Voting";
    }

    public void GoToGame()
    {
        NetworkManager.SceneManager.LoadScene(preGameScene, LoadSceneMode.Single);
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





 


}


