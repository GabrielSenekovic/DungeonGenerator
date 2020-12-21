using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttackIdentifier : AttackIdentifier
{
    [SerializeField]ProjectileController projectile;
    protected override void OnAttack(Vector3 direction, Vector3 source, Collider collider)
    {
        ProjectileController temp = Instantiate(projectile, source, Quaternion.identity);
        temp.Dir = direction;
        Physics.IgnoreCollision(collider, temp.GetComponent<Collider>());
    }
    protected override void OnAttack(Vector3 direction, Vector3 source, Vector3 origin, float orbitSpeed, Collider collider)
    {
        ProjectileController temp = Instantiate(projectile, source, Quaternion.identity);
        temp.Dir = direction;
        temp.orbitPoint = origin;
        temp.orbitSpeed = orbitSpeed;
        Physics.IgnoreCollision(collider, temp.GetComponent<Collider>());
    }
}
