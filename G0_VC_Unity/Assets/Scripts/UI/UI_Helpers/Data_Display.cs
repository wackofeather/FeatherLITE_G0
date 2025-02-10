using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Steamworks;
using Steamworks.Data;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;


public class Data_Display:MonoBehaviour
{
    public GameObject MenuPrefabButton;
    public Transform MenuVector;
    public VerticalLayoutGroup VerticalLayoutGroup;
    Dictionary<ulong,GameObject>lobbyList = new Dictionary<ulong,GameObject>();
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
    private void Start()
    {
        InvokeRepeating("UpdateLobbyListUI", 0,10);
    }
    void UpdateLobbyListUI()
    {
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
            
            Button.GetComponent<Menu_Buttons_Server_Helper>().value = servers.Id;
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
        
    }
   
    void Update()
    {
        Debug.Log("hello");
       
        //Debug.Log(MenuVector.transform);




    }

}
