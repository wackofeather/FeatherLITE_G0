using Unity.Netcode;
using UnityEngine;

public abstract class BaseGameMode
{

    public virtual void StartGame()
    {
        
    }
    public virtual void EndGame()
    {

    }

    public virtual void ServerSideRespawnplayer(NetworkObjectReference playerNetworkObject, ulong NetworkID)
    {

    }

    public virtual void OwnerSideRespawnPlayer(NetworkObjectReference playerNetworkObject, int SpawnTicker, RpcParams _param)
    {

    }

    public abstract void InitializePlayer(bool _isOwner, PlayerNetwork playerNetworkObject);
}
