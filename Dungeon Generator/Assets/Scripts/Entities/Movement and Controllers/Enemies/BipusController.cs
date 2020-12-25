using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BipusController : NPCController
{
    int attackTimer = 0;
    public int attackFrequency;
    private void FixedUpdate() 
    {
        attackTimer++;
        if(attackTimer == attackFrequency)
        {
            attackTimer = 0;
            Shoot();
            movementDirection = Vector2.zero;
            directionShiftTimer = directionShiftFrequency - 20;
        }
        directionShiftTimer++;
        if(directionShiftTimer == directionShiftFrequency)
        {
            directionShiftTimer = 0;
            movementDirection = directions[Random.Range(0, 4)];
            GetComponentInChildren<Animator>().SetFloat("DirectionX", movementDirection.x);
            GetComponentInChildren<Animator>().SetFloat("DirectionY", movementDirection.y);
        }
        GetComponent<EnemyMovementModel>().Dir = movementDirection;
        GetComponent<EnemyMovementModel>().facingDirection = movementDirection;
        GetComponent<EnemyMovementModel>().currentSpeed = GetComponent<EnemyMovementModel>().speed;
    }
    void Shoot()
    {
        GetComponentInChildren<Animator>().SetTrigger("Shooting");
        GetComponent<EnemyAttackManager>().Attack(0, GetComponent<EnemyMovementModel>().GetFacingDirection());
    }
}
