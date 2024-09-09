using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SteamLobbyManager : MonoBehaviour
{

    public static SteamLobbyManager instance { get; set; }

    public GameObject networkManager;

    public GameObject playerPrefab;

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

    public string MenuScene;
/*    public GameObject InLobbyFriend;
    public Transform content;*/

    //public Dictionary<SteamId, GameObject> inLobby = new Dictionary<SteamId, GameObject>();


    bool JoiningLobby;

    private void Start()
    {
        //DontDestroyOnLoad(this);

        SteamMatchmaking.OnLobbyCreated += OnLobbyCreatedCallBack;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        //SteamMatchmaking.OnChatMessage += OnChatMessage;
        SteamMatchmaking.OnLobbyMemberDisconnected += OnLobbyMemberDisconnected;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberDisconnected;
        //SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;
        //SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequest;
        //SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;

        ///SceneManager.LoadScene(MenuScene);



        JoiningLobby = false;
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
            currentLobby.SetPublic();
            //currentLobby.SetPrivate();
            currentLobby.SetJoinable(true);

            StartCoroutine(SteamLobbyManager.instance.JoiningGameCoroutine(currentLobby.Owner.Id, true));

            Debug.LogAssertion(currentLobby.Id);

            return true;
        }
        catch(System.Exception exception)
        {
            Debug.Log("Failed to create multiplayer lobby : " + exception);
            return false;
        }
    }

    IEnumerator JoiningGameCoroutine(SteamId targetID, bool hosting)
    {

        if (JoiningLobby) yield break;


        JoiningLobby = true;

        NetworkManager.Singleton.gameObject.GetComponent<FacepunchTransport>().targetSteamId = targetID;

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

            NetworkManager.Singleton.gameObject.GetComponent<UnityNetworkManager>().StartHost();


            SceneEventProgressStatus loadScene = NetworkManager.Singleton.SceneManager.LoadScene(SteamLobbyManager.instance.GameScene, LoadSceneMode.Single);


            while (true)
            {
                if (loadScene == SceneEventProgressStatus.Started) { break; }
                Debug.Log("beep");
                yield return null;
            }
            GUIUtility.systemCopyBuffer = targetID.Value.ToString();

        }
        else 
        { 
            NetworkManager.Singleton.gameObject.GetComponent<UnityNetworkManager>().StartClient();
            Debug.Log("nabsjagansjshsnauisusjskajsksjaksushenehshssnsnsnsnsnsnsnsns");
        }


        //Debug.LogAssertion(Game_GeneralManager.instance != null);


        while (!NetworkManager.Singleton.IsConnectedClient)
        {
            //Debug.Log("ggsgsgsg");
            yield return null;
        }


        //Game_GeneralManager.instance.SpawnPlayerRPC(NetworkManager.Singleton.LocalClientId);

        JoiningLobby = false;
        yield break;
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
            StartCoroutine(SteamLobbyManager.instance.JoiningGameCoroutine(lobbyId, false));
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
           // UserInLobby = false;
            /*            currentLobby.Leave();
                        //OnLobbyLeave.Invoke();
            *//*            foreach (var user in inLobby.Values)
                        {
                            Destroy(user);
                        }*//*
                        inLobby.Clear();*/

           // currentLobby.Leave();
        }
        catch
        {

        }
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
