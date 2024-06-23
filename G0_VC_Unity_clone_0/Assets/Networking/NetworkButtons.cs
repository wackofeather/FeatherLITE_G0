using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Steamworks;

public class NetworkButtons : MonoBehaviour
{
    [HideInInspector] public UnityNetworkManager unityNetworkManager;
    private void Start()
    {
        unityNetworkManager = GetComponent<UnityNetworkManager>();
    }
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            /*            if (GUILayout.Button("Host")) GetComponent<SteamNetworkManager>().StartHost(10);
                        if (GUILayout.Button("Client")) GetComponent<SteamNetworkManager>().StartClient(Steamworks.SteamClient.SteamId);*/
            if (GUILayout.Button("Host")) unityNetworkManager.StartHost();
            if (GUILayout.Button("Client")) unityNetworkManager.StartClient();
        }

        GUILayout.EndArea();
    }

    // private void Awake() {
    //     GetComponent<UnityTransport>().SetDebugSimulatorParameters(
    //         packetDelay: 120,
    //         packetJitter: 5,
    //         dropRate: 3);
    // }
}