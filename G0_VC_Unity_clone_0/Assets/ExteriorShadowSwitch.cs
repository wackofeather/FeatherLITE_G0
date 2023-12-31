using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExteriorShadowSwitch : MonoBehaviour
{
    public List<Renderer> renderers = new List<Renderer>();

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>().ToList();
    }
    public void ShadowsOnly(bool boolean)
    {
        if (boolean == true)
        {
            for (int i = 0; i < renderers.Count; i++) 
            {
                renderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }
        else
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }
        
    }
}
