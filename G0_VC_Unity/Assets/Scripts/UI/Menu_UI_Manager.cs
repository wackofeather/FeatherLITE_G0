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
using System.Linq;
using JetBrains.Annotations;


public class Menu_UI_Manager : UI_Manager
{
    [SerializeField] public Button CreateGameButton;
    [SerializeField] public Button JoinGameButton;
    [HideInInspector] public int MainKey;





    public static Menu_UI_Manager Menu_instance { get; set; }

    //instead of adding listeners, just reference function through inspector

    //declare a list of Screen_UIs, but then in runtime turn the listttttttt into a dictionary

    //use dictionary to pass in differnet key values to switch to, try and make this a parameter in a function instead of hardcoding numbers

    //async void Start ()
    //{
    //    using (var list = new Steamworks.ServerList.Internet())
    //    {
    //        list.AddFilter("map", "de_dust");
    //        await list.RunQueryAsync();

    //        foreach (var server in list.Responsive)
    //        {
    //            Debug.Log($"{server.Address} {server.Name}");
    //        }
    //    }
    //}


  

    //void Update()
    //{
        
    //}


    public void onClicky()
    {
        //NetworkManager.Singleton.StartHost();
        SteamLobbyManager.instance.CreateLobby();
       
    }
    //public void onClicky2()
    //{
    //    Debug.Log("mimi"); 
    //}
    public void ConstructMenuSingleton()
    {
        if (Menu_instance != null && Menu_instance != this)
        {
            Destroy(this); 
        }
        else
        {
            Menu_instance = this;
        }
    }

}