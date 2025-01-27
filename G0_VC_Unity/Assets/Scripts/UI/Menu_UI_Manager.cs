using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Steamworks;
using Netcode.Transports.Facepunch;
using System.Threading.Tasks;
using Steamworks.Data;
using System;
using UnityEngine.SceneManagement;


public class Menu_UI_Manager:UI_Manager
{
    [SerializeField] public Button CreateGameButton;
    [SerializeField] public NetworkManager NetworkManager;

    new private static Menu_UI_Manager instance { get; set; }

    void Start()
    {
        NetworkManager = GetComponent<NetworkManager>();

        CreateGameButton.onClick.AddListener(onClicky);

    }

    void onClicky()
    {
        NetworkManager.Singleton.StartHost();

    }
    public override void ConstructSingleton()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

}