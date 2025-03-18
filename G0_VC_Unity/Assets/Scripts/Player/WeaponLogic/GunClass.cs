using CGT.Pooling;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[CreateAssetMenu]
public class GunClass : WeaponClass
{
    public GunData gunData;
    [HideInInspector] float maxAmmo_Mag;
    [SerializeField] GameObject bullet;
    [SerializeField] float bulletSpeed;
    

    public override void Weapon_Init()
    {
        base.Weapon_Init();
        currentAmmo = weaponData.maxAmmo_Inventory;
    }
    public override void EnterWeapon()
    {
        base.EnterWeapon();

    }


    public override void Weapon_Update()
    {
        //Debug.Log(player.inventory.isScoping);
        //if (!player.networkInfo._isOwner) Debug.LogWarning("Blah");
        player.player_EXT_ARM_anim_controller.SetBool("Scoping", inventory.isScoping);

        player.player_EXT_ARM_anim_controller.SetBool("Firing", inventory.isShooting);

        player.player_EXT_ARM_anim_controller.SetBool("Reloading", inventory.isReloading);

        inventory.EXT_GetCurrentWeaponAnimator().SetBool("Scoping", inventory.isScoping);

        inventory.EXT_GetCurrentWeaponAnimator().SetBool("Firing", inventory.isShooting);

        inventory.EXT_GetCurrentWeaponAnimator().SetBool("Reloading", inventory.isReloading);

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

        player.player_VP_ARM_anim_controller.SetBool("Reloading", inventory.isReloading);

        inventory.VP_GetCurrentWeaponAnimator().SetBool("Scoping", inventory.isScoping);

        inventory.VP_GetCurrentWeaponAnimator().SetBool("Firing", inventory.isShooting);

        inventory.VP_GetCurrentWeaponAnimator().SetBool("Reloading", inventory.isReloading);

        if (player.inventory.isReloading) { return; }


        if (weaponData.fireInput.action.IsPressed() && !player.isMelee && !inventory.isShooting) shootCoroutine();


        if (player.inventory.isReloading) { return; }

        if (weaponData.scope.action.IsPressed() && !player.isMelee) StartScope();
        else StopScope();
    }


    public void StartScope() { Debug.Log("scoping"); inventory.isScoping = true; }
    public void StopScope() { inventory.isScoping = false; }

    public virtual async void shootCoroutine()
    {

        if (currentAmmo <= 0)
        {
            Reload();
            return;
        }
/*        if (player.IsOwner) 
    {*/
        inventory.isShooting = true;
        while (true)
        {
            if (currentAmmo <= 0) break;
            if (!weaponData.fireInput.action.IsPressed()) break;
            if (player.isMelee) break;

            ShootLogic();

            currentAmmo -= 1;

            if (player.playerNetwork != null) player.playerNetwork.DummyShootRPC();
            //Debug.LogWarning(player);
            //yield return new WaitForSeconds(1 / weaponData.BPS);
            await Task.Delay((int)(1/weaponData.BPS * 1000));
        }
        inventory.isShooting = false;
        var main = player.inventory.VP_GetProxy().GetComponent<GunProxy>().muzzleFlash.main;
        main.loop = false;
        // This just doesn't work for some reason muzzleFlash.main.loop = true;
        ///yield break;
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
    public virtual async Task<bool> ShootLogic()
    {
        RaycastHit hit = new RaycastHit();
        //if (Physics.Raycast(inventory.VP_GetProxy().GetComponent<GunProxy>().gunTip.transform.position, ))
        if (Physics.Raycast(player.PlayerCamera.transform.position, player.PlayerCamera.TransformDirection(Vector3.forward), out hit, 500f))
        {
            HS_Poolable toShoot = HS_PoolableManager.instance.GetInstanceOf(gunData.bulletFX.GetComponent<HS_Poolable>());
            toShoot.transform.position = player.inventory.VP_GetProxy().GetComponent<GunProxy>().gunTip.transform.position;
            Debug.Log("Pre-Pre-Pre-Chipotle");
            toShoot.transform.rotation = player.inventory.VP_GetProxy().GetComponent<GunProxy>().gunTip.transform.rotation;
            Debug.Log("Pre-Pre-Chipotle");
            toShoot.gameObject.GetComponent<BulletVFX>().maxTravelPerFrame = bulletSpeed;
            toShoot.gameObject.GetComponent<BulletVFX>().end = hit.point;
            Debug.Log("Pre-Chipotle");
            toShoot.gameObject.SetActive(true);
            Debug.Log("Chipotle");

            player.inventory.VP_GetProxy().GetComponent<GunProxy>().muzzleFlash.Play();

            if (hit.collider.gameObject.layer == (1 << LayerMask.NameToLayer("ENEMY"))) { hit.collider.gameObject.GetComponent<PlayerStateMachine>().playerNetwork.DamageRPC(1); Debug.LogAssertion("hit!"); }
        }

        return true;
    }

    public virtual async Task<bool> DummyShoot()
    {
        return true;
    }
    public virtual async Task Reload()
    {
        Debug.LogWarning("reloading");

        player.inventory.isScoping = false;


        player.inventory.isReloading = true;
        
        await Task.Delay(Mathf.FloorToInt(weaponData.reloadTime * 1000) + 1);
        currentAmmo = weaponData.maxAmmo_Inventory;
        player.inventory.isReloading = false;
    }
}
