using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHelper : MonoBehaviour
{


    public void StartAHost()
    {
        SteamLobbyManager.instance.CreateLobby();
    }
}
