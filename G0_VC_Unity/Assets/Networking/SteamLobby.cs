using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamLobby : MonoBehaviour
{

    [SerializeField] private GameObject buttons = null;

    public void HostLobby()
    {
        buttons.SetActive(false);


    }
}
