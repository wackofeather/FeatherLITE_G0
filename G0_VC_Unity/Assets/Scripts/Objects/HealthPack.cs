using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private float healthToRestore;

    private Collider objCollider;

    private void OnEnable()
    {
        objCollider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<HealthScript>() != null)
        {
            HealthScript healthScript = collision.gameObject.GetComponent<HealthScript>();
            healthScript.currentHealth += healthToRestore;
            gameObject.SetActive(false);
            Debug.Log("something broke");
        }
    }
}
