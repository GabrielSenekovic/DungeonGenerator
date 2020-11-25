﻿using System.Collections;
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
}