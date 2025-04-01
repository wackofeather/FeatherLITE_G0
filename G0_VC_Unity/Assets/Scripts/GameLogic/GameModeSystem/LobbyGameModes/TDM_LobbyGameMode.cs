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
    public NetworkVariable<TeamList> teamLists = new NetworkVariable<TeamList>(null);
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
        if (currentValue == 1)
        {
            if (IsHost)
            {
                TeamList list = new TeamList();
                list.ListClass.Add(new TeamClass());
                teamLists.Value = list;

            }
            else
            {
                teamLists.OnValueChanged += teamListsValueChanged;
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

    public void teamListsValueChanged(TeamList previous, TeamList current)
    {
        RemovedAll();
    }
    private void Update()
    {

        if(Lobby_GeneralManager.LobbyManager_Instance.CurrentGameMode_Int.Value == 1)
        {

            UpdateUIList();

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
        RemovedAll();
        teamLists.Value.ListClass.Clear();
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
            teamLists.Value = list;
            Debug.Log("teamListCount" + teamLists.Value.ListClass.Count);
        }
        teamLists.Value = list;
        
    }

    public override void GameMode_MemberJoined(Friend friend)
    {
        if (!Lobby_GeneralManager.LobbyManager_Instance.IsHost) RemovedAll();

        base.GameMode_MemberJoined(friend);
        RemovedAll();
        teamLists.Value.test += 1;
        Debug.Log("NEW MEMBER JOINED");
        int teamSize = Mathf.CeilToInt(Lobby_GeneralManager.LobbyManager_Instance.memberList.Count / slider.value);
        Debug.Log("This is the new teamSize" + teamSize);
        List<Friend> localUserList = new List<Friend>();
        localUserList.Add(friend);
        Debug.Log("NONONM");
        foreach (TeamClass teamClass in teamLists.Value.ListClass)
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
                teamLists.Value.ListClass.Add(newTeamClass);
                localUserList.RemoveAt(0);
            }

        }
    }

    public override void GameMode_MemberLeave(Friend friend)
    {
        if (!Lobby_GeneralManager.LobbyManager_Instance.IsHost) RemovedAll();
        base.GameMode_MemberLeave(friend);
        RemovedAll();
        Debug.Log("team member left");
        List<Friend> localUserList = new List<Friend>();
        int teamSize = Mathf.CeilToInt(Lobby_GeneralManager.LobbyManager_Instance.memberList.Count / slider.value);
        foreach (TeamClass teamClass in teamLists.Value.ListClass)
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
    public void RemovedAll()
    {
        foreach (GameObject obj in ActiveObjects)
        {
            ObjectPoolingScript.ObjectPoolingScript_Instance.pooledObjects.Add(obj);
            obj.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ButtonId = 0;
            obj.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().teamClass = null;
            obj.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().tmpText2.text = "";
            obj.transform.SetParent(ObjectPoolingScript.ObjectPoolingScript_Instance.MenuVector);
        }
        ActiveObjects.Clear();
    }
    public override void UpdateUIList()
    {
        base.UpdateUIList();
        Debug.Log("Update Triggered");
        Debug.Log(",miniingin" + teamLists.Value.ListClass.Count);
        GameObject Button;
        GameObject Button2;
        foreach (TeamClass team in teamLists.Value.ListClass)
        {
            Debug.Log("THISISTEAMFRIENDS" + team.Friends.Count);
            //UnityEngine.Color randomColor = new UnityEngine.Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);


                //Button2.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => OnClick(team));
                foreach (ulong friend in team.Friends)
                {
                if (ActiveObjects.Find(x => x.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ButtonId == friend))
                {
                    Debug.Log("muahaha" + ActiveObjects.Count);
                }
                else
                {
                    Debug.Log("muahaha" + ActiveObjects.Count);
                    Button = ObjectPoolingScript.ObjectPoolingScript_Instance.GetPooledObject();
                    ActiveObjects.Add(Button);
                    ////ColorBlock colorBlock = Button.GetComponentInChildren<UnityEngine.UI.Button>().colors;
                    ////colorBlock.normalColor = randomColor;
                    ////Button.GetComponentInChildren<UnityEngine.UI.Button>().colors = colorBlock;
                    //Button.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ConstructTeamButton(friend);
                    Button.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ButtonId = friend;
                    Button.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ConstructTeamButton(friend);
                    Button.transform.SetParent(MenuVector.transform, true);
                }
                }
                //ikgkgogkr

                    if (ActiveObjects.Find(x => x.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().teamClass == team))
                    {
                        Debug.Log("OH NO");
                    }
                    else
                    {
                        Debug.Log("hello");
                        Button2 = ObjectPoolingScript.ObjectPoolingScript_Instance.GetPooledObject();
                        ActiveObjects.Add(Button2);
                        Button2.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().teamClass = team;
                        Button2.transform.SetParent(MenuVector.transform, true);
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


