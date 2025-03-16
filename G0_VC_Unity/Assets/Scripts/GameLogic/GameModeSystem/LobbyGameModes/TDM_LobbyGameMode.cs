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
using TMPro;
using Steamworks.Data;
using UnityEngine.InputSystem;


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

        for (int i = 0; i < count &&count>0; i++)
        {
            ulong friend = serializer.IsReader ? 0 : Friends[i];
            serializer.SerializeValue(ref friend);
            //if (serializer.IsReader)
            //{
            //    Friends[i] = friend;
            //}
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
            //if (serializer.IsReader)
            //{
            //    ListClass[i] = team;
            //}
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
    public NetworkVariable<TeamList> TeamContainer = new NetworkVariable<TeamList>();
    public UnityEngine.UI.Slider slider;
    public GameObject spacerObject;
    public GameObject JoinTeamButton;
    public GameObject Trigger;
    [HideInInspector] public ulong GameID;

    //private void Awake()
    //{
    //    TeamContainer.OnValueChanged += OnTeamContainerChanged;
    //}

    //private void OnDestroy()
    //{
    //    TeamContainer.OnValueChanged -= OnTeamContainerChanged;
    //}

    //private void OnTeamContainerChanged(TeamList previous, TeamList current)
    //{
    //    Debug.Log("TeamContainer changed");
    //    UpdateUIList();
    //}

    private void CheckForEnable()
    {
        if (LobbyManager.LobbyManager_Instance.CurrentGameMode_Int.Value == 1)
        { 
                if (LobbyManager.LobbyManager_Instance.IsHost)
                {
                    TeamContainer.Value = TeamContainer.Value;
                    TeamSetting();

                CancelInvoke("CheckForEnable");

            }

                GameID = SteamClient.SteamId;

        }
    }

    //public void OnEnable()
    //{
    //    if(LobbyManager.LobbyManager_Instance.IsHost)
    //    {
    //        TeamContainer.Value = TeamContainer.Value;
    //        TeamSetting();



    //    }

    //    GameID = SteamClient.SteamId;
    //}
    public void Start()
    {
        InvokeRepeating("CheckForEnable", 0, 1);    
    }

    public void TeamSetting()
    {
        if (!LobbyManager.LobbyManager_Instance.IsHost) return;
        Debug.Log("IAMCLALED");
        List<Friend> FriendList = new List<Friend>(LobbyManager.LobbyManager_Instance.memberList);
        TeamContainer.Value.ListClass.Clear();
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
        UpdateUIList();
        UpdateClient();

    }

    public override void GameMode_MemberJoined(Friend friend)
    {
        if (!LobbyManager.LobbyManager_Instance.IsHost) return;
        base.GameMode_MemberJoined(friend);
       

        Debug.Log("NEW MEMBER JOINED");
        int teamSize = Mathf.CeilToInt(LobbyManager.LobbyManager_Instance.memberList.Count / slider.value);
        Debug.Log("This is the new teamSize" + teamSize);
        List<Friend> localUserList = new List<Friend>();
        localUserList.Add(friend);
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
            else
            {
                TeamClass teamClasses = new TeamClass();
                teamClasses.AddFriend(localUserList[0].Id);
                TeamContainer.Value.ListClass.Add(teamClasses);
                localUserList.RemoveAt(0);
            }
        }
        TeamContainer.SetDirty(true); // Mark the NetworkVariable as dirty to ensure the change is propagated
        UpdateUIList();
        UpdateClient();
    }

    public override void GameMode_MemberLeave(Friend friend)
    {
        if (!LobbyManager.LobbyManager_Instance.IsHost) return;
        base.GameMode_MemberLeave(friend);
       

        Debug.Log("team member left");
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
            else
            {
                teamClass.Friends.Remove(friend.Id);
            }
        }
        TeamContainer.SetDirty(true); // Mark the NetworkVariable as dirty to ensure the change is propagated
        UpdateUIList();
        UpdateClient();
    }

    public override void UpdateUIList()
    {
        base.UpdateUIList();
        ClearButtons();
        Debug.Log("Update Triggered");

        GameObject Button;
        GameObject Button2;
        foreach (TeamClass team in TeamContainer.Value.ListClass)
        {
            Debug.Log("THISISTEAMFRIENDS" + team.Friends.Count);
            UnityEngine.Color randomColor = new UnityEngine.Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

            if (team.Friends.Count != 0)
            {

                Button2 = Instantiate(JoinTeamButton, MenuVector);
                Button2.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnClick(team));
                Buttons.Add(Button2);
                foreach (ulong friend in team.Friends)
                {
                    Debug.Log("IAMTRIGGERING" + friend);
                    Button = Instantiate(MenuPrefabButton, MenuVector);
                    ColorBlock colorBlock = Button.GetComponent<UnityEngine.UI.Button>().colors;
                    colorBlock.normalColor = randomColor;
                    Button.GetComponent<UnityEngine.UI.Button>().colors = colorBlock;
                    Button.GetComponent<Lobby_Player_Buttons_Helpers>().ConstructTeamButton(friend);
                    Buttons.Add(Button);
                    Debug.Log("ths is button count" + Buttons.Count);
                }
                VerticalLayoutGroup.CalculateLayoutInputVertical();
            }
        }
    }

    public void OnClick(TeamClass teamContained)
    {
        if (teamContained.Friends.Count == 0)
        {
            RemovePlayerFromOtherTeams(GameID);
            AddPlayertoTeams(GameID, teamContained);
            Debug.Log("OEKGOEKPEGEGK");
            LobbyManager.LobbyManager_Instance.CurrentGameMode.UpdateUIList();
            return;
        }
        else if (teamContained.Friends.Count > slider.value + 2)
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
        AddPlayertoTeams(GameID, teamContained);

        LobbyManager.LobbyManager_Instance.CurrentGameMode.UpdateUIList();
    }

    private void AddPlayertoTeams(ulong gameId, TeamClass teamContained)
    {
        teamContained.AddFriend(gameId);
    }

    private void RemovePlayerFromOtherTeams(ulong gameId)
    {
        foreach (TeamClass team in TeamContainer.Value.ListClass)
        {
            team.Friends.Remove(gameId);
            Debug.Log("lhoeoe" + team.Friends.Count);
        }
    }

    private void UpdateClient()
    {
        if (IsHost)
        {
            Debug.Log("TRIGGERED");

            UpDaterCatchServerRpc();
        }

    }

    [Rpc(SendTo.Server)]
    public void UpDaterCatchServerRpc()
    {
        Debug.Log("TRIGGERED" + TeamContainer.Value.ListClass.Count);
        UpdateClientRpc();

    }
    [Rpc(SendTo.Everyone)]

    public void UpdateClientRpc()
    {
        Debug.Log("TRIGGERED" + TeamContainer.Value.ListClass.Count);
        if (!IsHost)
        {
            UpdateUIList();
        }
    }
}


