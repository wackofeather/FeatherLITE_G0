using Steamworks;
using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Game_GeneralManager;
using static PlayerNetwork;
using System.Linq;
using UnityEngine.UI;
public class GeneralManager : NetworkBehaviour
{
    public static GeneralManager instance { get; set; }


    public MapLookup mapLookup;


    public float testFloat;

    [HideInInspector] public ulong currentLobbyOwner;

    [HideInInspector] public float NetworkID;

    [HideInInspector] public bool wantConnection;

    private Slider FOVslider;

    internal Coroutine HostMigrationCorT;
    internal Coroutine JoiningGameCorT;


    public NetworkVariable<ulong> CurrentHost = new NetworkVariable<ulong>(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<ulong> BackupHost = new NetworkVariable<ulong>(writePerm: NetworkVariableWritePermission.Server);


    public void Awake()
    {
        _Awake();

    }

    public virtual void _Awake()
    {
        ConstructSingleton();
    }

    private void Start()
    {

        _Start();
    }

    public virtual void _Start()
    {
        wantConnection = false;
        /*        NetworkManager.SceneManager.PostSynchronizationSceneUnloading = false;

                NetworkManager.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);
                Debug.Log("hahahahajajajajaj" + NetworkManager.SceneManager.ClientSynchronizationMode);*/
        //SceneManager.LoadScene(GameScene, LoadSceneMode.Additive);
        SteamMatchmaking.OnLobbyDataChanged += OnLobbyDataChange;

        currentLobbyOwner = SteamLobbyManager.currentLobby.Owner.Id;
    }

    public virtual void OnLobbyDataChange(Lobby newLobbyData)
    {
        Debug.LogWarning("yeeyee ass haircut" + newLobbyData.GetData("Map"));
        Debug.LogAssertion("jeez louise  " + newLobbyData.Owner.Id + "   " + currentLobbyOwner);
        //if (!wantConnection) return;
        if (currentLobbyOwner != newLobbyData.Owner.Id)
        {
            Debug.LogAssertion("loooooooooooooooooooooook");
            if (HostMigrationCorT != null) StopCoroutine(HostMigrationCorT);
            if (JoiningGameCorT != null) StopCoroutine(JoiningGameCorT);


            HostMigrationCorT = StartCoroutine(HostMigrationCoroutine());
        }
    }
    /*    public virtual void OnConnectedToSession(bool _reconnecting)
        {

        }*/
    public virtual void PrepareHM()
    {

    }
    public async virtual void ExitHM()
    {

    }
    public virtual IEnumerator HostMigrationCoroutine()
    {
        SteamLobbyManager.instance.reconnecting = true;

        PrepareHM();

        currentLobbyOwner = SteamLobbyManager.currentLobby.Owner.Id;

        NetworkManager.Shutdown();



        while (NetworkManager.ShutdownInProgress)
        {
            Debug.Log("shutting down");
            yield return null;
        }

        Debug.LogWarning("shut down");
        //Debug.LogWarning(clientId == Player_LookUp[CurrentHost.Value].NetworkID);

        //NetworkManager.gameObject.SetActive(true);


        //Player_LookUp.Clear();




        Debug.LogWarning(SteamLobbyManager.currentLobby.Owner.Id);

        if (SteamLobbyManager.currentLobby.Owner.Id == SteamClient.SteamId.Value)
        {
            //NetworkManager.en
            JoiningGameCorT = StartCoroutine(SteamLobbyManager.instance.JoiningGameCoroutine(SteamLobbyManager.currentLobby.Owner.Id, true, true));
        }
        else JoiningGameCorT = StartCoroutine(SteamLobbyManager.instance.JoiningGameCoroutine(SteamLobbyManager.currentLobby.Owner.Id, false, true));

        Debug.Log("balsjhskshdidididididididididdi" + (NetworkManager.SpawnManager != null));


        while (NetworkManager.SpawnManager != null)
        {
            if (!SteamLobbyManager.instance.reconnecting) yield break;
            yield return null;
        }

        //yield return null;
        ///yield return new WaitUntil(() => NetworkManager.SpawnManager != null);
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

        ExitHM();

        yield break;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _OnNetworkSpawn();
    }

    public virtual void _OnNetworkSpawn()
    {
        if (IsHost)
        {
            Debug.Log("ratatatata");
            ///if (wantConnection) return;
            NetworkManager.SceneManager.PostSynchronizationSceneUnloading = false;

            NetworkManager.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);
            Debug.Log("hahahahajajajajaj" + NetworkManager.SceneManager.ClientSynchronizationMode);
            this.NetworkObject.DestroyWithScene = false;
            this.NetworkObject.DontDestroyWithOwner = true;

            FindBackupHost();
        }



        NetworkManager.SceneManager.VerifySceneBeforeLoading += VerifySceneLoading;
        NetworkManager.SceneManager.VerifySceneBeforeUnloading += VerifySceneUnLoading;
        NetworkManager.OnClientDisconnectCallback += Disconnect;
        ///NetworkManager.OnClientDisconnectCallback += TryReconnection;


        currentLobbyOwner = SteamLobbyManager.currentLobby.Owner.Id;
        NetworkID = NetworkManager.Singleton.LocalClientId;

        if (!wantConnection) testFloat = UnityEngine.Random.Range(0, 1000f);

        wantConnection = true;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        //wantConnection = false;

        NetworkManager.SceneManager.VerifySceneBeforeLoading -= VerifySceneLoading;
        NetworkManager.SceneManager.VerifySceneBeforeUnloading -= VerifySceneUnLoading;
        NetworkManager.OnClientDisconnectCallback -= Disconnect;
        ///NetworkManager.OnClientDisconnectCallback -= TryReconnection;

        //destroy playerobj
        //remove from lookups

        //SteamLobbyManager.instance.LeaveLobby();

        Debug.Log("despawing");
    }

    internal bool VerifySceneUnLoading(Scene scene)
    {

        //Debug.LogWarning("unloading" + scene.name);
        if (mapLookup.GetMapLookUp().ContainsValue(scene.name)) return true;
        else return true;
    }

    internal bool VerifySceneLoading(int sceneIndex, string sceneName, LoadSceneMode loadSceneMode)
    {
        Debug.LogWarning(sceneName);
        /*        if (sceneName == GameScene) return true;
                else return false;*/
        return true;
    }
    internal void Disconnect(ulong DisconnectingNetworkID)
    {
        if (!IsServer) return;

        //if (wantConnection) return;
        //Debug.LogAssertion("blahashsksjs");
        DisconnectRPC(DisconnectingNetworkID);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public virtual void DisconnectRPC(ulong DisconnectingNetworkID)
    {
        Debug.LogAssertion("bahsjssksksksksks  " + DisconnectingNetworkID);
        LocalDisconnect(DisconnectingNetworkID);
    }

    public virtual void LocalDisconnect(ulong DisconnectingNetworkID)
    {

    }



    void FindBackupHost()
    {
        List<Friend> LobbyMembers = SteamLobbyManager.currentLobby.Members.ToList<Friend>();
        if (LobbyMembers.Count >= 2)
        {
            BackupHost.Value = LobbyMembers.Find(member => member.Id != SteamLobbyManager.currentLobby.Owner.Id).Id;
            //HostBackups = Player_LookUp.Values.ToList().Select(person => person.SteamID).ToList();
        }
        /*        if (Player_LookUp.Count == 2) 
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
                }*/

        //Debug.LogWarning(Player_LookUp.Count);

    }

    void Update()
    {
        _Update();

    }

    public virtual void _Update()
    {
        //Debug.Log("testing this");
        if (IsServer)
        {
            CurrentHost.Value = SteamClient.SteamId;
            FindBackupHost();
        }
    }
    public virtual void ConstructSingleton()
    {
        Debug.Log("fart teehee");
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

/*    public void OnDestroy()
    {



        // wantConnection = false;
        SteamLobbyManager.instance.LeaveLobby();
        Debug.LogAssertion("hoooooha");
    }

    //Options logic
    public void saveData()
    {

    }
}*/
