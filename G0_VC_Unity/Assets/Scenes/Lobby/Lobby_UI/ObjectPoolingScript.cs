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
    //public TeamAssetLookup TeamAssetLookup;
    public List<TeamAssets> TeamAssetLookup;
    public Dictionary<GameObject,int> TeamJoinButtons = new Dictionary<GameObject,int>();
    public Dictionary<GameObject,int> PlayerThumbnails = new Dictionary<GameObject,int>();

    [HideInInspector] public List<GameObject> ActiveObjects;
    void Awake()
    {
        ConstructObjecyPoolingScriptSingleton();
        pooledObjects = new List<GameObject>();
        
        //for (int i = 0; i < amountToPool; i++)
        //{
        //    tmp = Instantiate(MenuPrefabButton, MenuVector);
        //    pooledObjects.Add(tmp);
        //}
        foreach(TeamAssets teamAssets in TeamAssetLookup)
        {
            for(int i = 0; i < amountToPool; i++)
            {
                GameObject obj = Instantiate(teamAssets.teamPlayerThumbnail, MenuVector);   
                PlayerThumbnails.Add(obj,teamAssets.teamId);
                ReturnToPool(obj);
            }
        }
        foreach (TeamAssets teamAssets in TeamAssetLookup)
        {
            if (teamAssets.teamId != -1)
            {
                GameObject obj = Instantiate(teamAssets.teamSwitchButton, MenuVector);
                TeamJoinButtons.Add(obj, teamAssets.teamId);
                ReturnToPool(obj);
            }
            
        }
    }

    public GameObject GetPooled_TeamSwitchObject(int teamId)
    {

        GameObject tryGet = TeamJoinButtons.FirstOrDefault(pair => pair.Value == teamId && pair.Key.activeSelf == false).Key;
        if (tryGet != null)
        {
            ActiveObjects.Add(tryGet);
            tryGet.SetActive(true);
            return tryGet;
        }
        else
        {
            GameObject temp;
            temp = Instantiate(TeamAssetLookup.FirstOrDefault(obj => obj.teamId == teamId).teamSwitchButton);
            ActiveObjects.Add(temp);
            TeamJoinButtons.Add(temp, teamId);
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
        GameObject tryGet = PlayerThumbnails.FirstOrDefault(pair => pair.Value == teamId && pair.Key.activeSelf == false).Key;
        if (tryGet != null)
        {
            ActiveObjects.Add(tryGet);
            tryGet.SetActive(true);
            return tryGet;
        }
        else
        {
            GameObject temp;
            temp = Instantiate(TeamAssetLookup.FirstOrDefault(obj => obj.teamId == teamId).teamPlayerThumbnail);
            ActiveObjects.Add(temp);
            PlayerThumbnails.Add(temp, teamId);
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
    public void Repool()
    {
        foreach (GameObject obj in ActiveObjects)
        {
            obj.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ButtonId = 0;
            //obj.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().tmpText.text = "";
            obj.transform.SetParent(ObjectPoolingScript.ObjectPoolingScript_Instance.MenuVector, false);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.localPosition = new Vector3(0, 0, 0);
            obj.SetActive(false);
        }
        ActiveObjects.Clear();
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().ButtonId = 0;
        //obj.GetComponentInChildren<Lobby_Player_Buttons_Helpers>().tmpText.text = "";
        obj.transform.SetParent(ObjectPoolingScript.ObjectPoolingScript_Instance.MenuVector, false);
        
        obj.transform.localScale = new Vector3(1, 1, 1);
        obj.transform.localPosition = new Vector3(0, 0, 0);

        obj.SetActive(false);

        ActiveObjects.Remove(obj);
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
