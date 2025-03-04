using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Steamworks;
using Steamworks.Data;
using TMPro;
using Unity.VisualScripting;
//using UnityEditor.ShaderGraph.Intaernal;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


public class Data_Display:MonoBehaviour
{
    public GameObject MenuPrefabButton;
    public Transform MenuVector;
    public VerticalLayoutGroup VerticalLayoutGroup;
    public TMP_InputField LobbyInput;
    public UnityEngine.UI.Button joinLobby;
    Dictionary<ulong,GameObject>lobbyList = new Dictionary<ulong,GameObject>();
    //List<int>lobbyList2 = new List<int>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public void Data_display_calculator(object x)
    //{
    //    Debug.Log(x);
    //}

    //GUI Image, text Script, lobby entry ui: image text button function says initializing image text 
    //Screen_UI Lobby List Manager 

    //public void clicky2()
    //{
    //    //SteamLobbyManager.instance.JoinLobbyAsync(Convert.ToUInt64(x));
    //    Debug.Log("hehe");
    //}
    //void Calculation()
    //{
    //    lobbyList2.Add(1);
    //}
    private void Start()
    {

/*        InvokeRepeating("Calculation", 0, 5);*/
        
        InvokeRepeating("UpdateLobbyListUI", 0, 10);
    }
    void UpdateLobbyListUI()
    {
        //foreach(GameObject lobbyButton in lobbyList.Values)
        //{
        //    if (Menu_UI_Manager.Menu_instance.serverArray.Exists(lobbyButton.GetComponent<Menu_Buttons_Server_Helper>().lobby))
        //    {

        //    }
        //}
        foreach (Lobby servers in Menu_UI_Manager.Menu_instance.serverArray)
        {
     
            GameObject Button;
            if (lobbyList.ContainsKey(servers.Id))
            {
                Button = lobbyList[servers.Id];
            }
            else
            {
                Button = Instantiate(MenuPrefabButton, MenuVector.transform);
                lobbyList.Add(servers.Id, Button);

            }

            Button.GetComponent<Menu_Buttons_Server_Helper>().lobby = servers;
            Button.GetComponent<Menu_Buttons_Server_Helper>().ConstructButton(servers);
            VerticalLayoutGroup.CalculateLayoutInputVertical();
            //Debug.Log(servers);

        }
        //for (int i = 0; i < 10; i++)
        //{
        //    GameObject newButton = Instantiate(MenuPrefabButton, MenuVector.transform);
        //    VerticalLayoutGroup.CalculateLayoutInputVertical();
        //    //newButton.GetComponent<Menu_Buttons_Server_Helper>().value = servers.Id;

        //    //Debug.Log(servers);
        //}

        //foreach (object servers in lobbyList2)
        //{
        //    GameObject Button;
        //    //if (lobbyList.ContainsKey(servers.Id))
        //    //{
        //    //    Button = lobbyList[servers.Id];
        //    //}
        //    //else
        //    //{
        //    Button = Instantiate(MenuPrefabButton, MenuVector.transform);
        //    //    lobbyList.Add(servers.Id, Button);

        //    //}

        //    //Button.GetComponent<Menu_Buttons_Server_Helper>().value = servers.Id;
        //    //Button.GetComponent<Menu_Buttons_Server_Helper>().ConstructButton(servers);
        //    VerticalLayoutGroup.CalculateLayoutInputVertical();
        //    //Debug.Log(servers);

        //}
    }
    public void LobbyJoiner()
    {
        
        Debug.Log(LobbyInput.text);
        //SteamLobbyManager.instance.JoinLobbyAsync(Convert.ToUInt64(LobbyId));
    }
    //void Update()
    //{
    //    Debug.Log("hello");
   
    //    //Debug.Log(MenuVector.transform);




    //}

}
