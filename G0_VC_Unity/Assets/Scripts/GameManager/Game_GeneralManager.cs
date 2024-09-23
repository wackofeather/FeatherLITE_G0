using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using Steamworks;
using Netcode.Transports.Facepunch;
using Steamworks.Data;
using UnityEngine.InputSystem;

public class Game_GeneralManager : GeneralManager
{
    new public static Game_GeneralManager instance { get; set; }

    public Dictionary<ulong, PlayerData> Player_LookUp = new Dictionary<ulong, PlayerData>(); //{ get; private set; }

    public List<Transform> SpawnPlaces = new List<Transform>();

    //ServerOnly
    private int internalSpawnTicker = 0;

    public InputActionReference escape;
    public bool paused;


    //test
    public float testFloat;

    public GameObject player;



    public void Update()
    {
        //Debug.Log(Player_LookUp.Count);
        if (escape.action.triggered)
        {
            if (paused)
            {
                Cursor.lockState = CursorLockMode.Locked;
                paused = false;
            }
            if (!paused)
            {
                Cursor.lockState = CursorLockMode.None;
                paused = true;
            }
        }
    }


    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void SpawnPlayerRPC(ulong Steam_ClientID, ulong NetworkID)
    {
        if (IsServer) { Debug.Log("goofy"); }

        //Debug.LogAssertion("balsahsadaksdsdkshdsdshdjsds");

        GameObject obj = Instantiate(player);

        //Debug.Log(NetworkManager.Singleton.IsConnectedClient);
        obj.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkID);
        obj.GetComponent<PlayerStateMachine>().SetHealth(100);

        AddPlayerServerRPC(Steam_ClientID, new PlayerData(obj.transform.position, 100, Steam_ClientID, NetworkID));
    }
    /*
        [Rpc(SendTo.Server, RequireOwnership = false)]
        public void DespawnPlayer()
        {

        }
    */




    /*    [Rpc(SendTo.Server, RequireOwnership = false)]
        public void TestSpawnServerRPC(NetworkObjectReference playerReference)
        {
            SpawnAtPlace(playerReference);
        }*/
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void AddPlayerServerRPC(ulong key, PlayerData player)
    {
        Player_LookUp.Add(key, player);

        SpawnAtPlace(player);

        AddClientPlayerLookUpClientRPC(key, player);

        Debug.Log("yipeee");

    }


    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void RemovePlayerServerRPC(ulong key, PlayerData player)
    {
        //Player_LookUp.Add(key, player);

        NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.Despawn();

        Player_LookUp.Remove(key);

        RemoveClientPlayerLookUpClientRPC(key);
        Debug.Log("bye");
    }


    [Rpc(SendTo.NotServer)]
    public void AddClientPlayerLookUpClientRPC(ulong key, PlayerData player)
    {
        if (IsHost) return;//NetworkSpawnManager.SpawnedObjects[objectId]
        Player_LookUp.Add(key, player);
    }
    [Rpc(SendTo.NotServer)]
    public void RemoveClientPlayerLookUpClientRPC(ulong key)
    {
        if (IsHost) return;

        Player_LookUp.Remove(key);
    }

    void SpawnAtPlace(PlayerData player)
    {
        NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<Rigidbody>().position = SpawnPlaces[internalSpawnTicker % SpawnPlaces.Count].position;

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
