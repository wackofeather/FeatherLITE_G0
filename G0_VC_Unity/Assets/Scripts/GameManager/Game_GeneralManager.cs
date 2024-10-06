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
       // obj.GetComponent<PlayerStateMachine>().SetHealth(100);

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

        AddClientPlayerLookUpClientRPC(key, player, NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject, RpcTarget.NotServer);
        NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().SetHealthRPC(100, RpcTarget.Single(player.NetworkID, RpcTargetUse.Temp));
        foreach (PlayerData playerData in Player_LookUp.Values.ToList())
        {
            if (playerData.SteamID == player.SteamID) return;
            AddClientPlayerLookUpClientRPC(playerData.SteamID, playerData, NetworkManager.Singleton.ConnectedClients[playerData.NetworkID].PlayerObject, RpcTarget.Single(player.NetworkID, RpcTargetUse.Temp));
        }

        if (!NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().IsOwner)
        {
            NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().DamageCollider.SetActive(true);
            NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().DamageCollider.layer = LayerMask.NameToLayer("ENEMY");
        }
       

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


    [Rpc(SendTo.SpecifiedInParams)]
    public void AddClientPlayerLookUpClientRPC(ulong key, PlayerData player, NetworkObjectReference playerObject, RpcParams _params)
    {
        if (IsHost) return;
        playerObject.TryGet(out NetworkObject obj);
        Debug.Log(!obj.gameObject.GetComponent<PlayerStateMachine>().IsOwner);
        //obj.gameObject.GetComponent<PlayerStateMachine>().GetComponent<PlayerStateMachine>().SetHealth(100);
        if (!obj.gameObject.GetComponent<PlayerStateMachine>().IsOwner)
        {
            obj.gameObject.GetComponent<PlayerStateMachine>().DamageCollider.SetActive(true);
            obj.gameObject.GetComponent<PlayerStateMachine>().DamageCollider.layer = LayerMask.NameToLayer("ENEMY");
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

    void SpawnAtPlace(PlayerData player)
    {
        NetworkManager.SpawnManager.GetPlayerNetworkObject(player.NetworkID).gameObject.GetComponent<Rigidbody>().position = SpawnPlaces[internalSpawnTicker % SpawnPlaces.Count].position;

        internalSpawnTicker++;
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
        float timer = 10;
        while (timer > 0) 
        { 
            timer -= Time.deltaTime;
            yield return null;
        }
        player.gameObject.transform.position = new Vector3(0, 0, 0);
        player.ChangeState(player.RegularState);
        player.SetHealthRPC(100, RpcTarget.Everyone);
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
