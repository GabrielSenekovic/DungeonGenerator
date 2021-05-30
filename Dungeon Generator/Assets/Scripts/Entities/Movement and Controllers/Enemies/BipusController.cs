using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BipusController : NPCController
{
    int attackTimer = 0;
    public int attackFrequency;

    private void Update()
    {
       //GetComponentInChildren<Animator>().SetFloat("DirectionX", GetComponent<EnemyMovementModel>().GetRelativeFacingDirection().x);
        //GetComponentInChildren<Animator>().SetFloat("DirectionY", GetComponent<EnemyMovementModel>().GetRelativeFacingDirection().y);
    }
    private void FixedUpdate() 
    {
        attackTimer++;
        if(attackTimer == attackFrequency){Attack();}
        directionShiftTimer++;
        if(directionShiftTimer == directionShiftFrequency){ ChangeDirection(); }

        //GetComponent<EnemyMovementModel>().Dir = movementDirection;
        //GetComponent<EnemyMovementModel>().currentSpeed = GetComponent<EnemyMovementModel>().speed;
    }
    void Shoot()
    {
        GetComponentInChildren<Animator>().SetTrigger("Shooting");
       // GetComponent<EnemyAttackManager>().Attack(0, GetComponent<EnemyMovementModel>().GetFacingDirection());
    }
    void Attack()
    {
        attackTimer = 0;
        Shoot();
       // movementDirection = Vector2.zero;
        directionShiftTimer = directionShiftFrequency - 20;
    }
    void ChangeDirection()
    {
        directionShiftTimer = 0;
        //movementDirection = directions[Random.Range(0, 4)];
       // GetComponent<EnemyMovementModel>().facingDirection = movementDirection;
    }
}
