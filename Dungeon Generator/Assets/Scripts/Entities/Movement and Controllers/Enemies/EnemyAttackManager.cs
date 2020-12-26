using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackManager : AttackManager
{
    public AttackIdentifier[] attacks;

    public void Attack(int index, Vector2 direction)
    {
        //attacks[index].Attack(direction, transform.position, GetComponent<Collider>());
    }
}
