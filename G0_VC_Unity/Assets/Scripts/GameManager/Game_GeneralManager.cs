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
//using UnityEditor.PackageManager;
using static PlayerNetwork;
using UnityEngine.LowLevel;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Unity.Networking.Transport;

public class Game_GeneralManager : GeneralManager
{

    public string GameScene;

    public string PreLoadScene;
    public bool wantConnection;
    public bool reconnecting;

    new public static Game_GeneralManager instance { get; set; }

    public Dictionary<ulong, PlayerData> Player_LookUp = new Dictionary<ulong, PlayerData>();






    public Dictionary<ulong, PlayerStateMachine> PlayerGameObject_LocalLookUp = new Dictionary<ulong, PlayerStateMachine>();
    //{ get; private set; }
    //public OrderedDictionary Player_LookUp = new OrderedDictionary();

    public List<Transform> SpawnPlaces = new List<Transform>();

    //ServerOnly
    private int internalSpawnTicker = 0;

    public InputActionReference escape;
    public bool paused;


    //test
    public float testFloat;

    public GameObject networkPlayer;

    public NetworkVariable<ulong> CurrentHost = new NetworkVariable<ulong>(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<ulong> BackupHost = new NetworkVariable<ulong>(writePerm: NetworkVariableWritePermission.Server);

    public ulong currentLobbyOwner;

    public PlayerNetworkState myPlayerState;

    [HideInInspector] public GameObject runtime_playerObj;

    NetworkVariable<HostInfo> hostInfo = new NetworkVariable<HostInfo>(writePerm: NetworkVariableWritePermission.Server);


    private void Start()
    {
        wantConnection = false;
/*        NetworkManager.SceneManager.PostSynchronizationSceneUnloading = false;

        NetworkManager.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);
        Debug.Log("hahahahajajajajaj" + NetworkManager.SceneManager.ClientSynchronizationMode);*/
        //SceneManager.LoadScene(GameScene, LoadSceneMode.Additive);

    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsHost)
        {
            Debug.Log("ratatatata");
            ///if (wantConnection) return;
            NetworkManager.SceneManager.PostSynchronizationSceneUnloading = false;

            NetworkManager.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);
            Debug.Log("hahahahajajajajaj" + NetworkManager.SceneManager.ClientSynchronizationMode);
            this.NetworkObject.DestroyWithScene = false;
            this.NetworkObject.DontDestroyWithOwner = true;
        }



        NetworkManager.SceneManager.VerifySceneBeforeLoading += VerifySceneLoading;
        NetworkManager.SceneManager.VerifySceneBeforeUnloading += VerifySceneUnLoading;
        NetworkManager.OnClientDisconnectCallback += Disconnect;
        NetworkManager.OnClientDisconnectCallback += HandleHostMigration;



        

        if (!wantConnection) testFloat = UnityEngine.Random.Range(0, 1000f);

        wantConnection = true;

        //DontDestroyOnLoad(this.gameObject);
        
    }

    private bool VerifySceneUnLoading(Scene scene)
    {

        //Debug.LogWarning("unloading" + scene.name);
        if (scene.name == GameScene) return true;
        else return true;
    }

    private bool VerifySceneLoading(int sceneIndex, string sceneName, LoadSceneMode loadSceneMode)
    {
        Debug.LogWarning(sceneName);
        /*        if (sceneName == GameScene) return true;
                else return false;*/
        return true;
    }



    public struct HostInfo : INetworkSerializable
    {
        public int spawnTicker;

        public HostInfo(int _spawnTicker)
        {
            spawnTicker = _spawnTicker;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref spawnTicker);
        }
    }

    private void HandleHostMigration(ulong myNetworkID)
    {
        Debug.LogAssertion("migration");
        if (!wantConnection) { return; }
        if (IsHost) { Debug.LogAssertion("ishostshsysjsajajs"); return; }

        /*DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerNetwork>().playerStateMachine.gameObject);*/
        ///DontDestroyOnLoad(gameObject);

        //gameObject.GetComponent<NetworkObject>().Despawn();
        //Destroy(gameObject.GetComponent<NetworkObject>());
        foreach (PlayerStateMachine _player in PlayerGameObject_LocalLookUp.Values)
        {
            DontDestroyOnLoad(_player.gameObject);
        }
        /*GameObject playerObj = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().gameObject;
        DontDestroyOnLoad(playerObj);*/

        /*        GameObject playerObj = Player_LookUp[SteamClient.SteamId].PlayerObjectReference;//Playerlo.Cast<DictionaryEntry>().ElementAt(SteamClient.SteamId.Value);
                PlayerNetworkState player = playerObj.GetComponent<PlayerNetwork>().GetState();*/
        StartCoroutine(HostMigrationCoroutine(myPlayerState));
    }

    IEnumerator HostMigrationCoroutine(PlayerNetworkState player)
    {
        reconnecting = true;
        NetworkManager.Shutdown();
        while (NetworkManager.ShutdownInProgress)
        {
            Debug.Log("shutting down");
            yield return null;
        }

        Debug.LogWarning("shut down");
        //Debug.LogWarning(clientId == Player_LookUp[CurrentHost.Value].NetworkID);

        //NetworkManager.gameObject.SetActive(true);

        if (wantConnection)
        {
            //Player_LookUp.Clear();
            

            LocalDisconnect(0);

            Debug.LogWarning(SteamLobbyManager.currentLobby.Owner.Id);

            if (BackupHost.Value == SteamClient.SteamId.Value)
            {
                //NetworkManager.en
                StartCoroutine(SteamLobbyManager.instance.JoiningGameCoroutine(SteamLobbyManager.currentLobby.Owner.Id, true, true));
            }
            else StartCoroutine(SteamLobbyManager.instance.JoiningGameCoroutine(SteamLobbyManager.currentLobby.Owner.Id, false, true));
        }
        else yield break;

        Debug.Log("balsjhskshdidididididididididdi" + (NetworkManager.SpawnManager != null));

        
        //yield return null;
        yield return new WaitUntil(() => NetworkManager.SpawnManager != null);
        //yield return new WaitUntil(() => NetworkManager.Singleton.IsConnectedClient);

        //yield return new WaitUntil(() => NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject() != null);
/*        while (true)
        {
            if (NetworkManager.IsConnectedClient) { Debug.LogWarning("plduaaksss"); break; }
            Debug.LogWarning("waiting for connection" + NetworkManager.Singleton.IsApproved);
            yield return null;
        }*/

        //Debug.Log();

        while (true)
        {
            if (NetworkManager.Singleton.IsConnectedClient) break;
            yield return null;
        }

        ///SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        //gameObject.AddComponent<NetworkObject>();
        //if (IsHost) gameObject.GetComponent<NetworkObject>();

        while (true)
        {
            if (NetworkManager.SpawnManager.GetLocalPlayerObject() != null) { Debug.LogWarning("plduaaksss"); break; }
            Debug.LogWarning("ahhhhhhhhhhh" + NetworkManager.IsConnectedClient);
            yield return null;
        }



        if (IsServer) internalSpawnTicker = hostInfo.Value.spawnTicker;

        Debug.LogAssertion("ajhshsjkdhs" + player.Position);

        if (IsOwner)
        {
            GameObject playerObj = NetworkManager.SpawnManager.GetLocalPlayerObject().gameObject;
            playerObj.GetComponent<PlayerNetwork>().TransmitState(player);
        }



        foreach (PlayerStateMachine _player in PlayerGameObject_LocalLookUp.Values)
        {
            Debug.LogWarning("pluh " + _player);
            SceneManager.MoveGameObjectToScene(_player.gameObject, SceneManager.GetActiveScene());
        }

        reconnecting = false;

        yield break;
    }




    public void Update()
    {
        //Debug.Log(Player_LookUp.Count);
        if (escape.action.triggered)
        {
            if (paused)
            {

                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                //paused = false;
            }
            if (!paused)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                //paused = true;
            }
            paused = !paused;
        }
        if (IsServer)
        {
            CurrentHost.Value = SteamClient.SteamId;
            FindBackupHost();
            hostInfo.Value = new HostInfo(internalSpawnTicker);
            
            //if (Player_LookUp.Count == 2) BackupHost.Value = SteamLobbyManager.currentLobby.;
        }

        //Debug.Log("Player Local Count: " + PlayerGameObject_LocalLookUp.Count);
        //Debug.LogAssertion("Global Player Dict Count: " + Player_LookUp.Count);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        //wantConnection = false;

        NetworkManager.SceneManager.VerifySceneBeforeLoading -= VerifySceneLoading;
        NetworkManager.SceneManager.VerifySceneBeforeUnloading -= VerifySceneUnLoading;
        NetworkManager.OnClientDisconnectCallback -= Disconnect;
        NetworkManager.OnClientDisconnectCallback -= HandleHostMigration;

        //destroy playerobj
        //remove from lookups

        //SteamLobbyManager.instance.LeaveLobby();

        Debug.Log("despawing");
    }

    public void OnDestroy()
    {
        


       // wantConnection = false;
        SteamLobbyManager.instance.LeaveLobby();
    }

    void Disconnect(ulong DisconnectingNetworkID)
    {
        //if (wantConnection) return;
        //Debug.LogAssertion("blahashsksjs");
        DisconnectRPC(DisconnectingNetworkID);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void DisconnectRPC(ulong DisconnectingNetworkID)
    {
       // Debug.LogAssertion("bahsjssksksksksks  " + DisconnectingNetworkID);
        LocalDisconnect(DisconnectingNetworkID);
    }

    void LocalDisconnect(ulong DisconnectingNetworkID)
    {
        ulong myId = NetworkManager.LocalClientId;
        Debug.Log((DisconnectingNetworkID != OwnerClientId) + "    " + DisconnectingNetworkID + "    " + myId);
        if (DisconnectingNetworkID != myId | (DisconnectingNetworkID == 0 && !IsHost))
        {
            ulong steam_id = Player_LookUp.Where(d => d.Value.NetworkID == DisconnectingNetworkID).First().Value.SteamID;
            Player_LookUp.Remove(steam_id);
            PlayerGameObject_LocalLookUp[steam_id].Player_OnNetworkDespawn();
            PlayerGameObject_LocalLookUp[steam_id].Player_OnDisconnect();
            Destroy(PlayerGameObject_LocalLookUp[steam_id].gameObject);
            PlayerGameObject_LocalLookUp.Remove(steam_id);
        }
    }



    void FindBackupHost()
    {
        if (Player_LookUp.Count == 2) 
        {
            //Debug.Log(Player_LookUp.Values.ToList()[1].SteamID);
            //Debug.LogAssertion(BackupHost.Value);
            //BackupHost.Value = SteamLobbyManager.currentLobby.Members.ToList()[1].Id;
            BackupHost.Value = Player_LookUp.Values.ToList()[1].SteamID;

        }

        if (!Player_LookUp.ContainsKey(BackupHost.Value) && BackupHost.Value != 0 && Player_LookUp.Count >= 2)
        {
            BackupHost.Value = Player_LookUp.Values.ToList()[1].SteamID;
            //BackupHost.Value = SteamLobbyManager.currentLobby.Members.ToList()[1].Id;
        }

        //Debug.LogWarning(Player_LookUp.Count);
        
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
    public void SpawnPlayerRPC(ulong Steam_ClientID, ulong NetworkID, bool _reconnecting)
    {
        if (IsServer) { Debug.Log("goofy"); }

        Debug.LogWarning("balsahsadaksdsdkshdsdshdjsds");

        GameObject _networkPlayer = Instantiate(networkPlayer);
        //GameObject _playerObj = Instantiate(playerObj);
        //_networkPlayer.GetComponent<PlayerNetwork>().playerStateMachine = _playerObj.GetComponent<PlayerStateMachine>();
        //runtime_playerObj = _playerObj;
        _networkPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkID);
        Debug.LogAssertion(_networkPlayer.GetComponent<PlayerNetwork>().playerStateMachine);

        //_networkPlayer.GetComponent<PlayerNetwork>().playerStateMachine.Player_OnNetworkSpawn();
        /*GameObject obj = Instantiate(player);
        
        //Debug.Log(NetworkManager.Singleton.IsConnectedClient);
        obj.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkID);
        if (!_reconnecting) obj.GetComponent<Rigidbody>().position = new Vector3(-1000,-1000,-1000);
       // obj.GetComponent<PlayerStateMachine>().SetHealth(100);*/

        //if (_reconnecting) _networkPlayer.GetComponent<PlayerNetwork>().playerStateMachine = ;

        //AddPlayerServerRPC(Steam_ClientID, new PlayerData(_networkPlayer.GetComponent<PlayerNetwork>().playerStateMachine.transform.position, 100, Steam_ClientID, NetworkID, null), _reconnecting);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public virtual void Server_SpawnPlayerForGameRPC(NetworkObjectReference playerNetworkObject, ulong NetworkID)
    {
        Owner_SpawnPlayerForGameRPC(playerNetworkObject, internalSpawnTicker, RpcTarget.Single(NetworkID, RpcTargetUse.Temp));

        internalSpawnTicker++;
/*        ((GameObject)playerNetworkObject).GetComponent<PlayerNetwork>().playerStateMachine.rb.MovePosition(new Vector3(-1000, -1000, -1000));
        ((GameObject)playerNetworkObject).GetComponent<PlayerNetwork>().SetHealthRPC(100, RpcTarget.Owner);*/
    }

    [Rpc(SendTo.SpecifiedInParams, RequireOwnership = false)]
    public virtual void Owner_SpawnPlayerForGameRPC(NetworkObjectReference playerNetworkObject,  int SpawnTicker, RpcParams _param)
    {
        playerNetworkObject.TryGet(out NetworkObject playerObj);

        playerObj.gameObject.GetComponent<PlayerNetwork>().playerStateMachine.gameObject.transform.position = SpawnPlaces[SpawnTicker % SpawnPlaces.Count].position;//.rb.MovePosition(SpawnPlaces[internalSpawnTicker % SpawnPlaces.Count].position);
        playerObj.gameObject.GetComponent<PlayerNetwork>().SetHealthRPC(100, RpcTarget.Owner);
        //Debug.LogWarning(SpawnPlaces[internalSpawnTicker % SpawnPlaces.Count].position);

    }

    public void InitPlayer(bool _isOwner, PlayerNetwork playerNetworkObject)
    {
        if (_isOwner)
        {
            return;
        }
        
        playerNetworkObject.playerStateMachine.gameObject.layer = LayerMask.NameToLayer("ENEMY");


        //playerObj.gameObject.GetComponent<PlayerNetwork>().playerStateMachine.gameObject.layer = LayerMask.NameToLayer("ENEMY");
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
    public void AddPlayerServerRPC(ulong key, PlayerData player, bool _reconnecting)
    {
        Player_LookUp.Add(key, player);

        if (Player_LookUp.Count == 2) BackupHost.Value = player.SteamID;

        AddClientPlayerLookUpClientRPC(key, player, NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject, RpcTarget.NotServer);
        NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerNetwork>().SetHealthRPC(100, RpcTarget.Owner); //RpcTarget.Single(player.NetworkID, RpcTargetUse.Temp));
        foreach (PlayerData playerData in Player_LookUp.Values)
        {
            if (playerData.SteamID != player.SteamID)
            {
                AddClientPlayerLookUpClientRPC(playerData.SteamID, playerData, NetworkManager.Singleton.ConnectedClients[playerData.NetworkID].PlayerObject, RpcTarget.Single(player.NetworkID, RpcTargetUse.Temp));
            }
            
            //NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().SetHealth(-1000);
        }

        if (!NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerNetwork>().IsOwner)
        {
            /*            NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().DamageCollider.SetActive(true);
                        NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().DamageCollider.layer = LayerMask.NameToLayer("ENEMY");*/
            NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.gameObject.layer = LayerMask.NameToLayer("ENEMY");

        }
       

        //Debug.Log("yipeee   " + NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerStateMachine>().networkInfo._isOwner);

        
        if (!_reconnecting) SpawnAtPlaceRPC(player, SpawnPlaces[internalSpawnTicker % SpawnPlaces.Count].position, RpcTarget.Single(player.NetworkID, RpcTargetUse.Temp));
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
        //NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerNetwork>().playerStateMachine = player.GetComponent<PlayerStateMachine>();

        Destroy(NetworkManager.Singleton.ConnectedClients[player.NetworkID].PlayerObject.GetComponent<PlayerNetwork>().playerStateMachine.gameObject);
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
        Debug.Log(!obj.gameObject.GetComponent<PlayerStateMachine>().networkInfo._isOwner);
        //obj.gameObject.GetComponent<PlayerStateMachine>().GetComponent<PlayerStateMachine>().SetHealth(100);
        if (!obj.gameObject.GetComponent<PlayerStateMachine>().networkInfo._isOwner)
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

        NetworkManager.SpawnManager.GetLocalPlayerObject().gameObject.GetComponent<PlayerNetwork>().playerStateMachine.gameObject.GetComponent<Rigidbody>().MovePosition(_position);
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
        //player.gameObject.transform.position = new Vector3(0, 0, 0);
        Server_SpawnPlayerForGameRPC(player.playerNetwork.GetComponent<NetworkObject>(), player.playerNetwork.OwnerClientId);
        player.ChangeState(player.RegularState);
        player.playerNetwork.SetHealthRPC(100, RpcTarget.Owner);
        yield break;
        //
    }

    private void OnApplicationQuit()
    {
        wantConnection = false;
    }



    public override void ConstructSingleton()
    {
        Debug.Log("Instance is" + instance);
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
}
