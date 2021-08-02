using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAttackModel : AttackModel
{
    public AttackIdentifier[] attacks;
    public int attackTimer = 0;
    public int attackTimerFrequency; //Also known as Agression. How often will you try to attack
    float brutality; //Likelihood of attack

    private void Start() 
    {
        currentAttack = attacks[0];
    }

    public void Attack(Vector2 direction)
    {
        if(attacks.Length == 0){return;}
        attackTimer++;
        if(attackTimer >= attackTimerFrequency && Random.Range(0.0f, 1.0f) < brutality)
        {
            attackTimer = 0;
            int i = Random.Range(0, attacks.Length);
            currentAttack = attacks[i];
            attacks[i].Attack();
        }
    }
}
