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
using System.Threading.Tasks;


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
            for (int j = 0; j < count; j++)
            {
                Friends.Add(0); // Add default elements to avoid "index out of range"
            }
        }

        for (int i = 0; i < count; i++)
            {
                ulong friend = serializer.IsReader ? 0 : Friends[i];
                serializer.SerializeValue(ref friend);
                if (serializer.IsReader&&Friends.Count!=0)
                {
                    Debug.Log("MINIBIFI" + i);
                    Debug.Log("MINIBIFI" + Friends.Count);
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
    public float test = 1;
    public List<TeamClass> ListClass = new List<TeamClass>();
    //public List<int> ListIndex = new List<int>();

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        int count = ListClass.Count;
        serializer.SerializeValue(ref count);

        if (serializer.IsReader)
        {
            ListClass = new List<TeamClass>(count);
            for (int j = 0; j < count; j++)
            {
                ListClass.Add(null); // Add default elements to avoid "index out of range"
            }
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

//public class TeamList : INetworkSerializable, IEquatable<TeamList>
//{
//    public float test = 1;
//    //public List<TeamClass> ListClass = new List<TeamClass>();
//    public List<int> ListIndex = new List<int>();

//    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
//    {
//        int count = ListIndex.Count;
//        serializer.SerializeValue(ref count);

//        if (serializer.IsReader)
//        {
//            ListIndex = new List<int>(count);
//        }

//        for (int i = 0; i < count; i++)
//        {
//            int value = serializer.IsReader ? 0 : ListIndex[i];
//            serializer.SerializeValue(ref value);
//            if (serializer.IsReader)
//            {
//                ListIndex[i] = value;
//            }
//        }
//    }

//    public bool Equals(TeamList other)
//    {
//        if (other == null) return false;
//        if (ListIndex.Count != other.ListIndex.Count) return false;
//        for (int i = 0; i < ListIndex.Count; i++)
//        {
//            if (ListIndex[i] != other.ListIndex[i]) return false;
//        }
//        return true;
//    }

//    public override int GetHashCode()
//    {
//        int hash = 17;
//        foreach (var index in ListIndex)
//        {
//            hash = hash * 31 + index.GetHashCode();
//        }
//        return hash;
//    }
//}



public class TDM_LobbyGameMode : Base_LobbyGameMode
{
    public UnityEngine.UI.Slider slider;
    public GameObject spacerObject;
    public GameObject JoinTeamButton;
    public GameObject Trigger;
    public NetworkVariable<TeamList> network_teamList = new NetworkVariable<TeamList>(null);
    TeamList local_teamList;
    public NetworkVariable<float> TeamSizeNetwork = new NetworkVariable<float>();
    
    [HideInInspector] public ulong GameID;




    //public void OnEnable()
    //{
        
    //}

    //public void Update()
    //{
    //    // Debug.Log(teamLists.Value);
    //}

    public override void OnGameModeSwitch(int previousValue, int currentValue)
    {
        base.OnGameModeSwitch(previousValue, currentValue);
        Debug.Log("natoma");
        if (currentValue == 1)
        {
            if (IsHost)
            {
                TeamList list = new TeamList();
                list.ListClass.Add(new TeamClass());
                network_teamList.Value = list;

            }
            
            TeamSetting();
            //if (IsHost)
            //{
            //    //teamLists.Value = new TeamList();
            //    //teamLists.Value.ListIndex.Add(1);
            //    //TeamList newList = new TeamList();
            //    //TeamList newList = new TeamList();
            //    //newList.ListClass.Add(new TeamClass());
            //    //teamLists.Value = newList;
            //    //Debug.Log("WOLOLO" + teamLists.Value.ListClass.Count);
            //    teamSize.Value += 1;
            //    Debug.Log(teamSize.Value);
            //    //TeamSetting();
            //}


            GameID = SteamClient.SteamId;
        }

    }
    private void Update()
    {
        Debug.Log("lettermane " + TeamVerified);

        local_teamList = network_teamList.Value;

        if (Lobby_GeneralManager.LobbyManager_Instance.CurrentGameMode_Int.Value == 1)
        {

            UpdateUIList();

            if (TeamVerified)
            {
                
                
            }
            
        }
    }
    //public override void OnGameModeSwitch(int previousValue, int currentValue)
    //{
    //    base.OnGameModeSwitch (previousValue, currentValue);
    //    if (currentValue == 1)
    //    {
    //        if (Lobby_GeneralManager.LobbyManager_Instance.IsHost)
    //        {
    //            //teamLists.Value = new TeamList();
    //            //teamLists.Value.ListIndex.Add(1);
    //            //TeamList newList = new TeamList();
    //            TeamList newList = new TeamList();
    //            newList.ListClass.Add(new TeamClass());
    //            teamLists.Value = newList;
    //            Debug.Log("WOLOLO" + teamLists.Value.ListClass.Count);
    //            //teamSize.Value += 1;
    //            //TeamSetting();
    //        }
    //        else
    //        {
    //            Debug.Log("WOLOLO" + teamLists.Value.ListClass.Count);
    //            //Debug.Log("Iamtriggered");
    //            //Debug.Log(",miniingin" + teamLists.Value.ListClass);
    //            //Debug.Log(",miniingin" + teamLists.Value.ListIndex.Count);
    //            //Debug.Log(teamSize.Value);
    //            //UpdateUIList();
    //        }

    //        GameID = SteamClient.SteamId;
    //    }
    //}
    public void TeamSetting()
    {
        if (!Lobby_GeneralManager.LobbyManager_Instance.IsHost) return;
        ObjectPoolingScript.ObjectPoolingScript_Instance.Repool();
        network_teamList.Value.ListClass.Clear();
        Debug.Log("IAMCLALED");
        TeamList list = new TeamList();
        List<Friend> FriendList = new List<Friend>(Lobby_GeneralManager.LobbyManager_Instance.memberList);
        //teamLists.Value.ListClass.Clear();
        int teamSize = Mathf.CeilToInt(Lobby_GeneralManager.LobbyManager_Instance.memberList.Count / slider.value);
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
            list.ListClass.Add(teamClass);
            network_teamList.Value = list;
            Debug.Log("teamListCount" + network_teamList.Value.ListClass.Count);
        }
        network_teamList.Value = list;
        
    }

    public override void GameMode_MemberJoined(Friend friend)
    {
        base.GameMode_MemberJoined(friend);
        if (!IsHost) return;
        ObjectPoolingScript.ObjectPoolingScript_Instance.Repool();
        network_teamList.Value.test += 1;
        Debug.Log("NEW MEMBER JOINED");
        int teamSize = Mathf.CeilToInt(Lobby_GeneralManager.LobbyManager_Instance.memberList.Count / slider.value);
        Debug.Log("This is the new teamSize" + teamSize);
        List<Friend> localUserList = new List<Friend>();
        localUserList.Add(friend);
        Debug.Log("NONONM");
        foreach (TeamClass teamClass in network_teamList.Value.ListClass)
        {
            if (teamClass.Friends.Count > teamSize)
            {
                for (int j = 0; j < (teamClass.Friends.Count - teamSize); j++)
                {
                    localUserList.Add(Lobby_GeneralManager.LobbyManager_Instance.memberList.Find(x => x.Id == teamClass.Friends[0]));
                    teamClass.Friends.RemoveAt(0);
                }
            }
            else if (teamClass.Friends.Count < teamSize)
            {
                Debug.Log("Team is lacking members");
                for (int j = 0; j < (teamSize - teamClass.Friends.Count) && localUserList.Count > 0; j++)
                {
                    Debug.Log("EEE" + teamClass.Friends.Count);
                    teamClass.Friends.Add(localUserList[0].Id);

                    localUserList.RemoveAt(0);
                }
            }
            else
            {
                Debug.Log("IAM TRIGGERED");
                TeamClass newTeamClass = new TeamClass();
                newTeamClass.AddFriend(localUserList[0].Id);
                network_teamList.Value.ListClass.Add(newTeamClass);
                localUserList.RemoveAt(0);
            }

        }
    }

    public override void GameMode_MemberLeave(Friend friend)
    {
        base.GameMode_MemberLeave(friend);
        if (!IsHost) return;
        ObjectPoolingScript.ObjectPoolingScript_Instance.Repool();
        Debug.Log("team member left");
        List<Friend> localUserList = new List<Friend>();
        int teamSize = Mathf.CeilToInt(Lobby_GeneralManager.LobbyManager_Instance.memberList.Count / slider.value);
        foreach (TeamClass teamClass in network_teamList.Value.ListClass)
        {
            if(teamClass.Friends.Contains(friend.Id))
            {
                teamClass.Friends.Remove(friend.Id);
            }
            if (teamClass.Friends.Count > teamSize)
            {
                for (int j = 0; j < (teamClass.Friends.Count - teamSize); j++)
                {
                    localUserList.Add(Lobby_GeneralManager.LobbyManager_Instance.memberList.Find(x => x.Id == teamClass.Friends[0]));
                    teamClass.Friends.RemoveAt(0);
                    Debug.Log("WALALA" + teamClass.Friends[0]);
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
                Debug.Log("IAM TRIGGERED");
                teamClass.Friends.Remove(friend.Id);
            }

        }
 
    }
    
    public override void UpdateUIList()
    {
        base.UpdateUIList();
        Debug.Log("Update Triggered");
        Debug.Log(",miniingin" + local_teamList.ListClass.Count);

        for (int i = 0; i < local_teamList.ListClass.Count; i++)
        {
            TeamClass team = local_teamList.ListClass[i];
            Debug.Log("THISISTEAMFRIENDS" + team.Friends.Count);
            //UnityEngine.Color randomColor = new UnityEngine.Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);


                //Button2.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => OnClick(team));
            foreach (ulong friend in team.Friends)
            {
                GameObject Button;
                bool currentFriendThumbnail = false;
                List<GameObject> existingLikeFriends = ObjectPoolingScript.ObjectPoolingScript_Instance.ActiveObjects.FindAll(x => x.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ButtonId == friend);
                for (int j = 0; j < existingLikeFriends.Count; j++)
                {
                    GameObject thumbnail = existingLikeFriends[j];
                    if (thumbnail.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().Team_Index == j) { currentFriendThumbnail = true; continue; }
                    ObjectPoolingScript.ObjectPoolingScript_Instance.ReturnToPool(thumbnail);
                }
                if (currentFriendThumbnail == false)
                {
                    List<GameObject> existingMyThumbnails = ObjectPoolingScript.ObjectPoolingScript_Instance.ActiveObjects.FindAll(x => x.GetComponent<Lobby_Player_Buttons_Helpers>().ButtonId == friend);

                    foreach (GameObject undesirable_button in existingMyThumbnails)
                    {
                        ObjectPoolingScript.ObjectPoolingScript_Instance.ReturnToPool(undesirable_button);
                    }

                    Button = ObjectPoolingScript.ObjectPoolingScript_Instance.GetPooled_PlayerThumbnailObject(i);
                    ////ColorBlock colorBlock = Button.GetComponentInChildren<UnityEngine.UI.Button>().colors;
                    ////colorBlock.normalColor = randomColor;
                    ////Button.GetComponentInChildren<UnityEngine.UI.Button>().colors = colorBlock;
                    //Button.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ConstructTeamButton(friend);
                    
                    Button.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ConstructFriendButton(Lobby_GeneralManager.LobbyManager_Instance.memberList.Find(x => x.Id == friend));
                    Button.transform.SetParent(MenuVector.transform);
                }
            }
                //ikgkgogkr

            if (!ObjectPoolingScript.ObjectPoolingScript_Instance.ActiveObjects.Find(x => x.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().Team_Index == i && x.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ButtonId == 0))
            {
                GameObject SwitchButton;
                Debug.Log("hello");
                SwitchButton = ObjectPoolingScript.ObjectPoolingScript_Instance.GetPooled_TeamSwitchObject(i);
                SwitchButton.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ConstructTeamButton();
                SwitchButton.transform.SetParent(MenuVector.transform);
            }


                //if(ObjectPoolingScript.ObjectPoolingScript_Instance.ActiveObjects.Count > 0)
                //{
                //    if (!ObjectPoolingScript.ObjectPoolingScript_Instance.ActiveObjects.Find(x => x.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ButtonId == friend))
                //    {
                //        Button = ObjectPoolingScript.ObjectPoolingScript_Instance.GetPooledObject(MenuVector, friend);
                //        Button.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ButtonId = friend;
                //        Button.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ConstructTeamButton(friend);
                //        Buttons.Add(Button);
                //    }
                //}
                //else
                //{
                //    Button = ObjectPoolingScript.ObjectPoolingScript_Instance.GetPooledObject(MenuVector, friend);
                //    Button.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ButtonId = friend;
                //    Button.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ConstructTeamButton(friend);
                //    Buttons.Add(Button);
                //}
                //Debug.Log("IAMTRIGGERING" + friend);
                //Button = Instantiate(MenuPrefabButton, MenuVector);
                ////ColorBlock colorBlock = Button.GetComponentInChildren<UnityEngine.UI.Button>().colors;
                ////colorBlock.normalColor = randomColor;
                ////Button.GetComponentInChildren<UnityEngine.UI.Button>().colors = colorBlock;
                //Button.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ConstructTeamButton(friend);
                //Buttons.Add(Button);
                //Debug.Log("ths is button count" + Buttons.Count);
        }

        VerticalLayoutGroup.CalculateLayoutInputVertical();
    }




    //Team Switching
    //Team Switching
    public int myTeam = -2;
    public bool TeamVerified = true;
    [Rpc(SendTo.Server)]
    public void RequestTeam_ServerRPC(int requested_playerTeam, int current_playerTeam, ulong steamId, ulong networkID)
    {
        int finalizedTeam = 0;

        if (network_teamList.Value.ListClass[requested_playerTeam].Friends.Find(x => x == steamId) != 0)
        {
            //playerisontheRightTeam
            finalizedTeam = requested_playerTeam;
        }
        else
        {
            if (network_teamList.Value.ListClass[requested_playerTeam].Friends.Count < Mathf.CeilToInt(Lobby_GeneralManager.LobbyManager_Instance.memberList.Count / slider.value))
            {
                //has space

                network_teamList.Value.ListClass[current_playerTeam].Friends.Remove(steamId);

                network_teamList.Value.ListClass[requested_playerTeam].Friends.Add(steamId);
                
            }
            else 
            {
                finalizedTeam = current_playerTeam; 
            }
            
    //shuffle teams, try
    //else, keep player on same team
        }


        ValidateTeamList_ClientRPC(finalizedTeam, network_teamList.Value, RpcTarget.Single(networkID, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void ValidateTeamList_ClientRPC(int forced_playerTeam, TeamList forced_teamList, RpcParams param)
    {
        network_teamList.Value = forced_teamList;
        myTeam = forced_playerTeam;
        TeamVerified = true;
    }



    public async void SwitchPlayerTeam(int teamId)
    {
        if (myTeam == -2)
        {
            for (int i = 0; i < network_teamList.Value.ListClass.Count; i++)
            {
                TeamClass team = network_teamList.Value.ListClass[i];
                if (team.Friends.Contains(SteamClient.SteamId)) myTeam = i;
            }
        }
        
        //switch teams locally
        
        TeamVerified = false;

        while (TeamVerified == false)
        {
            Debug.Log("waiting for fucking" + myTeam);
            RequestTeam_ServerRPC(teamId, myTeam, SteamClient.SteamId, NetworkManager.LocalClientId);
            await Task.Yield();
        }

        if (myTeam != teamId) { } //switch unsuccessful
    }
}



    //private void ResetRectTransform(RectTransform rectTransform)
    //{
    //    rectTransform.localScale = Vector3.one;
    //    rectTransform.sizeDelta = new Vector2(300, 30); // Set to your desired size
    //    rectTransform.anchoredPosition3D = Vector3.zero;
    //}


    //public void OnClick(TeamClass teamContained)
    //{
    //    if (teamContained.Friends.Count == 0)
    //    {
    //        RemovePlayerFromOtherTeams(GameID);
    //        AddPlayertoTeams(GameID, teamContained);
    //        Debug.Log("OEKGOEKPEGEGK");
    //        Lobby_GeneralManager.LobbyManager_Instance.CurrentGameMode.UpdateUIList();
    //        return;
    //    }
    //    else if (teamContained.Friends.Count > slider.value + 2)
    //    {
    //        return;
    //    }

    //    foreach (ulong member in teamContained.Friends)
    //    {
    //        Debug.Log("RUNNING");
    //        Debug.Log("GEOKGE" + GameID);

    //        if (member == GameID)
    //        {
    //            Debug.Log("IAM RETURNED");
    //            return;
    //        }
    //    }

    //    Debug.Log("OEKGOEKPEGEGK");
    //    RemovePlayerFromOtherTeams(GameID);
    //    AddPlayertoTeams(GameID, teamContained);

    //    Lobby_GeneralManager.LobbyManager_Instance.CurrentGameMode.UpdateUIList();
    //}

    //private void AddPlayertoTeams(ulong gameId, TeamClass teamContained)
    //{
    //    teamContained.AddFriend(gameId);
    //}

    //private void RemovePlayerFromOtherTeams(ulong gameId)
    //{
    //    foreach (TeamClass team in teamLists.Value.ListClass)
    //    {
    //        team.Friends.Remove(gameId);
    //        Debug.Log("lhoeoe" + team.Friends.Count);
    //    }
    //}



//public void UpdateClient (int mini)
//{

//}//wgwogo


