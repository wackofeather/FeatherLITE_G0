using Steamworks;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;
using System;
using UnityEngine.UIElements;
using System.ComponentModel;
using UnityEditor;
using TMPro;


[Serializable]
public class TeamClass : INetworkSerializable, IEquatable<TeamClass>
{
    public List<ulong> Friends = new List<ulong>();

    public void AddFriend(ulong friend)
    {
        Friends.Add(friend);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        int count = Friends.Count;
        serializer.SerializeValue(ref count);

        if (serializer.IsReader)
        {
            Friends = new List<ulong>(count);
        }

        for (int i = 0; i < count; i++)
        {
            ulong friend = serializer.IsReader ? 0 : Friends[i];
            serializer.SerializeValue(ref friend);
            if (serializer.IsReader)
            {
                Friends[i] = friend;
            }
        }
    }

    public bool Equals(TeamClass other)
    {
        if (other == null) return false;
        if (Friends.Count != other.Friends.Count) return false;
        for (int i = 0; i < Friends.Count; i++)
        {
            if (Friends[i] != other.Friends[i]) return false;
        }
        return true;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (var friend in Friends)
        {
            hash = hash * 31 + friend.GetHashCode();
        }
        return hash;
    }
}

[Serializable]
public class TeamList : INetworkSerializable, IEquatable<TeamList>
{
    public List<TeamClass> ListClass = new List<TeamClass>();

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        int count = ListClass.Count;
        serializer.SerializeValue(ref count);

        if (serializer.IsReader)
        {
            ListClass = new List<TeamClass>(count);
        }

        for (int i = 0; i < count; i++)
        {
            TeamClass team = serializer.IsReader ? new TeamClass() : ListClass[i];
            serializer.SerializeValue(ref team);
            if (serializer.IsReader)
            {
                ListClass[i] = team;
            }
        }
    }

    public bool Equals(TeamList other)
    {
        if (other == null) return false;
        if (ListClass.Count != other.ListClass.Count) return false;
        for (int i = 0; i < ListClass.Count; i++)
        {
            if (!ListClass[i].Equals(other.ListClass[i])) return false;
        }
        return true;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (var team in ListClass)
        {
            hash = hash * 31 + team.GetHashCode();
        }
        return hash;
    }
}




public class TDM_LobbyGameMode : Base_LobbyGameMode
{
    public NetworkVariable<TeamList> TeamContainer = new NetworkVariable<TeamList>(); //change to TeamList
    public UnityEngine.UI.Slider slider;
    public GameObject scroll;
    
    //change to userList


    public void TeamSetting()
    {
        TeamContainer.Value.ListClass.Clear();
        List<Friend> FriendList = new List<Friend>(LobbyManager.LobbyManager_Instance.memberList);

        int teamSize = Mathf.CeilToInt(LobbyManager.LobbyManager_Instance.memberList.Count / slider.value);
        Debug.Log("SliderValue: " + slider.value);
        Debug.Log("TeamSize: " + teamSize);

        for (int i = 0; i < slider.value; i++)
        {
            TeamClass teamClass = new TeamClass();
            for (int j = 0; j < teamSize && FriendList.Count > 0; j++)
            {
                Debug.Log(FriendList[0]);
                teamClass.AddFriend(FriendList[0].Id);
                FriendList.RemoveAt(0);
            }
            TeamContainer.Value.ListClass.Add(teamClass);
        }

        //Debug.Log("Total Teams: " + ListClass.Count);
        //foreach (TeamClass teamClass in ListClass)
        //{
        //    Debug.Log("TeamClass2: " + teamClass);
        //    Debug.Log("ListClassCount2: " + ListClass.Count);
        //    Debug.Log("This is the length of teamClass for this2" + teamClass.Friends.Count);
        //    foreach (ulong teamId in teamClass.Friends)
        //    {

        //        Debug.Log("Team Member ID2: " + teamId);
        //    }
        //}
    }


    public override void GameMode_MemberJoined(Friend friend)
    {
        base.GameMode_MemberJoined(friend);
        Debug.Log("NEW MEMBER JOINED");
        int teamSize = Mathf.CeilToInt(LobbyManager.LobbyManager_Instance.memberList.Count / slider.value);
        Debug.Log("This is the new teamSize" + teamSize);
        List<Friend> localUserList = new List<Friend>();
        foreach (TeamClass teamClass in TeamContainer.Value.ListClass)
        {
            if (teamClass.Friends.Count > teamSize)
            {
                for (int j = 0; j < (teamClass.Friends.Count - teamSize); j++)
                {
                    localUserList.Add(LobbyManager.LobbyManager_Instance.memberList.Find(x => x.Id == teamClass.Friends[0]));
                    teamClass.Friends.RemoveAt(0);
                    
                }
            }
            else if (teamClass.Friends.Count < teamSize)
            {
                Debug.Log("Team is lacking members");
                for (int j = 0; j < (teamSize - teamClass.Friends.Count) && localUserList.Count > 0; j++)
                {
                    teamClass.Friends.Add(localUserList[0].Id);
                    Debug.Log(teamClass.Friends.Count);
                    localUserList.RemoveAt(0);
                }
            }
            UpdateUIList();
        }

        //foreach (TeamClass teamClass in ListClass)
        //{
        //    Debug.Log("ListClassCount: " +ListClass.Count);
        //    Debug.Log("This is the length of teamClass for this" + teamClass.Friends.Count);
        //    foreach (ulong teamId in teamClass.Friends)
        //    {

        //        Debug.Log("Team Member ID: " + teamId);
        //    }
        //}
    }
    public override void GameMode_MemberLeave(Friend friend)
    {
        base.GameMode_MemberLeave(friend);
        List<Friend> localUserList = new List<Friend>();
        int teamSize = Mathf.CeilToInt(LobbyManager.LobbyManager_Instance.memberList.Count / slider.value);
        foreach (TeamClass teamClass in TeamContainer.Value.ListClass)
        {
            if (teamClass.Friends.Count > teamSize)
            {
                for (int j = 0; j < (teamClass.Friends.Count - teamSize); j++)
                {
                    localUserList.Add(LobbyManager.LobbyManager_Instance.memberList.Find(x => x.Id == teamClass.Friends[0]));
                    teamClass.Friends.RemoveAt(0);
                }
            }
            else if (teamClass.Friends.Count < teamSize)
            {
                Debug.Log("Team is lacking members");
                for (int j = 0; j < (teamSize - teamClass.Friends.Count) && localUserList.Count > 0; j++)
                {
                    teamClass.Friends.Add(localUserList[0].Id);
                    Debug.Log(teamClass.Friends.Count);
                    localUserList.RemoveAt(0);
                }
            }
            UpdateUIList();
        }
        
        //foreach (TeamClass teamClass in ListClass)
        //{
        //    Debug.Log("ListClassCount: " + ListClass.Count);
        //    Debug.Log("This is the length of teamClass for this" + teamClass.Friends.Count);
        //    foreach (ulong teamId in teamClass.Friends)
        //    {

        //        Debug.Log("Team Member ID: " + teamId);
        //    }
        //}
    }

    public  void OnEnable()
    {
        TeamSetting();
        UpdateUIList();
    }

 
    public override void UpdateUIList()
    {
        base.UpdateUIList();
        ClearButtons();
        
        GameObject scrollRect;
        
        foreach (TeamClass team in TeamContainer.Value.ListClass)
        {
            
            if (team.Friends.Count>0)
            {
                Debug.Log("THISISTEAMFRIENDS" + team.Friends.Count);
                scrollRect = Instantiate(scroll, MenuVector.transform);
                foreach (ulong friend in team.Friends)
                {
                    Debug.Log("IAMTRIGGERING"+friend);
                    ScrollRectRenderer.scrollRectRendererInstance.CreateButton(friend);
                    VerticalLayoutGroup.CalculateLayoutInputVertical();
                }
                VerticalLayoutGroup.CalculateLayoutInputVertical();


            }
        }
        VerticalLayoutGroup.CalculateLayoutInputVertical();
    }
}
