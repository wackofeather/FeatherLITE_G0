using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public struct PlayerData : INetworkSerializable
{
    public Vector3 position;
    public float health;
    public ulong SteamID;
    public ulong NetworkID;
    public NetworkObjectReference PlayerObjectReference;

    public PlayerData(Vector3 _position, float _health, ulong _SteamID, ulong _NetworkID, NetworkObjectReference _PlayerObjectReference)
    {
        position = _position;
        health = _health;
        SteamID = _SteamID;
        NetworkID = _NetworkID;
        PlayerObjectReference = _PlayerObjectReference;
    }


    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref health);
        serializer.SerializeValue(ref SteamID);
        serializer.SerializeValue(ref NetworkID);
        serializer.SerializeValue(ref PlayerObjectReference);
    }
}