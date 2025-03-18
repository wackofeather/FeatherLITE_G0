using UnityEngine;
using CGT.Pooling;

public class BulletVFXSpawner : MonoBehaviour
{
    public GameObject bullet;
    public Transform start;
    public Vector3 end;
    public float maxTravelPerFrame;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HS_Poolable projectile = HS_PoolableManager.instance.GetInstanceOf(bullet.GetComponent<HS_Poolable>());
            projectile.transform.position = start.position;
            BulletVFX bulletVFX = projectile.GetComponent<BulletVFX>();
            bulletVFX.end = end;
            bulletVFX.maxTravelPerFrame = maxTravelPerFrame;
            projectile.gameObject.SetActive(true);
        }
    }
}
