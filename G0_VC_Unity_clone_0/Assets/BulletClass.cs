using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletClass : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 1f;
    public virtual void Shoot(Player_Inventory inventory)
    {

    }

    public IEnumerator StandardBulletCoroutine(Player_Inventory inventory)
    {
        Vector3 bulletdirection = new Vector3(inventory.player.xRotation, inventory.player.yRotation, 0);
        float bulletdistance = 0;
        gameObject.transform.position = inventory.GunTip();
        while (true)
        {
            Physics.Raycast(gameObject.transform.position, bulletdirection, out RaycastHit hitInfo, bulletSpeed);
            if ((hitInfo.collider != null)) 
            { 
                bulletdistance += bulletSpeed;
            }
            else
            {
                bulletdistance += hitInfo.distance;
                if (!(hitInfo.collider.gameObject.layer == LayerMask.NameToLayer(inventory.player.EnemyLayer)))
                {
                    //deal damage
                    yield break;
                }
                

                yield break;
            }

        }
        yield break;
    }
}
