using UnityEngine;
using CGT.Pooling;

public class ImpactVFX : MonoBehaviour
{
    public ParticleSystem ps;
    public HS_Poolable poolableScript;
    private void OnEnable()
    {
        ps.Play();
    }

    public void Update()
    {
        poolableScript.RejoinPoolAfter(ps.duration);
    }
}
