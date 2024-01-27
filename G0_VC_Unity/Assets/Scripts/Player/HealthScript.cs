using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public float currentHealth;
    [SerializeField] private float maxHealth = 100f;

    void Start()
    {
        currentHealth = maxHealth;
        StartCoroutine(DamageOverTime(60));
    }

    void Update()
    {
        
    }

    public IEnumerator DamageOverTime(float totalDamage, float delayBetweenDamageTicks = 1f, float totalTime = 3f)
    {
        // calculates the total amount of ticks from the total time and delay
        float totalAmountOfTicks = totalTime / delayBetweenDamageTicks;

        // calculates the amount of damage to do based on how many ticks and the total damage
        float damagePerTick = totalDamage / totalAmountOfTicks;


        // does the damage per tick for the amount of ticks, waiting the delay between the ticks of damage
        for(int i = 0; i < totalAmountOfTicks; i++)
        {
            currentHealth -= damagePerTick;
            yield return new WaitForSeconds(delayBetweenDamageTicks);
        }
    }
}
