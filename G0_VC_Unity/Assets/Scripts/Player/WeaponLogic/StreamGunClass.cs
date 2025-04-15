using CGT.Pooling;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu]
public class StreamGunClass : GunClass
{
    public override void Weapon_Update()
    {
        base.Weapon_Update();

        if (player.networkInfo._isOwner)
        {
            if (!inventory.isShooting)
            {
                player.inventory.VP_GetProxy().GetComponent<StreamGunProxy>().bullet.SetActive(false);
            }
        }
    }
    public override async Task<bool> ShootLogic()
    {
        //player.rb.AddForce(player.PlayerCamera.forward * -shootBackForce, ForceMode.Impulse);
        if (!(Vector3.Dot((player.PlayerCamera.forward * -shootBackSpeed).normalized, player.rb.linearVelocity.normalized) > -0.1f && player.rb.linearVelocity.magnitude > shootBackSpeed))
        {
            player.rb.AddForce(player.PlayerCamera.forward * -shootBackSpeed, ForceMode.VelocityChange);
            /* Vector3 shootBackForce = Vector3.ClampMagnitude(((player.PlayerCamera.forward * -shootBackSpeed) - player.rb.linearVelocity) * 0.1f, shootBackSpeed);
             player.rb.AddForce(shootBackForce, ForceMode.VelocityChange);*/
        }
        player.totalRecoil += recoilAmount;

        RaycastHit hit = new RaycastHit();


        player.inventory.VP_GetProxy().GetComponent<GunProxy>().muzzleFlash.Play();

        //if (Physics.Raycast(inventory.VP_GetProxy().GetComponent<GunProxy>().gunTip.transform.position, ))
        if (Physics.Raycast(player.PlayerCamera.transform.position, player.PlayerCamera.TransformDirection(Vector3.forward), out hit, 500f))
        {



            await Task.Yield();
            
            player.inventory.VP_GetProxy().GetComponent<StreamGunProxy>().bullet.SetActive(true);
            Debug.Log("enemy" + player.inventory.VP_GetProxy().GetComponent<StreamGunProxy>().bullet.activeSelf);

            player.inventory.VP_GetProxy().GetComponent<StreamGunProxy>().bullet.transform.position = Vector3.Lerp(player.inventory.VP_GetProxy().GetComponent<StreamGunProxy>().gunTip.transform.position, hit.point, 0.5f); //Random.Range(0.2f, 0.7f));


            if (hit.collider.gameObject.layer == (1 << LayerMask.NameToLayer("ENEMY")))
            {
                Debug.Log("Leatherbys");
                hit.collider.gameObject.GetComponent<PlayerStateMachine>().playerNetwork.DamageRPC(1); Debug.LogAssertion("hit!");
                HS_Poolable bloodSpatter = HS_PoolableManager.instance.GetInstanceOf(gunData.bloodSplatter.GetComponent<HS_Poolable>());
                bloodSpatter.transform.position = hit.point;
                bloodSpatter.transform.rotation = Quaternion.Euler(hit.normal);
                bloodSpatter.gameObject.SetActive(true);
            }
            

            if (hit.collider.gameObject.layer == Mathf.RoundToInt(Mathf.Log(player.enemyMask.value, 2))) { hit.collider.gameObject.GetComponent<PlayerStateMachine>().LocalDamage(1); Debug.LogAssertion("hit!"); }
        }


        

        return true;
    }

    public virtual async Task<bool> DummyShoot()
    {
        return true;
    }
}
