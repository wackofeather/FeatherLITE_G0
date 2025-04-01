using System.Collections.Generic;
using Steamworks;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class ObjectPoolingScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject MenuPrefabButton;
    public List<GameObject> pooledObjects;
    public Transform MenuVector;
    public int amountToPool;
    public static ObjectPoolingScript ObjectPoolingScript_Instance { get; set; }
    void Start()
    {
        ConstructObjecyPoolingScriptSingleton();
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(MenuPrefabButton,MenuVector);
            pooledObjects.Add(tmp);
        }
    }

    public GameObject GetPooledObject()
    {
        GameObject temp;
        temp = pooledObjects[0];
        //ActiveObjects.Add(pooledObjects[0]);
        pooledObjects.RemoveAt(0);
        return temp;
        
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
