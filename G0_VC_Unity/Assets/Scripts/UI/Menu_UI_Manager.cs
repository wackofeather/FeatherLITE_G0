using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using Steamworks;
using Steamworks.Data;
using System.Linq;


public class Menu_UI_Manager : UI_Manager
{
    [SerializeField] public Button CreateGameButton;
    [SerializeField] public Button JoinGameButton;
    [HideInInspector] public int MainKey;

    public Lobby[] serverArray;
    public List<Lobby> LobbyList = new List<Lobby>();


    //ss
    public static Menu_UI_Manager Menu_instance { get; set; }

    //instead of adding listeners, just reference function through inspector

    //declare a list of Screen_UIs, but then in runtime turn the listttttttt into a dictionary

    //use dictionary to pass in differnet key values to switch to, try and make this a parameter in a function instead of hardcoding numbers

    public async void AsyncGetServerList()
    {
            
        serverArray = await SteamMatchmaking.LobbyList.RequestAsync();
        if (serverArray != null) LobbyList = serverArray.ToList();
    }

    public override void ChildAwake()
    {
        InvokeRepeating("AsyncGetServerList", 0, 10);
        ConstructMenuSingleton();
        AsyncGetServerList();
    }







    //void Update()
    //{

    //}


    public void onClicky()
    {
        //NetworkManager.Singleton.StartHost();
        SteamLobbyManager.instance.CreateLobby();
        //SceneManager.LoadScene("LobbyScene");
       
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