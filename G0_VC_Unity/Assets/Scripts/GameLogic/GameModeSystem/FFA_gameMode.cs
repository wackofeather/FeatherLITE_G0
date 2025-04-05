using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class FFA_gameMode : BaseGameMode
{
    public override void InitializePlayer(bool _isOwner, PlayerNetwork playerNetworkObject)
    {
        if (_isOwner)
        {
            return;
        }

        playerNetworkObject.playerStateMachine.gameObject.layer = LayerMask.NameToLayer("ENEMY");
    }

    public async override void OwnerSideRespawnPlayer(NetworkObjectReference playerNetworkObject, int SpawnTicker, RpcParams _param)
    {
        base.OwnerSideRespawnPlayer(playerNetworkObject, SpawnTicker, _param);

        playerNetworkObject.TryGet(out NetworkObject playerObj);
        await Task.Yield();
        Debug.Log("networkobject is" + Game_GeneralManager.game_instance.currentMap.SpawnPlaces[SpawnTicker % Game_GeneralManager.game_instance.currentMap.SpawnPlaces.Count]);
        playerObj.gameObject.GetComponent<PlayerNetwork>().playerStateMachine.gameObject.transform.position = Game_GeneralManager.game_instance.currentMap.SpawnPlaces[SpawnTicker % Game_GeneralManager.game_instance.currentMap.SpawnPlaces.Count].position;
        while (playerObj.gameObject.GetComponent<PlayerNetwork>().playerStateMachine.rb.position != Game_GeneralManager.game_instance.currentMap.SpawnPlaces[SpawnTicker % Game_GeneralManager.game_instance.currentMap.SpawnPlaces.Count].position)
        {
            playerObj.gameObject.GetComponent<PlayerNetwork>().playerStateMachine.rb.position = Game_GeneralManager.game_instance.currentMap.SpawnPlaces[SpawnTicker % Game_GeneralManager.game_instance.currentMap.SpawnPlaces.Count].position;
            await Task.Yield();
        }
        //.rb.MovePosition(SpawnPlaces[internalSpawnTicker % SpawnPlaces.Count].position);
        playerObj.gameObject.GetComponent<PlayerNetwork>().SetHealthRPC(100, Game_GeneralManager.game_instance.RpcTarget.Owner);
        Debug.LogWarning(Game_GeneralManager.game_instance.currentMap.SpawnPlaces[SpawnTicker % Game_GeneralManager.game_instance.currentMap.SpawnPlaces.Count].position + "missed the train");
    }

    public override void ServerSideRespawnplayer(NetworkObjectReference playerNetworkObject, ulong NetworkID)
    {
        base.ServerSideRespawnplayer(playerNetworkObject, NetworkID);

        Debug.Log("lalalalalapoopoo");
        Game_GeneralManager.game_instance.Owner_SpawnPlayerForGameRPC(playerNetworkObject, Game_GeneralManager.game_instance.internalSpawnTicker, Game_GeneralManager.game_instance.RpcTarget.Single(NetworkID, RpcTargetUse.Temp));
        Debug.Log("spawnTicker is" + Game_GeneralManager.game_instance.internalSpawnTicker);
        Game_GeneralManager.game_instance.internalSpawnTicker++;
    }
}
