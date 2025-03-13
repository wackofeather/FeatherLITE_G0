using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "RailgunClass", menuName = "Scriptable Objects/RailgunClass")]
public class RailgunClass : GunClass
{
    public int bounceNumber;
    public float maxHitDistance;
    public LayerMask endHitMask;
    public LayerMask enemymask;
    BeamPath path;
    public float beamSpeed;
    public override void Weapon_Update()
    {
        base.Weapon_Update();

        
        if (player.networkInfo._isOwner)
        {
            path = GetBouncePath();

            player.playerNetwork.currentRailPath.Value = path;

            player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().predictivePath.positionCount = path.points.Count;

            for (int i = 0; i < path.points.Count; i++)
            {
                player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().predictivePath.SetPosition(i, path.points[i].point);

            }
        }
        else
        {
            path = player.playerNetwork.currentRailPath.Value;

            player.inventory.EXT_GetProxy().GetComponent<RailgunProxy>().predictivePath.positionCount = path.points.Count;

            for (int i = 0; i < path.points.Count; i++)
            {
                player.inventory.EXT_GetProxy().GetComponent<RailgunProxy>().predictivePath.SetPosition(i, path.points[i].point);

            }
        }

    }

    public override void ExitWeapon()
    {
        base.ExitWeapon();

        if (player.networkInfo._isOwner)
        {
            player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().predictivePath.positionCount = 0;
            player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().shootPath.positionCount = 0;
        }
        else
        {
            player.inventory.EXT_GetProxy().GetComponent<RailgunProxy>().predictivePath.positionCount = 0;
            player.inventory.EXT_GetProxy().GetComponent<RailgunProxy>().shootPath.positionCount = 0;
        }
    }
    public async override Task<bool> ShootLogic()
    {
        BeamPath shootPathContainer = path;
        

        Debug.Log("ladeeda" + player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().shootPath.GetPosition(1));
        player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().shootPath.material.color = new Color(player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().shootPath.material.color.r, player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().shootPath.material.color.g, player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().shootPath.material.color.b, 1);
        for (int i = 0; i < shootPathContainer.points.Count; i++)
        {
            player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().shootPath.positionCount = i + 1;
            /*Debug.Log("broh" + i);
            Debug.LogAssertion(player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().shootPath.GetPosition(1));*/
            //spawn appropriate fx here
            BounceHit hit = shootPathContainer.points[i];
            player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().shootPath.SetPosition(i, hit.point);

            if (hit.hitType == 2) 
            { 
                try { Game_GeneralManager.game_instance.PlayerGameObject_LocalLookUp[hit.playerHitID].playerNetwork.DamageRPC(100); } 
                catch { } 
                continue; 
            }

            await Task.Delay((int)(1 / beamSpeed * 1000));
        }

        await Task.Delay(1000);


        while (player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().shootPath.material.color.a > 0)
        {
            player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().shootPath.material.color = new Color(player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().shootPath.material.color.r, player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().shootPath.material.color.g, player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().shootPath.material.color.b, (float)(player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().shootPath.material.color.a - 10 * Time.deltaTime));
            await Task.Yield();
        }
        player.inventory.VP_GetProxy().GetComponent<RailgunProxy>().shootPath.positionCount = 0;

        return true;
    }

    public async override Task<bool> DummyShoot()
    {
        player.inventory.EXT_GetProxy().GetComponent<RailgunProxy>().predictivePath.positionCount = path.points.Count;

        for (int i = 0; i < path.points.Count; i++)
        {
            //spawn appropriate fx here
            BounceHit hit = path.points[i];
            player.inventory.EXT_GetProxy().GetComponent<RailgunProxy>().shootPath.SetPosition(i, hit.point);

            if (hit.hitType == 2) { try { Game_GeneralManager.game_instance.PlayerGameObject_LocalLookUp[hit.playerHitID].playerNetwork.DamageRPC(100); } catch { } }

            await Task.Delay((int)(1 / beamSpeed));
        }
        return true;
    }

    public BeamPath GetBouncePath()
    {
        
        BeamPath path = new BeamPath();
        Vector3 direction = player.PlayerCamera.transform.forward;
        float hitDistance = maxHitDistance;

        path.points.Add(new BounceHit(player.inventory.VP_GetProxy().GetComponent<GunProxy>().gunTip.transform.position, 0, 0));
        for (int i = 1; i <= bounceNumber; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(path.points[i-1].point, direction, out hit, hitDistance))
            {

                


                Debug.Log(hit.collider.gameObject.layer);
                hitDistance -= Vector3.Distance(hit.point, path.points[i - 1].point);

                if (hit.collider.gameObject.layer == Mathf.RoundToInt(Mathf.Log(enemymask.value, 2))) { path.points.Add(new BounceHit(hit.point, 2, hit.collider.gameObject.GetComponent<PlayerStateMachine>().playerNetwork._SteamID.Value)); continue; }
                else
                {
                        



                    path.points.Add(new BounceHit(hit.point, 1, 0));

                    direction = Vector3.Reflect(direction, hit.normal);
                }


                if (Vector3.Angle(hit.normal, direction) < 15) { break; }
            }
            else
            {

                path.points.Add(new BounceHit(path.points[path.points.Count - 1].point + (direction).normalized * hitDistance, 0, 0));
                hitDistance = 0;
                break;
            }
        }

        return path;
    }
}
