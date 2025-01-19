using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StreamGunClass : GunClass
{
    private GunProxy VP_proxy;
    private GunProxy EXT_proxy;
    public override void EnterWeapon()
    {
        base.EnterWeapon();
        VP_proxy = player.inventory.VP_GetProxy().GetComponent<GunProxy>();
        EXT_proxy = player.inventory.EXT_GetProxy().GetComponent<GunProxy>();
    }

    public override void Weapon_Update()
    {
        base.Weapon_Update();

        if (!player.inventory.isShooting) return;

        if (player.networkInfo._isOwner)
        {

        }
    }
}
