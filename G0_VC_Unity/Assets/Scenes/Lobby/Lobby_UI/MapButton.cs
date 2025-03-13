using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;



public class MapButton : NetworkBehaviour
{
    public NetworkVariable<int> voteCount = new NetworkVariable<int>();
    public MapData mapData;
    public SortingGroup sortingGroup;


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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost)
        {
            voteCount.Value = 0;
        }
    }

    public void OnVoteChanged(Toggle toggle)
    {
        Debug.Log("HELLO2" + IsClient.ToString());
        Debug.Log("HELLO2" + IsHost.ToString());
        if (!IsHost)
        {
            Debug.Log("IAMCLIENT" + SteamLobbyManager.currentLobby.Owner.Id);
            UpdateVoteClientClientRpc(toggle.isOn);
            Debug.Log("hello" + IsClient.ToString());
        }
        else
        {
            Debug.Log("hello" + IsHost.ToString());
            if (toggle.isOn)
            {
                LobbyManager.LobbyManager_Instance.map1 += 1;
            }
            else
            {
                LobbyManager.LobbyManager_Instance.map1 -= 1;
            }
        }
    }
    public void OnVoteChanged2(Toggle toggle)
    {
        Debug.Log("HELLO2" + IsClient.ToString());
        Debug.Log("HELLO2" + IsHost.ToString());
        if (!IsHost)
        {
            Debug.Log("IAMCLIENT" + SteamLobbyManager.currentLobby.Owner.Id);
            UpdateVote2ClientClientRpc(toggle.isOn);
            Debug.Log("hello" + IsClient.ToString());
        }
        else
        {
            Debug.Log("hello" + IsHost.ToString());
            if (toggle.isOn)
            {
                LobbyManager.LobbyManager_Instance.map2 += 1;
            }
            else
            {
                LobbyManager.LobbyManager_Instance.map2 -= 1;
            }
        }
    }
    [Rpc(SendTo.Everyone)]
    private void UpdateVoteClientClientRpc(bool isOn)
    {
        Debug.Log("ClientRpc called");
        if (isOn)
        {
            LobbyManager.LobbyManager_Instance.UpdateVoteServer(1, 0);
        }
        else
        {
            LobbyManager.LobbyManager_Instance.UpdateVoteServer(-1, 0);
        }
    }
    [Rpc(SendTo.Everyone)]
    private void UpdateVote2ClientClientRpc(bool isOn)
    {
        Debug.Log("ClientRpc called");
        if (isOn)
        {
            LobbyManager.LobbyManager_Instance.UpdateVoteServer(0, 1);
        }
        else
        {
            LobbyManager.LobbyManager_Instance.UpdateVoteServer(0, -1);
        }
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
//    //        LobbyManager.LobbyManager_Instance.voteCount.Value = 0;
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
//    //            LobbyManager.LobbyManager_Instance.countUp[0] += 1;
//    //            LobbyManager.LobbyManager_Instance.totalVotes += 1;
//    //        }
//    //        else
//    //        {
//    //            LobbyManager.LobbyManager_Instance.voteCount.Value -= 1;
//    //            LobbyManager.LobbyManager_Instance.totalVotes -= 1;
//    //        }
//    //    }
//    //}
//    public void onVoteChange(Toggle toggle)
//    {
//        if (toggle.isOn)
//        {
//            LobbyManager.LobbyManager_Instance.map1 += 1;
//            LobbyManager.LobbyManager_Instance.totalVotes += 1;
//            Debug.Log("hey2" + LobbyManager.LobbyManager_Instance.map1);
//        }
//        else
//        {
//            LobbyManager.LobbyManager_Instance.map1 -= 1;
//            LobbyManager.LobbyManager_Instance.totalVotes -= 1;
//            Debug.Log("hey2" + LobbyManager.LobbyManager_Instance.map1);
//        }
//    }
//    public void onVoteChange2(Toggle toggle)
//    {
//        if (toggle.isOn)
//        {
//            LobbyManager.LobbyManager_Instance.map2 += 1;
//            LobbyManager.LobbyManager_Instance.totalVotes += 1;
//            Debug.Log("hey" + LobbyManager.LobbyManager_Instance.map2);
//        }
//        else
//        {
//            LobbyManager.LobbyManager_Instance.map2 -= 1;
//            LobbyManager.LobbyManager_Instance.totalVotes -= 1;
//            Debug.Log("hey" + LobbyManager.LobbyManager_Instance.map2);
//        }
//    }
//}
////    [Rpc(SendTo.Server)]
////    private void UpdateVoteServerRpc(bool isOn)
////    {
////        Debug.Log("RPCCALLED");
////        if (isOn)
////        {

////            LobbyManager.LobbyManager_Instance.voteCount.Value += 1;
////            LobbyManager.LobbyManager_Instance.totalVotes.Value += 1;
////        }
////        else
////        {
////            LobbyManager.LobbyManager_Instance.voteCount.Value -= 1;
////            LobbyManager.LobbyManager_Instance.totalVotes.Value -= 1;
////        }
////    }
////}
