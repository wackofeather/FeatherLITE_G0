using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GunData : ScriptableObject
{

    [SerializeField] float maxAmmo_Inventory;
    [Space]
    [SerializeField] float BPS;
    public float bulletSpeed;
    public GameObject bulletFX;
    public GameObject bloodSplatter;

   /* public class GunLogic : WeaponLogic
    {
        float currentAmmo;
        GunData gunData;


        public GunLogic(GunData data)
        {
            gunData = data;
        }


        public override void Weapon_Update(PlayerStateMachine player, Player_Inventory inventory)
        {
            base.Weapon_Update(player, inventory);
            Debug.Log("ahhhhhhhhhhhh");
            if (gunData.fireInput.action.IsPressed() && !player.isMelee) isShooting = true;
            else isShooting = false;

            if (gunData.scope.action.IsPressed() && !player.isMelee) StartScope();
            else StopScope();
            //Debug.Log(isShooting);

            player.player_VP_anim_controller.SetBool("Scoping", isScoping);

            player.player_VP_anim_controller.SetBool("Firing", isShooting);
        }


        public void StartScope() { isScoping = true; }
        public void StopScope() { isScoping = false; }


    }

    public override IWeaponable GetClass()
    {
        return new GunLogic(this);
    }*/
}
