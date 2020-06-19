using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageProjectile : DealDamage
{
    protected override void Hit(GameObject target) 
    {
        foreach(Damage damage in damageToDeal)
        {
            target.GetComponent<HealthModel>().TakeDamage(damage);
            Destroy(this.gameObject);
        }
    }
}
