using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;



public class MapButton : NetworkBehaviour
{
    public NetworkVariable<int> voteCount = new NetworkVariable<int>(0);
    public MapData mapData;
    public SortingGroup sortingGroup;
    public Toggle button;


    
    public void OnHoverEnter()
    {
        gameObject.transform.localScale *= 1.1f;
        sortingGroup.sortingOrder = -1;
    }

    public void OnHoverExit()
    {
        gameObject.transform.localScale /= 1.1f;
        sortingGroup.sortingOrder = 0;
    }
    public void OnToggle()
    {
        Lobby_GeneralManager.LobbyManager_Instance.myVote = this;
    }

    private void Start()
    {
        if (button.isOn) Lobby_GeneralManager.LobbyManager_Instance.myVote = this;
    }


}
//public class MapButton : NetworkBehaviour
//{

//    public int voteCountInt;
//    public MapData mapData;
//    public SortingGroup sortingGroup;

//    public void OnHoverEnter()
//    {
//        gameObject.transform.localScale *= 1.1f;
//        sortingGroup.sortingOrder = -1;
//    }

//    public void OnHoverExit()
//    {
//        gameObject.transform.localScale /= 1.1f;
//        sortingGroup.sortingOrder = 0;
//    }
//    //public override void OnNetworkSpawn()
//    //{
//    //    base.OnNetworkSpawn();
//    //    if (IsHost)
//    //    {
//    //        Lobby_GeneralManager.LobbyManager_Instance.voteCount.Value = 0;
//    //    }
//    //}
//    //public void OnVoteChanged(Toggle toggle)
//    //{
//    //    Debug.Log("HELLO2"+IsClient.ToString());
//    //    Debug.Log("HELLO2" + IsHost.ToString());
//    //    if (!IsHost)
//    //    {
//    //        Debug.Log("IAMCLIENT"+SteamLobbyManager.currentLobby.Owner.Id);
//    //        UpdateVoteServerRpc(toggle.isOn);
//    //        Debug.Log("hello" + IsClient.ToString());
//    //    }
//    //    else
//    //    {
//    //        Debug.Log("hello" + IsHost.ToString());
//    //        if (toggle.isOn)
//    //        {
//    //            Lobby_GeneralManager.LobbyManager_Instance.countUp[0] += 1;
//    //            Lobby_GeneralManager.LobbyManager_Instance.totalVotes += 1;
//    //        }
//    //        else
//    //        {
//    //            Lobby_GeneralManager.LobbyManager_Instance.voteCount.Value -= 1;
//    //            Lobby_GeneralManager.LobbyManager_Instance.totalVotes -= 1;
//    //        }
//    //    }
//    //}
//    public void onVoteChange(Toggle toggle)
//    {
//        if (toggle.isOn)
//        {
//            Lobby_GeneralManager.LobbyManager_Instance.map1 += 1;
//            Lobby_GeneralManager.LobbyManager_Instance.totalVotes += 1;
//            Debug.Log("hey2" + Lobby_GeneralManager.LobbyManager_Instance.map1);
//        }
//        else
//        {
//            Lobby_GeneralManager.LobbyManager_Instance.map1 -= 1;
//            Lobby_GeneralManager.LobbyManager_Instance.totalVotes -= 1;
//            Debug.Log("hey2" + Lobby_GeneralManager.LobbyManager_Instance.map1);
//        }
//    }
//    public void onVoteChange2(Toggle toggle)
//    {
//        if (toggle.isOn)
//        {
//            Lobby_GeneralManager.LobbyManager_Instance.map2 += 1;
//            Lobby_GeneralManager.LobbyManager_Instance.totalVotes += 1;
//            Debug.Log("hey" + Lobby_GeneralManager.LobbyManager_Instance.map2);
//        }
//        else
//        {
//            Lobby_GeneralManager.LobbyManager_Instance.map2 -= 1;
//            Lobby_GeneralManager.LobbyManager_Instance.totalVotes -= 1;
//            Debug.Log("hey" + Lobby_GeneralManager.LobbyManager_Instance.map2);
//        }
//    }
//}
////    [Rpc(SendTo.Server)]
////    private void UpdateVoteServerRpc(bool isOn)
////    {
////        Debug.Log("RPCCALLED");
////        if (isOn)
////        {

////            Lobby_GeneralManager.LobbyManager_Instance.voteCount.Value += 1;
////            Lobby_GeneralManager.LobbyManager_Instance.totalVotes.Value += 1;
////        }
////        else
////        {
////            Lobby_GeneralManager.LobbyManager_Instance.voteCount.Value -= 1;
////            Lobby_GeneralManager.LobbyManager_Instance.totalVotes.Value -= 1;
////        }
////    }
////}
