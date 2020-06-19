using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BipusController : EnemyController
{
    int attackTimer = 0;
    int directionShiftTimer = 0;
    public int attackFrequency;

    float distanceFromBody = 0.5f;

    public int directionShiftFrequency;

    Vector2[] directions = new Vector2[4]{new Vector2(0,1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0)};

    [SerializeField]GameObject laser;
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
           // GetComponent<EnemyMovementModel>().FlipWhenWalkingSideways(movementDirection);
        }
        GetComponent<EnemyMovementModel>().Dir = movementDirection;
        GetComponent<EnemyMovementModel>().facingDirection = movementDirection;
        GetComponent<EnemyMovementModel>().currentSpeed = GetComponent<EnemyMovementModel>().speed;
    }
    void Shoot()
    {
        GetComponentInChildren<Animator>().SetTrigger("Shooting");
        Vector2 direction = GetComponent<EnemyMovementModel>().GetFacingDirection();
        if(direction == new Vector2(1, 0))
        {
            GetComponent<EnemyAttackManager>().Attack(0, new Vector2(1,0));
            /*GameObject newLaser = Instantiate(laser, new Vector2(transform.position.x, transform.position.y + distanceFromBody), Quaternion.identity);
            GameObject newLaser2 = Instantiate(laser, new Vector2(transform.position.x, transform.position.y - distanceFromBody), Quaternion.identity);
            newLaser.GetComponent<EntityMovementModel>().Dir = GetComponent<EnemyMovementModel>().GetFacingDirection();
            newLaser2.GetComponent<EntityMovementModel>().Dir = GetComponent<EnemyMovementModel>().GetFacingDirection();
            newLaser.transform.rotation = Quaternion.Euler(0, 0, 90);
            newLaser2.transform.rotation = Quaternion.Euler(0, 0, 90);*/
        }
        else if(direction == new Vector2(-1, 0))
        {
            GetComponent<EnemyAttackManager>().Attack(0, new Vector2(-1,0));
            /*GameObject newLaser = Instantiate(laser, new Vector2(transform.position.x, transform.position.y + distanceFromBody), Quaternion.identity);
            GameObject newLaser2 = Instantiate(laser, new Vector2(transform.position.x, transform.position.y - distanceFromBody), Quaternion.identity);
            newLaser.GetComponent<EntityMovementModel>().Dir = GetComponent<EnemyMovementModel>().GetFacingDirection();
            newLaser2.GetComponent<EntityMovementModel>().Dir = GetComponent<EnemyMovementModel>().GetFacingDirection();
            newLaser.transform.rotation = Quaternion.Euler(0, 0, 90);
            newLaser2.transform.rotation = Quaternion.Euler(0, 0, 90);*/
        }
        else if(direction == new Vector2(0, 1))
        {
            GetComponent<EnemyAttackManager>().Attack(0, new Vector2(0,1));
           /* GameObject newLaser = Instantiate(laser, new Vector2(transform.position.x + distanceFromBody, transform.position.y), Quaternion.identity);
            GameObject newLaser2 = Instantiate(laser, new Vector2(transform.position.x - distanceFromBody, transform.position.y), Quaternion.identity);
            newLaser.GetComponent<EntityMovementModel>().Dir = GetComponent<EnemyMovementModel>().GetFacingDirection();
            newLaser2.GetComponent<EntityMovementModel>().Dir = GetComponent<EnemyMovementModel>().GetFacingDirection();*/
        }
        else if(direction == new Vector2(0, -1))
        {
            GetComponent<EnemyAttackManager>().Attack(0, new Vector2(0,-1));
           /* GameObject newLaser = Instantiate(laser, new Vector2(transform.position.x + distanceFromBody, transform.position.y), Quaternion.identity);
            GameObject newLaser2 = Instantiate(laser, new Vector2(transform.position.x - distanceFromBody, transform.position.y), Quaternion.identity);
            newLaser.GetComponent<EntityMovementModel>().Dir = GetComponent<EnemyMovementModel>().GetFacingDirection();
            newLaser2.GetComponent<EntityMovementModel>().Dir = GetComponent<EnemyMovementModel>().GetFacingDirection();*/
        }
    }
}
