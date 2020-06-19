using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthModel : MonoBehaviour
{
    public float currentHealth;
    public int maxHealth;
    private void Awake() 
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(DealDamage.Damage damage)
    {
        if(currentHealth - damage.damage <= 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            currentHealth -= damage.damage;
        }
    }
    public float GetHealthPercentage()
    {
        return currentHealth / (float)maxHealth;
    }
    public float GetHealthPercentage(float modifier)
    {
        return (currentHealth + modifier) / (float)maxHealth;
    }
}
