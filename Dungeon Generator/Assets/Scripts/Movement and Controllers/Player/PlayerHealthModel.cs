using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthModel : MonoBehaviour
{
    public float currentHealth;
    public int maxHealth;

    private void Awake() 
    {
        currentHealth = maxHealth;
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            currentHealth--;
        }
    }
    public void TakeDamage(DealDamage.Damage damage)
    {
        currentHealth -= damage.damage;
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
