using UnityEngine;
using CGT.Pooling;

public class BulletVFX : MonoBehaviour
{
    
    public Vector3 end;
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
        //Debug.LogWarning(maxTravelPerFrame);
        transform.position = Vector3.MoveTowards(transform.position, end, maxTravelPerFrame);
        if(transform.position == end) {
            Debug.Log("I need sleep");
            HS_Poolable impact = HS_PoolableManager.instance.GetInstanceOf(impactVFX.GetComponent<HS_Poolable>());
            impact.transform.position = transform.position;
            impact.gameObject.SetActive(true);
            gameObject.GetComponent<HS_Poolable>().RejoinPool();
        }
    }
}
