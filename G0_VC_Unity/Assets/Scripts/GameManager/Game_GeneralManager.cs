using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using Steamworks;
using Netcode.Transports.Facepunch;
using Steamworks.Data;
using UnityEngine.InputSystem;
using System.Threading;
using System.Linq;
using System.Collections.Specialized;
using System;

public class Game_GeneralManager : GeneralManager
{
    new public static Game_GeneralManager instance { get; set; }

    //public Dictionary<ulong, PlayerData> Player_LookUp = new Dictionary<ulong, PlayerData>(); //{ get; private set; }
    public OrderedDictionary Player_LookUp = new OrderedDictionary();

    public List<Transform> SpawnPlaces = new List<Transform>();

    //ServerOnly
    private int internalSpawnTicker = 0;

    public InputActionReference escape;
    public bool paused;


    //test
    public float testFloat;

    public GameObject player;

    NetworkVariable<ulong> CurrentHost = new NetworkVariable<ulong>(writePerm: NetworkVariableWritePermission.Server);
    NetworkVariable<ulong> BackupHost = new NetworkVariable<ulong>(writePerm: NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        //NetworkManager.OnClientDisconnectCallback += HandleHostMigration;
    }

    private void HandleHostMigration(ulong clientId)
    {
        if (Player_LookUp[clientId] == CurrentHost)
        {
            NetworkManager.DisconnectClient(clientId);
            NetworkManager.Shutdown();
            if (BackupHost.Value == SteamClient.SteamId.Value)
            {
                //NetworkManager.en
                NetworkManager.StartHost();
            }
            else StartCoroutine(SteamLobbyManager.instance.JoiningGameCoroutine(BackupHost.Value, false));
        }
    }



    public void Update()
    {
        //Debug.Log(Player_LookUp.Count);
        if (escape.action.triggered)
        {
            if (paused)
            {

                Cursor.lockState = CursorLockMode.Locked;
                //paused = false;
            }
            if (!paused)
            {
                Cursor.lockState = CursorLockMode.None;
                //paused = true;
            }
            paused = !paused;
        }
        if (IsServer)
        {
            CurrentHost.Value = SteamClient.SteamId;
            if (Player_LookUp.Count > 2) BackupHost.Value = (ulong)Player_LookUp[1];
        }
    }

/*    [Rpc(SendTo.Server)]
    public List<PlayerData> GetPlayerLookUpRPC()
    {
        return Player_LookUp.Values.ToList();
    }
    [Rpc(SendTo.Server)]
    public NetworkObjectReference GetPlayerObjectReferenceRPC(ulong playerId)
    {
        return NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject;
    }
    [Rpc(SendTo.SpecifiedInParams)]
    int GetPlayerHealthRPC(RpcParams _params)
    {
        return NetworkManager.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerStateMachine>().health;
    }*/

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void SpawnPlayerRPC(ulong Steam_ClientID, ulong NetworkID)
    {
        if (IsServer) { Debug.Log("goofy"); }

        //Debug.LogAssertion("balsahsadaksdsdkshdsdshdjsds");

        GameObject obj = Instantiate(player);

        //Debug.Log(NetworkManager.Singleton.IsConnectedClient);
        obj.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkID);
        obj.GetComponent<Rigidbody>().position = new Vector3(-1000,-1000,-1000);
       // obj.GetComponent<PlayerStateMachine>().SetHealth(100);

        AddPlayerServerRPC(Steam_ClientID, new PlayerData(obj.transform.position, 100, Steam_ClientID, NetworkID, obj.GetComponent<NetworkObject>()));
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

        

        AddClientPlayerLookUpClientRPC(key, player, NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject, RpcTarget.NotServer);
        NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().SetHealthRPC(100, RpcTarget.Owner); //RpcTarget.Single(player.NetworkID, RpcTargetUse.Temp));
        foreach (PlayerData playerData in Player_LookUp.Values)
        {
            if (playerData.SteamID != player.SteamID)
            {
                AddClientPlayerLookUpClientRPC(playerData.SteamID, playerData, NetworkManager.Singleton.ConnectedClients[playerData.NetworkID].PlayerObject, RpcTarget.Single(player.NetworkID, RpcTargetUse.Temp));
            }
            
            //NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().SetHealth(-1000);
        }

        if (!NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().IsOwner)
        {
            /*            NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().DamageCollider.SetActive(true);
                        NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().DamageCollider.layer = LayerMask.NameToLayer("ENEMY");*/
            NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.gameObject.layer = LayerMask.NameToLayer("ENEMY");

        }
       

        Debug.Log("yipeee   " + NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().IsOwner);

        
        SpawnAtPlaceRPC(player, SpawnPlaces[internalSpawnTicker % SpawnPlaces.Count].position, RpcTarget.Single(player.NetworkID, RpcTargetUse.Temp));
        internalSpawnTicker++;

    }

   /* public IEnumerator AddPlayerServerRPC_Coroutine(ulong key, PlayerData player)
    {
        Player_LookUp.Add(key, player);

        SpawnAtPlace(player);

        AddClientPlayerLookUpClientRPC(key, player, NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject, RpcTarget.NotServer);
        NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().SetHealthRPC(100, RpcTarget.Single(player.NetworkID, RpcTargetUse.Temp));
        foreach (PlayerData playerData in Player_LookUp.Values.ToList())
        {
            if (playerData.SteamID != player.SteamID)
            {
                AddClientPlayerLookUpClientRPC(playerData.SteamID, playerData, NetworkManager.Singleton.ConnectedClients[playerData.NetworkID].PlayerObject, RpcTarget.Single(player.NetworkID, RpcTargetUse.Temp));
                //NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().SetHealth(-1000);
            }
           
        }

        if (!NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().IsOwner)
        {
            NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().DamageCollider.SetActive(true);
            NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().DamageCollider.layer = LayerMask.NameToLayer("ENEMY");
        }

        yield break;
    }*/


    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void RemovePlayerServerRPC(ulong key, PlayerData player)
    {
        //Player_LookUp.Add(key, player);

        NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.Despawn();

        Player_LookUp.Remove(key);

        RemoveClientPlayerLookUpClientRPC(key);
        Debug.Log("bye");
    }


    [Rpc(SendTo.SpecifiedInParams)]
    public void AddClientPlayerLookUpClientRPC(ulong key, PlayerData player, NetworkObjectReference playerObject, RpcParams _params)
    {
        if (IsHost) return;
        playerObject.TryGet(out NetworkObject obj);
        Debug.Log(!obj.gameObject.GetComponent<PlayerStateMachine>().IsOwner);
        //obj.gameObject.GetComponent<PlayerStateMachine>().GetComponent<PlayerStateMachine>().SetHealth(100);
        if (!obj.gameObject.GetComponent<PlayerStateMachine>().IsOwner)
        {
            /*obj.gameObject.GetComponent<PlayerStateMachine>().DamageCollider.SetActive(true);
            obj.gameObject.GetComponent<PlayerStateMachine>().DamageCollider.layer = LayerMask.NameToLayer("ENEMY");*/
            obj.gameObject.layer = LayerMask.NameToLayer("ENEMY");
        }
/*        if (obj.gameObject.GetComponent<PlayerStateMachine>().IsOwner)
        {
            obj.gameObject.GetComponent<PlayerStateMachine>().SetHealth
        }*/
        Player_LookUp.Add(key, player);
    }
    [Rpc(SendTo.NotServer)]
    public void RemoveClientPlayerLookUpClientRPC(ulong key)
    {

        if (IsHost) return;

        Player_LookUp.Remove(key);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void SpawnAtPlaceRPC(PlayerData player, Vector3 _position, RpcParams _params)
    {
        //NetworkManager.SpawnManager.GetPlayerNetworkObject(player.NetworkID).gameObject.GetComponent<Rigidbody>().position = _position;

        NetworkManager.SpawnManager.GetLocalPlayerObject().gameObject.GetComponent<Rigidbody>().MovePosition(_position);
        Debug.Log("hahskjahshssjdhsjdshdhjshdsjdhsdhshdshd");
        //internalSpawnTicker++;
    }


    //Death Management

    public void Kill(PlayerStateMachine player)
    {
        StartCoroutine(KillCoroutine(player));
    }

    IEnumerator KillCoroutine(PlayerStateMachine player)
    {
        player.ChangeState(player.DeathState);
        player.gameObject.transform.position = new Vector3(0, 0, -10000);
        float timer = 3;
        while (timer > 0) 
        { 
            timer -= Time.deltaTime;
            yield return null;
        }
        player.gameObject.transform.position = new Vector3(0, 0, 0);
        player.ChangeState(player.RegularState);
        player.SetHealthRPC(100, RpcTarget.Owner);
        yield break;
        //
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
