using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class Game_GeneralManager : GeneralManager
{
    new public static Game_GeneralManager instance { get; set; }

    public Dictionary<float, NetworkObject> Player_LookUp { get; private set; }

    public List<Transform> SpawnPlaces;

    //ServerOnly
    private int internalSpawnTicker = 0;


    //test
    public float testFloat;

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void TestSpawnServerRPC(NetworkObjectReference playerReference)
    {
        SpawnAtPlace(playerReference);
    }
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void AddPlayerServerRPC(float key, NetworkObjectReference player)
    {
        //Player_LookUp.Add(key, player);

        SpawnAtPlace(player);

        //AddClientPlayerLookUpClientRPC(key, player);

        Debug.Log("yipeee");
    }


    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void RemovePlayerServerRPC(float key, NetworkObjectReference player)
    {
        //Player_LookUp.Add(key, player);

        //Player_LookUp.Remove(key);

        //RemoveClientPlayerLookUpClientRPC(key);
        Debug.Log("bye");
    }


    [Rpc(SendTo.NotServer)]
    public void AddClientPlayerLookUpClientRPC(float key, NetworkObjectReference player)
    {
        if (IsHost) return;//NetworkSpawnManager.SpawnedObjects[objectId]
        Player_LookUp.Add(key, NetworkManager.SpawnManager.SpawnedObjects[player.NetworkObjectId]);
    }
    [Rpc(SendTo.NotServer)]
    public void RemoveClientPlayerLookUpClientRPC(float key)
    {
        if (IsHost) return;

        Player_LookUp.Remove(key);
    }

    void SpawnAtPlace(NetworkObjectReference player)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects[player.NetworkObjectId].gameObject.GetComponent<Rigidbody>().position = SpawnPlaces[internalSpawnTicker % SpawnPlaces.Count].position;

        internalSpawnTicker++;
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
