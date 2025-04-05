using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SteamLobbyManager : MonoBehaviour
{
    bool gameSceneLoaded;
    public static SteamLobbyManager instance { get; set; }

    public GameObject networkManager;

    public GameObject playerPrefab;

    public MapLookup mapLookup;

    public string gameSetupScene;

    [HideInInspector] public bool reconnecting;

    private void Awake()
    {
        ConstructSingleton();


        DontDestroyOnLoad(this);

        //DontDestroyOnLoad(networkManager);

        



        //Steamworks.SteamClient.Init(480, true);

    }

    public virtual void ConstructSingleton()
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


    public static Lobby currentLobby;
    public static bool UserInLobby;
    /*    public UnityEvent OnLobbyCreated;
        public UnityEvent OnLobbyJoined;
        public UnityEvent OnLobbyLeave;*/

    public String GameScene;

    public string PreLoadScene;

    public string MenuScene;
/*    public GameObject InLobbyFriend;
    public Transform content;*/

    //public Dictionary<SteamId, GameObject> inLobby = new Dictionary<SteamId, GameObject>();


    public bool JoiningLobby;

    private void Start()
    {
        //DontDestroyOnLoad(this);

        SteamMatchmaking.OnLobbyCreated += OnLobbyCreatedCallBack;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        //SteamMatchmaking.OnChatMessage += OnChatMessage;
        SteamMatchmaking.OnLobbyMemberDisconnected += OnLobbyMemberDisconnected;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberDisconnected;
        //SteamMatchmaking.onlobbyle
        //SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;
        //SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequest;
        //SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;

        //To get playerPrefs stuff

        // NetworkManager.Singleton.SceneManager.OnLoadComplete += OnGameSceneLoaded;

        SceneManager.LoadScene(MenuScene);

       

        JoiningLobby = false;
    }

    private void OnGameSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        Debug.LogWarning(sceneName);
        if (sceneName == GameScene) gameSceneLoaded = true;
    }

    private void OnRegularSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        //if (scene.name == GameScene) gameSceneLoaded = true;
        if (mapLookup.GetMapLookUp().Values.Contains(scene.name)| scene.name == "LobbyScene") gameSceneLoaded = true;
    }

    /*    void OnLobbyInvite(Friend friend, Lobby lobby)
        {
            Debug.Log($"{friend.Name} invited you to his lobby.");
        }*/


    /*    private void OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId id)
        {

        }*/

    private async void OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {
        Debug.Log($"{friend.Name} joined the lobby");
/*        GameObject obj = Instantiate(InLobbyFriend, content);
        obj.GetComponentInChildren<Text>().text = friend.Name;
        obj.GetComponentInChildren<RawImage>().texture = await SteamFriendsManager.GetTextureFromSteamIdAsync(friend.Id);
        inLobby.Add(friend.Id, obj);*/
    }

    void OnLobbyMemberDisconnected(Lobby lobby, Friend friend)
    {
        Debug.Log($"{friend.Name} left the lobby");
        Debug.Log($"New lobby owner is {currentLobby.Owner}");

        if (friend.IsMe) Debug.LogWarning("uh oh");
/*        if (inLobby.ContainsKey(friend.Id))
        {
            Destroy(inLobby[friend.Id]);
            inLobby.Remove(friend.Id);
        }*/
    }

/*    void OnChatMessage(Lobby lobby, Friend friend, string message)
    {
        Debug.Log($"incoming chat message from {friend.Name} : {message}");
    }

    async void OnGameLobbyJoinRequest(Lobby joinedLobby, SteamId id)
    {
        RoomEnter joinedLobbySuccess = await joinedLobby.Join();
        if (joinedLobbySuccess != RoomEnter.Success)
        {
            Debug.Log("failed to join lobby : " + joinedLobbySuccess);
        }
        else
        {
            currentLobby = joinedLobby;
        }
    }*/

    void OnLobbyCreatedCallBack(Result result, Lobby lobby)
    {
        if (result != Result.OK)
        {
            Debug.Log("lobby creation result not ok : " + result);
        }
        else
        {
            //OnLobbyCreated.Invoke();
            Debug.Log("lobby creation result ok");
        }
    }

    async void OnLobbyEntered(Lobby lobby)
    {
        Debug.Log("Client joined the lobby");
        UserInLobby = true;

/*        foreach (var user in inLobby.Values)
        {
            Destroy(user);
        }
        inLobby.Clear();*/

        //GameObject obj = Instantiate(InLobbyFriend, content);
/*        obj.GetComponentInChildren<Text>().text = SteamClient.Name;
        obj.GetComponentInChildren<RawImage>().texture = await SteamFriendsManager.GetTextureFromSteamIdAsync(SteamClient.SteamId);*/

        //inLobby.Add(SteamClient.SteamId, obj);

/*        foreach (var friend in currentLobby.Members)
        {
            if (friend.Id != SteamClient.SteamId)
            {
                GameObject obj2 = Instantiate(InLobbyFriend, content);
                obj2.GetComponentInChildren<Text>().text = friend.Name;
                obj2.GetComponentInChildren<RawImage>().texture = await SteamFriendsManager.GetTextureFromSteamIdAsync(friend.Id);

                inLobby.Add(friend.Id, obj2);
            }
        }*/
       // OnLobbyJoined.Invoke();
    }


/*    public async void CreateLobbyAsync()
    {
        bool result = await CreateLobby();
        if (!result)
        {
            //Invoke a error message.
        }
    }*/

    public async Task<bool> CreateLobby()
    {
        try
        {
            var createLobbyOutput = await SteamMatchmaking.CreateLobbyAsync();
            if (!createLobbyOutput.HasValue)
            {
                Debug.Log("Lobby created but not correctly instantiated.");
                return false;
            }
            currentLobby = createLobbyOutput.Value;
            currentLobby.SetData("GameMode", "FFA");
            currentLobby.SetData("Map", "Lobby");
            

            StartCoroutine(SteamLobbyManager.instance.JoiningGameCoroutine(currentLobby.Owner.Id, true, false));

            //Debug.LogAssertion(currentLobby.Id);

            return true;
        }
        catch(System.Exception exception)
        {
            Debug.Log("Failed to create multiplayer lobby : " + exception);
            return false;
        }
    }

    public IEnumerator JoiningGameCoroutine(ulong targetID, bool hosting, bool _reconnecting)
    {

        //if (JoiningLobby) { Debug.LogWarning("blahhhhh"); yield break; }

        

        JoiningLobby = true;

        if (_reconnecting)
        {
            yield return new WaitUntil(() => currentLobby.IsOwnedBy(targetID));
            /*Debug.Log(currentLobby.IsOwnedBy(SteamClient.SteamId));

            yield return new WaitUntil(() => !currentLobby.IsOwnedBy(Game_GeneralManager.instance.CurrentHost.Value));

            Debug.Log(currentLobby.IsOwnedBy(SteamClient.SteamId) + "2");
            
            *//*            while (true)
                        {*//*
            if (currentLobby.IsOwnedBy(SteamClient.SteamId))
            {
                if (!hosting)
                {
                    Debug.Log("transferring ownership");
                    currentLobby.Owner = new Friend(Game_GeneralManager.instance.BackupHost.Value);
                }
            }

            yield return new WaitUntil(() => currentLobby.IsOwnedBy(Game_GeneralManager.instance.BackupHost.Value));

            Debug.LogAssertion("ownership successfully transferred");
            //Debug.LogWarning("waiting for ne owner" + currentLobby.Owner.Id);
            *//*                if (Game_GeneralManager.instance.PlayerGameObject_LocalLookUp.ContainsKey(currentLobby.Owner.Id))
                            {
                            Debug.LogWarning(currentLobby.Owner.Id);*/
            NetworkManager.Singleton.gameObject.GetComponent<FacepunchTransport>().targetSteamId = currentLobby.Owner.Id;
/*                    break;
            }*/
            //yield return null;
            //}
        } 
        else NetworkManager.Singleton.gameObject.GetComponent<FacepunchTransport>().targetSteamId = targetID;

        /*        SceneEventProgressStatus loadScene = NetworkManager.SceneManager.LoadScene(SteamLobbyManager.instance.GameScene, LoadSceneMode.Single);


                while (true)
                {
                    if (loadScene == SceneEventProgressStatus.Started) { break; }
                    Debug.Log("beep");
                    yield return null;
                }*/

        //yield return new WaitUntil(() => SceneManager.GetActiveScene().name == GameScene);


        //Debug.Log(SceneManager.GetActiveScene().name);

        if (hosting)
        {



            if (!_reconnecting)
            {
                gameSceneLoaded = false;

                SceneManager.sceneLoaded += OnRegularSceneLoaded; //when Scene Loaded call this. 

                if (currentLobby.GetData("Map") == "Lobby") SceneManager.LoadScene("LobbyScene");
                else
                {
                    SceneManager.LoadScene(gameSetupScene);
                }
                    //SceneManager.LoadScene(mapLookup.GetMapLookUp()[currentLobby.GetData("Map")], LoadSceneMode.Single);
                    //SceneManager.LoadScene("LobbyScene");
                    //SceneManager.LoadScene(gameSetupScene); 

                    while (true)
                    {
                        if (gameSceneLoaded == true) { break; }
                        Debug.Log("beep");
                        yield return null;
                    }

                SceneManager.sceneLoaded -= OnRegularSceneLoaded;
            }

            GUIUtility.systemCopyBuffer = currentLobby.Id.ToString();

            ///NetworkManager.Singleton.StartHost();
            ///
            Task<bool> tryConnect = TryConnect(true);
            yield return new WaitUntil(() => tryConnect.IsCompleted);
            if (tryConnect.Result == false)
            {
                if (_reconnecting) 
                { 
                    reconnecting = false; 
                }

                RevertToMenu();

                yield break;
            }

            if (!_reconnecting)
            {
                currentLobby.SetPublic();
                //currentLobby.SetPrivate();
                currentLobby.SetJoinable(true);
            }
            //Debug.Log(Game_GeneralManager.instance != null);
            //Game_GeneralManager.instance.SpawnPlayerRPC(Steamworks.SteamClient.SteamId);
            Debug.Log(NetworkManager.Singleton.IsConnectedClient);

        }
        else 
        {
            /*            gameSceneLoaded = false;

                        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnGameSceneLoaded;

                        NetworkManager.Singleton.SceneManager.LoadScene(SteamLobbyManager.instance.PreLoadScene, LoadSceneMode.Single);



                        while (true)
                        {
                            if (gameSceneLoaded == true) { break; }
                            Debug.Log("beep");
                            yield return null;
                        }*/
            //
            /*            if (NetworkManager.Singleton.StartClient())
                        {

                        }
                        else
                        {
                            Debug.LogWarning("blahaj");
                            Game_GeneralManager.instance.reconnecting = false;
                            yield break;
                        }*/
            //


            if (!_reconnecting)
            {
                gameSceneLoaded = false;

                SceneManager.sceneLoaded += OnRegularSceneLoaded;

                //SceneManager.LoadScene(mapLookup.GetMapLookUp()[currentLobby.GetData("Map")], LoadSceneMode.Single);
                if (currentLobby.GetData("Map") == "Lobby") SceneManager.LoadScene("LobbyScene");
                else
                {
                    SceneManager.LoadScene(gameSetupScene);
                }


                while (true)
                {
                    if (gameSceneLoaded == true) { break; }
                    Debug.Log("beep");
                    yield return null;
                }

                SceneManager.sceneLoaded -= OnRegularSceneLoaded;
            }


            Task<bool> tryConnect = TryConnect(false);
            yield return new WaitUntil(() => tryConnect.IsCompleted);
            if (tryConnect.Result == false)
            {
                if (_reconnecting)
                {
                    reconnecting = false;
                }

                RevertToMenu();

                yield break;
            }


            /*            if (NetworkManager.Singleton.StartClient())
                        {

                        }
                        else
                        {
                            Game_GeneralManager.instance.reconnecting = false;
                            yield break;
                        }*/

            //Game_GeneralManager.instance.SpawnPlayerRPC(Steamworks.SteamClient.SteamId);
            //Debug.LogWarning("kiki " + currentLobby.GetData("GameMode"));
        }


        //Debug.LogAssertion(Game_GeneralManager.instance != null);


        while (!NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.Log(NetworkManager.Singleton.IsConnectedClient);
            yield return null;
        }
        Debug.LogWarning(GeneralManager.instance);
        //GeneralManager.instance.OnConnectedToSession(_reconnecting);

        //Game_GeneralManager.instance.SpawnPlayerRPC(NetworkManager.Singleton.LocalClientId);

        JoiningLobby = false;
        yield break;
    }


    public async Task<bool> TryConnect(bool Hosting)
    {
        if (Hosting)
        {
            if (NetworkManager.Singleton.StartHost())
            {
                return true;
            }
            else return false;
        }
        else
        {
            if (NetworkManager.Singleton.StartClient())
            {
                while (NetworkManager.Singleton.GetComponent<FacepunchTransport>().connectionManager == null)
                {
                    Debug.LogWarning("waiting");
                    await Task.Yield();
                }
                while (!NetworkManager.Singleton.GetComponent<FacepunchTransport>().connectionManager.Connected)
                {
                    if (!NetworkManager.Singleton.GetComponent<FacepunchTransport>().connectionManager.Connecting)
                    {
                        return false;
                    }
                    await Task.Yield();
                }
                return true;
            }
            else return false;
        }
    }


    public async Task<bool> JoinLobbyAsync(SteamId lobbyId)
    {
        /*bool result = await JoinLobby(lobbyId);
        if (!result)
        {
            //Invoke a error message.
        }*/

        try
        {
            var createLobbyOutput = await SteamMatchmaking.JoinLobbyAsync(lobbyId);
            if (!createLobbyOutput.HasValue)
            {
                Debug.Log("Lobby created but not correctly instantiated.");
                return false;
            }
            currentLobby = createLobbyOutput.Value;

            Debug.LogWarning((lobbyId == currentLobby.Owner.Id) + "  " + lobbyId + "   " + currentLobby.MemberCount);
            StartCoroutine(SteamLobbyManager.instance.JoiningGameCoroutine(currentLobby.Owner.Id, false, false));
            return true;
        }
        catch(System.Exception exception)
        {
            Debug.Log("Failed to create multiplayer lobby : " + exception);
            return false;
        }
       
    }

    /*public async Task<bool> JoinLobby(SteamId lobbyId)
    {
        try
        {
            //await SteamMatchmaking.JoinLobbyAsync(lobbyId);


            StartCoroutine(JoiningGameCoroutine(lobbyId, false));

*//*            NetworkManager.Singleton.gameObject.GetComponent<FacepunchTransport>().targetSteamId = lobbyId;

            SceneManager.LoadScene(SteamLobbyManager.instance.GameScene);*//*

            

            return true;
        }
        catch (System.Exception exception)
        {
            Debug.Log("Failed to join multiplayer lobby : " + exception);
            return false;
        }
    }*/
    
    public void LeaveLobby()
    {
        try
        {
            UserInLobby = false;
            currentLobby.Leave();
            //OnLobbyLeave.Invoke();

            // currentLobby.Leave();
        }
        catch
        {

        }
    }

    public async void RevertToMenu()
    {
        LeaveLobby();

        if (SceneManager.GetActiveScene().name != MenuScene)
        {
            SceneManager.LoadScene(MenuScene);
            while (SceneManager.GetActiveScene().name != MenuScene) { await Task.Yield(); }
        }

        //display disconnect info

    }



    public void OnApplicationQuit()
    {
        Debug.Log("bye");
        //NetworkManager.Singleton.Shutdown();


    }
    
    public void OnDestroy()
    {
        Debug.Log("ahh im dying");
    }




}
