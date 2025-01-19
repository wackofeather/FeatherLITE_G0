using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GeneralManager : NetworkBehaviour
{
    public static GeneralManager instance { get; set; }

    private void Awake()
    {
        ConstructSingleton();

    }

    public virtual void ConstructSingleton()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
}
