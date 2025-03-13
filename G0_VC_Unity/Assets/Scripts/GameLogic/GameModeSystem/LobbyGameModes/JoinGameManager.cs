using System;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using Steamworks;
using UnityEngine.UIElements;
using System.Collections.Generic;
using TMPro;

public class JoinGameManager : NetworkBehaviour
{
    [HideInInspector] public ulong GameID;
    public GameObject scrolled;
    public TeamClass teamContained;
    public Action updateUIList;
    public List<TeamClass> teamClasses;
    public int sliderValued;

    public void Start()
    {
        GameID = SteamClient.SteamId;
    }

    public void OnDefine(TeamClass teamContain, Action updateUI, List<TeamClass> teamClass,int sliderValue)
    {
        teamContained = teamContain;
        updateUIList = updateUI;
        teamClasses = teamClass;
        sliderValued = sliderValue;
    }

    public void OnClick()
    {
        Debug.Log("HELLO");
        Debug.Log("teamContain" + teamContained.Friends.Count);
        Debug.Log("IOAMHOST");

        if (teamContained.Friends.Count == 0)
        {
            RemovePlayerFromOtherTeams(GameID);
            teamContained.Friends.Add(GameID);
            Debug.Log("OEKGOEKPEGEGK");

            updateUIList.Invoke();
            return;
        }else if(teamContained.Friends.Count>sliderValued+2)
        {
            return;
        }

        foreach (ulong member in teamContained.Friends)
        {
            Debug.Log("RUNNING");
            Debug.Log("GEOKGE" + GameID);

            if (member == GameID)
            {
                Debug.Log("IAM RETURNED");
                return;
            }
        }

        Debug.Log("OEKGOEKPEGEGK");
        RemovePlayerFromOtherTeams(GameID);
        teamContained.Friends.Add(GameID);
        
        updateUIList.Invoke();
    }

    private void RemovePlayerFromOtherTeams(ulong gameId)
    {
        foreach (TeamClass team in teamClasses)
        {
            team.Friends.Remove(gameId);
            Debug.Log("lhoeoe" + team.Friends.Count);
        }
    }

    //public void UpdateNetworkVariable(ulong gameId)
    //{
    //    if (IsHost)
    //    {
    //        foreach (TeamClass team in teamClasses)
    //        {
    //            team.Friends.Remove(gameId);
    //        }
    //    }
    //}

    //[Rpc(SendTo.Everyone)]
    //private void UpdateMainNetworkVariableClientRpc()
    //{
    //    UpdateNetworkVariable(GameID);
    //}
}


