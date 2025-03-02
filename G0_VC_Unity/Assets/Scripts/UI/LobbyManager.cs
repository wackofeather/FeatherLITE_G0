using System.Collections.Generic;
using System.Linq;
using Steamworks;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager:GeneralManager
{
    public List<Friend> memberList;

    public string preGameScene;
    public NetworkVariable<float> countDown = new NetworkVariable<float>();
    public static LobbyManager LobbyManager_Instance { get; set; }

    public List<MapButton> MapButtonList = new List<MapButton>();
    // Start is called once before the first execution of Update after the MonoBehaviour is create


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsServer)
        {
            countDown.Value = 10;
        }
    }
    private void Update()
    {
        if (IsServer)
        {
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
    public void GoToGame()
    {
        NetworkManager.SceneManager.LoadScene(preGameScene, LoadSceneMode.Single);
    }
    public void VoteFor()
    {

    }
    public override void _Start()
    {
        base._Start();
        memberList = SteamLobbyManager.currentLobby.Members.ToList<Friend>();
        
    }
    public override void _Awake()
    {
        base._Awake();
        CounstructLobbyManagerSingleton();
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
