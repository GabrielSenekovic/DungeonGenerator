using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAttackModel : AttackModel
{
    public AttackIdentifier[] attacks;

    public void Attack(int index, Vector2 direction)
    {
        //attacks[index].Attack(direction, transform.position, GetComponent<Collider>());
    }
}
