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
using System.Threading.Tasks;
using UnityEngine.Rendering.Universal;

public class Game_GeneralManager : GeneralManager
{

    public string GameScene;

    public string PreLoadScene;
    
    
    
    public static Game_GeneralManager game_instance { get; set; }

    public Dictionary<ulong, PlayerData> Player_LookUp = new Dictionary<ulong, PlayerData>();






    public Dictionary<ulong, PlayerStateMachine> PlayerGameObject_LocalLookUp = new Dictionary<ulong, PlayerStateMachine>();
    //{ get; private set; }
    //public OrderedDictionary Player_LookUp = new OrderedDictionary();

    //public List<Transform> SpawnPlaces = new List<Transform>();

    [HideInInspector] public Runtime_MapData currentMap;

    //ServerOnly
    public int internalSpawnTicker = 0;

    public InputActionReference escape;
    public bool paused;


    //test
    

    public GameObject networkPlayer;


    //public NetworkList<ulong> HostBackups = new NetworkList<ulong>(new List<ulong>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    

    public PlayerNetworkState myPlayerState;

    [HideInInspector] public GameObject runtime_playerObj;

    

    NetworkVariable<GameHostInfo> hostInfo = new NetworkVariable<GameHostInfo>(writePerm: NetworkVariableWritePermission.Server);



    GameObject Env_Container;

    [HideInInspector] public BaseGameMode gameMode;
    public override void _Awake()
    {
        base._Awake();

        Game_ConstructSingleton();

        
    }

    public override void _Start()
    {
        base._Start();

        LoadGameScene();
    }

    public async void LoadGameScene()
    {
        await SceneManager.LoadSceneAsync(mapLookup.GetMapLookUp()[SteamLobbyManager.currentLobby.GetData("Map")], LoadSceneMode.Additive);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(mapLookup.GetMapLookUp()[SteamLobbyManager.currentLobby.GetData("Map")]));
        currentMap = GameObject.FindFirstObjectByType<Runtime_MapData>();

        Env_Container = currentMap.gameObject;
    }

    public override void _OnNetworkSpawn()
    {
        base._OnNetworkSpawn();

        if (IsHost)
        {

            Env_Container.GetComponent<NetworkObject>().DestroyWithScene = false;
            Env_Container.GetComponent<NetworkObject>().DontDestroyWithOwner = true;
        }
        string GameMode = SteamLobbyManager.currentLobby.GetData("GameMode");
        if (GameMode == "FFA")
        {
            gameMode = new FFA_gameMode();
        }

        //DontDestroyOnLoad(this.gameObject);

    }

    public override void OnLobbyDataChange(Lobby newLobbyData)
    {
        Debug.LogAssertion("jeez louise  " + newLobbyData.Owner.Id + "   " + currentLobbyOwner);
        //if (!wantConnection) return;
        if (currentLobbyOwner != newLobbyData.Owner.Id)
        {
            Debug.LogAssertion("loooooooooooooooooooooook");
            if (HostMigrationCorT != null) StopCoroutine(HostMigrationCorT);
            if (JoiningGameCorT != null) StopCoroutine(JoiningGameCorT);

            /*            if (mapLookup.GetMapLookUp().ContainsValue(SceneManager.GetActiveScene().name))
                        {*/
            foreach (PlayerStateMachine _player in PlayerGameObject_LocalLookUp.Values)
            {
                DontDestroyOnLoad(_player.gameObject);
            }
            //}


            HostMigrationCorT = StartCoroutine(HostMigrationCoroutine());
        }
    }





    public struct GameHostInfo : INetworkSerializable
    {
        public int spawnTicker;

        public GameHostInfo(int _spawnTicker)
        {
            spawnTicker = _spawnTicker;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref spawnTicker);
        }
    }

    /*    private async void TryReconnection(ulong myNetworkID)
        {
            Debug.LogAssertion("migration");
            if (!wantConnection) { SteamLobbyManager.instance.RevertToMenu(); return; }
            if (IsHost) { SteamLobbyManager.instance.RevertToMenu(); return; }

            *//*DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerNetwork>().playerStateMachine.gameObject);*//*
            ///DontDestroyOnLoad(gameObject);

            //gameObject.GetComponent<NetworkObject>().Despawn();
            //Destroy(gameObject.GetComponent<NetworkObject>());

            Task<bool> tryReconnectingtoCurrent = SteamLobbyManager.instance.TryConnect(false);

            while (!tryReconnectingtoCurrent.IsCompleted)
            {
                await Task.Yield();
            }
            if (!tryReconnectingtoCurrent.IsCompletedSuccessfully) { SteamLobbyManager.instance.RevertToMenu(); return; }
            if (tryReconnectingtoCurrent.Result == true) return;


            foreach (PlayerStateMachine _player in PlayerGameObject_LocalLookUp.Values)
            {
                DontDestroyOnLoad(_player.gameObject);
            }
            *//*GameObject playerObj = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().gameObject;
            DontDestroyOnLoad(playerObj);*/

    /*        GameObject playerObj = Player_LookUp[SteamClient.SteamId].PlayerObjectReference;//Playerlo.Cast<DictionaryEntry>().ElementAt(SteamClient.SteamId.Value);
            PlayerNetworkState player = playerObj.GetComponent<PlayerNetwork>().GetState();*//*
    StartCoroutine(HostMigrationCoroutine(myPlayerState));
}*/


    public override void PrepareHM()
    {
        base.PrepareHM();
        Debug.LogAssertion("kookookaka " + currentLobbyOwner + "  " + Player_LookUp[currentLobbyOwner].NetworkID);
        LocalDisconnect(Player_LookUp[currentLobbyOwner].NetworkID);
    }

    public async override void ExitHM()
    {
        while (true)
        {
            if (NetworkManager.SpawnManager.GetLocalPlayerObject() != null) { Debug.LogWarning("plduaaksss"); break; }
            Debug.LogWarning("ahhhhhhhhhhh" + NetworkManager.IsConnectedClient);
            await Task.Yield();
        }



        if (IsServer) internalSpawnTicker = hostInfo.Value.spawnTicker;

        Debug.LogAssertion("ajhshsjkdhs" + myPlayerState.Position + "  " + (NetworkManager.SpawnManager.GetLocalPlayerObject() != null));

        if (IsOwner)
        {
            GameObject playerObj = NetworkManager.SpawnManager.GetLocalPlayerObject().gameObject;
            playerObj.GetComponent<PlayerNetwork>().TransmitState(myPlayerState);
        }



        foreach (PlayerStateMachine _player in PlayerGameObject_LocalLookUp.Values)
        {
            Debug.LogWarning("pluh " + _player);
            SceneManager.MoveGameObjectToScene(_player.gameObject, SceneManager.GetActiveScene());
        }

        SteamLobbyManager.instance.reconnecting = false;
    }

    public override void _Update()
    {
        base._Update();
        //Debug.LogAssertion(IsHost);
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

            hostInfo.Value = new GameHostInfo(internalSpawnTicker);
            
            //if (Player_LookUp.Count == 2) BackupHost.Value = SteamLobbyManager.currentLobby.;
        }

        //Debug.Log("Player Local Count: " + PlayerGameObject_LocalLookUp.Count);
        //Debug.LogAssertion("Global Player Dict Count: " + Player_LookUp.Count);
    }







    [Rpc(SendTo.ClientsAndHost)]
    public override void DisconnectRPC(ulong DisconnectingNetworkID)
    {
        Debug.LogAssertion("bahsjssksksksksks  " + DisconnectingNetworkID);
        LocalDisconnect(DisconnectingNetworkID);
    }

    public override void LocalDisconnect(ulong DisconnectingNetworkID)
    {
        ulong myId = (ulong)NetworkID;
        Debug.LogAssertion("my ID " + DisconnectingNetworkID);
        Debug.LogWarning((DisconnectingNetworkID != OwnerClientId) + "    " + DisconnectingNetworkID + "    " + myId + "  " + Player_LookUp.Count + "  " + IsHost);
        if (DisconnectingNetworkID != myId | (DisconnectingNetworkID == 0 && myId != 0))
        {
            ulong steam_id = 0;
            try { steam_id = Player_LookUp.Where(d => d.Value.NetworkID == DisconnectingNetworkID).First().Value.SteamID; }
            catch { return; }
            Player_LookUp.Remove(steam_id);
            PlayerGameObject_LocalLookUp[steam_id].Player_OnNetworkDespawn();
            PlayerGameObject_LocalLookUp[steam_id].Player_OnDisconnect();
            Destroy(PlayerGameObject_LocalLookUp[steam_id].gameObject);
            PlayerGameObject_LocalLookUp.Remove(steam_id);
        }
    }

    public override void OnConnectedToSession(bool _reconnecting)
    {
        base.OnConnectedToSession(_reconnecting);
        SpawnPlayerRPC(Steamworks.SteamClient.SteamId, NetworkManager.Singleton.LocalClientId, _reconnecting);
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
        gameMode.ServerSideRespawnplayer(playerNetworkObject, NetworkID);
    }

    [Rpc(SendTo.SpecifiedInParams, RequireOwnership = false)]
    public virtual void Owner_SpawnPlayerForGameRPC(NetworkObjectReference playerNetworkObject,  int SpawnTicker, RpcParams _param)
    {
        gameMode.OwnerSideRespawnPlayer(playerNetworkObject, SpawnTicker, _param);

    }

    public void InitPlayer(bool _isOwner, PlayerNetwork playerNetworkObject)
    {
        gameMode.InitializePlayer(_isOwner, playerNetworkObject);


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

        if (!_reconnecting) Debug.LogAssertion("gagagagagag");
        if (!_reconnecting) SpawnAtPlaceRPC(player, currentMap.SpawnPlaces[internalSpawnTicker % currentMap.SpawnPlaces.Count].position, RpcTarget.Single(player.NetworkID, RpcTargetUse.Temp));
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
        player.rb.MovePosition(new Vector3(0, 0, -10000));
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



    public void Game_ConstructSingleton()
    {
        //Debug.Log("Instance is" + instance);
        if (game_instance != null && game_instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            game_instance = this;
        }
    }
}
