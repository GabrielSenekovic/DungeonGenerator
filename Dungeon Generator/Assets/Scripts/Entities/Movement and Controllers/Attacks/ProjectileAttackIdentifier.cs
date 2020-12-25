using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttackIdentifier : AttackIdentifier
{
    [SerializeField]ProjectileController projectile;
    protected override void OnAttack(Vector3 direction, Vector3 source, Collider collider)
    {
        ProjectileController temp = Instantiate(projectile, source, Quaternion.identity);
        SetDirectionAndRotate(ref temp, direction);
        Physics.IgnoreCollision(collider, temp.GetComponent<Collider>());
    }
    protected override void OnAttack(Vector3 direction, Vector3 source, Vector3 origin, float orbitSpeed, Collider collider)
    {
        ProjectileController temp = Instantiate(projectile, source, Quaternion.identity);
        SetDirectionAndRotate(ref temp, direction);
        temp.orbitPoint = origin; temp.orbitSpeed = orbitSpeed;
        Physics.IgnoreCollision(collider, temp.GetComponent<Collider>());
    }
    void SetDirectionAndRotate(ref ProjectileController projectile, Vector3 direction)
    {
        projectile.Dir = direction;
        float angle = Mathf.Asin(direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
