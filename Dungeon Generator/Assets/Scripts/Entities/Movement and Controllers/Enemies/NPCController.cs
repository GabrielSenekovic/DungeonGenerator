using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public int directionShiftTimer = 0;
    public int directionShiftFrequency;
    public enum NPCMovementState
    {
        CHASING = 0,
        ESCAPING = 1,
        IDLE = 2,

        WANDERING = 3
    }
    NPCMovementState movementState = NPCMovementState.IDLE;

    MovementModel movementModel;
    StatusConditionModel statusConditionModel;

    NPCAttackModel attackModel;

    SphereCollider visionCollider;

    Transform target;


    private void Awake()
    {
        movementModel = GetComponent<MovementModel>();
        statusConditionModel = GetComponent<StatusConditionModel>();
        attackModel = GetComponent<NPCAttackModel>();

        visionCollider = gameObject.AddComponent<SphereCollider>();
        visionCollider.radius = 6;
        visionCollider.isTrigger = true;
        
        VisualsRotator.Add(GetComponentInChildren<MeshRenderer>());
    }
    private void Start()
    {
    }
    private void FixedUpdate()
    {
        if(movementState == NPCMovementState.WANDERING)
        {
            if(statusConditionModel.rigid){RigidMovement();}
            else
            {
                Wander();
            }
        }
        else if(movementState == NPCMovementState.CHASING && target)
        {
            movementModel.SetMovementDirection((target.position - transform.position).normalized);
            Attack();
        }
        else if(movementState == NPCMovementState.ESCAPING)
        {
            movementModel.SetMovementDirection((transform.position - target.position).normalized);
        }
    }

    void Attack()
    {
        attackModel.Attack(movementModel.GetFacingDirection());
    }

    void Wander()
    {
        //Walk in one direction, stop, walk in another, stop
        directionShiftTimer++;
        if (directionShiftTimer >= directionShiftFrequency)
        {
            directionShiftTimer = 0;
            movementModel.SetMovementDirection(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized);
        }
        else
        {
            movementModel.SetMovementDirection(movementModel.GetFacingDirection());
        }
    }

    void RigidMovement()
    {
        Vector2[] directions = 
        new Vector2[4] { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };

        directionShiftTimer++;
        if (directionShiftTimer >= directionShiftFrequency)
        {
            directionShiftTimer = 0;
            movementModel.SetMovementDirection(directions[Random.Range(0, 4)]);
        }
        else
        {
            movementModel.SetMovementDirection(movementModel.GetFacingDirection());
        }
    }

    private void OnTriggerStay(Collider other) 
    {
        if(!target && other.CompareTag("Player"))
        {
            movementState = NPCMovementState.CHASING;
            target = other.transform;
        }
    }
    private void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            movementState = NPCMovementState.WANDERING;
            target = null;
        }
    }

    private void OnDrawGizmos() 
    {
        switch(movementState)
        {
            case NPCMovementState.IDLE: Gizmos.color = Color.green; break;
            case NPCMovementState.CHASING: Gizmos.color = Color.red; break;
            case NPCMovementState.ESCAPING: Gizmos.color = Color.yellow; break;
            case NPCMovementState.WANDERING: Gizmos.color = Color.blue; break;
        }
        Gizmos.DrawSphere(transform.position, 1);
    }
}
