using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    public enum DamageType
    {
        NONE = 0,
        BLUNT = 1,
        SLASHING = 2,
        PIERCING = 3,
        MAGIC = 4
    }
    public enum Element
    {
        NONE = 0,
        WATER = 1,
        FIRE = 2,
        EARTH = 3,
        AIR = 4,
        LIGHT = 5,
        DARK = 6,

        AETHER = 7
    }
    [System.Serializable]public struct Damage
    {
        public DamageType type;
        public Element element;
        public int damage;
    }
    public List<Damage> damageToDeal;

    void OnTriggerEnter(Collider other) 
    {
        if(other.GetComponent<HealthModel>())
        {
            Hit(other.gameObject);
        }
        else
        {
            OnImpact();
        }
    }
    protected virtual void Hit(GameObject target)
    {
        foreach(Damage damage in damageToDeal)
        {
            target.GetComponent<HealthModel>().TakeDamage(damage);
        }
    }
    protected virtual void OnImpact()
    {
    }
}
