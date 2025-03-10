using UnityEngine;
using CGT.Pooling;

public class BulletVFX : MonoBehaviour
{
    
    public Transform end;
    public float maxTravelPerFrame;
    public ParticleSystem particleSystem;
    public GameObject impactVFX;

    private void OnEnable()
    {
        transform.LookAt(end);
        particleSystem.Play();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, end.position, maxTravelPerFrame);
        if(transform.position == end.position) {
            HS_Poolable impact = HS_PoolableManager.instance.GetInstanceOf(impactVFX.GetComponent<HS_Poolable>());
            impact.transform.position = transform.position;
            impact.gameObject.SetActive(true);
            gameObject.GetComponent<HS_Poolable>().RejoinPool();
        }
    }
}
