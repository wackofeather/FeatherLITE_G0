using CGT.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[CreateAssetMenu]
public class GunClass : WeaponClass
{
    public GunData gunData;
    [HideInInspector] float maxAmmo_Mag;
    [SerializeField] GameObject bullet;

    public override void EnterWeapon()
    {
        base.EnterWeapon();

    }


    public override void Weapon_Update()
    {

        //if (!player.networkInfo._isOwner) Debug.LogWarning("Blah");
        player.player_EXT_ARM_anim_controller.SetBool("Scoping", inventory.isScoping);

        player.player_EXT_ARM_anim_controller.SetBool("Firing", inventory.isShooting);

        inventory.EXT_GetCurrentWeaponAnimator().SetBool("Scoping", inventory.isScoping);

        inventory.EXT_GetCurrentWeaponAnimator().SetBool("Firing", inventory.isShooting);

        if (!player.networkInfo._isOwner)
        {
            if (inventory.isShooting)
            {
                //do ext shooting here?
            }
            return;
        }


        base.Weapon_Update();

        player.player_VP_ARM_anim_controller.SetBool("Scoping", inventory.isScoping);

        player.player_VP_ARM_anim_controller.SetBool("Firing", inventory.isShooting);

        inventory.VP_GetCurrentWeaponAnimator().SetBool("Scoping", inventory.isScoping);

        inventory.VP_GetCurrentWeaponAnimator().SetBool("Firing", inventory.isShooting);


        if (shootingTimer > 0) return;




        if (weaponData.fireInput.action.IsPressed() && !player.isMelee && !inventory.isShooting) inventory.StartCoroutine(shootCoroutine());


        if (weaponData.scope.action.IsPressed() && !player.isMelee) StartScope();
        else StopScope();
    }


    public void StartScope() { inventory.isScoping = true; }
    public void StopScope() { inventory.isScoping = false; }

    public virtual IEnumerator shootCoroutine()
    {
/*        if (player.IsOwner) 
        {*/
            inventory.isShooting = true;
            while (true)
            {
                if (!weaponData.fireInput.action.IsPressed()) break;
                if (player.isMelee) break;
                RaycastHit hit = new RaycastHit();
                //if (Physics.Raycast(inventory.VP_GetProxy().GetComponent<GunProxy>().gunTip.transform.position, ))
                if (Physics.Raycast(player.PlayerCamera.transform.position, player.PlayerCamera.TransformDirection(Vector3.forward), out hit, 500f))
                {
                    HS_Poolable toShoot = HS_PoolableManager.instance.GetInstanceOf(gunData.bulletFX.GetComponent<HS_Poolable>());
                    toShoot.transform.position = player.inventory.VP_GetProxy().GetComponent<GunProxy>().gunTip.transform.position;
                    toShoot.transform.rotation = player.inventory.VP_GetProxy().GetComponent<GunProxy>().gunTip.transform.rotation;
                    toShoot.gameObject.SetActive(true);

                    if (hit.collider.gameObject.layer == (1 << LayerMask.NameToLayer("ENEMY"))) { hit.collider.gameObject.GetComponent<PlayerStateMachine>().playerNetwork.DamageRPC(1); Debug.LogAssertion("hit!"); }
                }


            //Debug.LogWarning(player);
            yield return new WaitForSeconds(1 / weaponData.BPS);
            }
            inventory.isShooting = false;
            yield break;
/*        }*/
/*        else
        {
            while (inventory.isShooting == true)
            {
                yield return new WaitForSeconds(1 / weaponData.BPS);
            }
            yield break;
*//*            while (true)
            {
                if (!weaponData.fireInput.action.IsPressed()) break;
                if (player.isMelee) break;

                //if (Physics.Raycast(inventory.VP_GetProxy().transform.position, ))

                yield return new WaitForSeconds(1 / weaponData.BPS);
            }
            inventory.isShooting = false;
            yield break;*//*
        }*/
        
    }
}
