using System.Collections.Generic;
using System.Linq;
using Steamworks;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ObjectPoolingScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject MenuPrefabButton;
    public List<GameObject> pooledObjects;
    public Transform MenuVector;
    public int amountToPool;
    //public Slider TeamNumberSlider;
    public static ObjectPoolingScript ObjectPoolingScript_Instance { get; set; }
    public List<TeamAssets> TeamAssetsList = new List<TeamAssets>();
    public Dictionary<GameObject,int> TeamJoinButtons = new Dictionary<GameObject,int>();
    public Dictionary<GameObject,int> PlayerThumbnails = new Dictionary<GameObject,int>();

    public List<GameObject> ActiveObjects;
    void Awake()
    {
        ConstructObjecyPoolingScriptSingleton();
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        //for (int i = 0; i < amountToPool; i++)
        //{
        //    tmp = Instantiate(MenuPrefabButton, MenuVector);
        //    pooledObjects.Add(tmp);
        //}
        foreach(TeamAssets teamAssets in TeamAssetsList)
        {
            for(int i = 0; i < 10; i++)
            {
                GameObject obj = Instantiate(teamAssets.teamPlayerThumbnail, MenuVector);   
                PlayerThumbnails.Add(obj,teamAssets.teamId);
            }
        }
        foreach (TeamAssets teamAssets in TeamAssetsList)
        {
            
            GameObject obj = Instantiate(teamAssets.teamSwitchButton, MenuVector);
            TeamJoinButtons.Add(obj, teamAssets.teamId);
        }
    }

    public GameObject GetPooled_TeamSwitchObject(int teamId)
    {
        
        
        if(TeamJoinButtons.FirstOrDefault(pair => pair.Value == teamId&&pair.Key.activeSelf ==false).Key!=null)
        {
            return TeamJoinButtons.FirstOrDefault(pair => pair.Value == teamId).Key;
        }
        else
        {
            GameObject temp;
            temp = Instantiate(TeamAssetsList.FirstOrDefault(obj => obj.teamId == teamId).teamSwitchButton);
            return temp;
        }
        //temp = pooledObjects[0];
        //ActiveObjects.Add(pooledObjects[0]);
        //pooledObjects.RemoveAt(0);
        

        // Debug.Log("walala"+pooledObjects.Count);
        //if(ActiveObjects.Count>0)
        // {
        //     foreach (GameObject btn in ActiveObjects)
        //     {
        //         if (btn.GetComponent<Lobby_Player_Buttons_Helpers>().ButtonId == friend)
        //         {
        //             return;
        //         }
        //         else
        //         {
        //             ActiveObjects.Add(pooledObjects[0]);
        //             btn.transform.SetParent(x);
        //             pooledObjects.RemoveAt(0);
        //         }
        //     }
        // }
        // else
        // {
        //     ActiveObjects.Add(pooledObjects[0]);
        //     pooledObjects[0].transform.SetParent(x);
        //     pooledObjects.RemoveAt(0);

        // }
        //     if (ActiveObjects.Count > Lobby_GeneralManager.LobbyManager_Instance.memberList.Count)
        // {
        //     foreach(Friend friends in Lobby_GeneralManager.LobbyManager_Instance.memberList)
        //     {
        //         foreach(GameObject obj in ActiveObjects)
        //         {
        //             if (obj.GetComponent<Lobby_Player_Buttons_Helpers>().ButtonId == friends.Id)
        //             {
        //                 pooledObjects.Add(obj);
        //                 obj.transform.SetParent(MenuVector);
        //                 ActiveObjects.Remove(obj);
        //             }
        //         }
        //     }
        // }

    }
    public GameObject GetPooled_PlayerThumbnailObject(int teamId)
    {
        if (PlayerThumbnails.FirstOrDefault(pair => pair.Value == teamId && pair.Key.activeSelf == false).Key != null)
        {
            return PlayerThumbnails.FirstOrDefault(pair => pair.Value == teamId).Key;
        }
        else
        {
            GameObject temp;
            temp = Instantiate(TeamAssetsList.FirstOrDefault(obj => obj.teamId == teamId).teamPlayerThumbnail);
            return temp;
        }

        // Debug.Log("walala"+pooledObjects.Count);
        //if(ActiveObjects.Count>0)
        // {
        //     foreach (GameObject btn in ActiveObjects)
        //     {
        //         if (btn.GetComponent<Lobby_Player_Buttons_Helpers>().ButtonId == friend)
        //         {
        //             return;
        //         }
        //         else
        //         {
        //             ActiveObjects.Add(pooledObjects[0]);
        //             btn.transform.SetParent(x);
        //             pooledObjects.RemoveAt(0);
        //         }
        //     }
        // }
        // else
        // {
        //     ActiveObjects.Add(pooledObjects[0]);
        //     pooledObjects[0].transform.SetParent(x);
        //     pooledObjects.RemoveAt(0);

        // }
        //     if (ActiveObjects.Count > Lobby_GeneralManager.LobbyManager_Instance.memberList.Count)
        // {
        //     foreach(Friend friends in Lobby_GeneralManager.LobbyManager_Instance.memberList)
        //     {
        //         foreach(GameObject obj in ActiveObjects)
        //         {
        //             if (obj.GetComponent<Lobby_Player_Buttons_Helpers>().ButtonId == friends.Id)
        //             {
        //                 pooledObjects.Add(obj);
        //                 obj.transform.SetParent(MenuVector);
        //                 ActiveObjects.Remove(obj);
        //             }
        //         }
        //     }
        // }

    }

  
        // Update is called once per frame


    public void ConstructObjecyPoolingScriptSingleton()
    {
        if (ObjectPoolingScript_Instance != null && ObjectPoolingScript_Instance != this)
        {
            Destroy(this);
        }
        else
        {
            ObjectPoolingScript_Instance = this;
        }
    }
}
