using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Steamworks;
using Netcode.Transports.Facepunch;
using System.Threading.Tasks;
using Steamworks.Data;
using System;
using UnityEngine.SceneManagement;

public class NetworkButtons : MonoBehaviour
{
    [HideInInspector] public UnityNetworkManager unityNetworkManager;

    [HideInInspector] public FacepunchTransport transport;

    public static Lobby currentLobby;

    public string LobbyID_Input = "Put Steam Lobby ID Here";

    public GameObject playerPrefab;
    private void Start()
    {
        unityNetworkManager = NetworkManager.Singleton.gameObject.GetComponent<UnityNetworkManager>();

        transport = GetComponent<FacepunchTransport>();
    }
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        LobbyID_Input = GUILayout.TextField(LobbyID_Input);
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            /*            if (GUILayout.Button("Host")) GetComponent<SteamNetworkManager>().StartHost(10);
                        if (GUILayout.Button("Client")) GetComponent<SteamNetworkManager>().StartClient(Steamworks.SteamClient.SteamId);*/
            if (GUILayout.Button("Host")) 
            {
                SteamLobbyManager.instance.CreateLobby();
                //NetworkManager.Singleton.gameObject.GetComponent<NetworkHelper>().StartAHost();
                /*                SteamLobbyManager.instance.CreateLobbyAsync();
                                Debug.Log(SceneManager.GetActiveScene().name);
                                unityNetworkManager.StartHost();*/
            }
            
            if (GUILayout.Button("Client"))
            {

                //SteamLobbyManager.instance.JoinLobbyAsync(Convert.ToUInt64(LobbyID_Input));
                SteamLobbyManager.instance.JoinLobbyAsync(Convert.ToUInt64(LobbyID_Input));
                /*                SteamId lobbyId = Convert.ToUInt64(LobbyID_Input);
                                SteamLobbyManager.instance.JoinLobbyAsync(lobbyId);
                                unityNetworkManager.StartClient();*/
            }

            
        }

        GUILayout.EndArea();
    }


/*    public async void StartAHost()
    {
        bool successful = await 

        if (successful)
        {
            Debug.Log(SceneManager.GetActiveScene().name);

        }
    }
*/

    



    // private void Awake() {
    //     GetComponent<UnityTransport>().SetDebugSimulatorParameters(
    //         packetDelay: 120,
    //         packetJitter: 5,
    //         dropRate: 3);
    // }
}