using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : EntityMovementModel
{
    public enum ProjectileInteractionMode
    {
        NONE = 0,
        SUCK = 1,
        TRAP = 2
    }
    public enum ProjectileAccelerationMode
    {
        NONE = 0,
        ACCELERATE = 1,
        DEACCELERATE = 2
    }
    public enum HomingMode
    {
        NONE = 0,
        HOMING = 1,
        REPULSED = 2, //goes in the opposite direction of the target, may hit allies instead because of it, or bounce off walls in unexpected manners
        HOMING_REPULSED = 3 //goes after player and tries to stay at a distance from them
    }
    public float attractionSpeed;
    public float interactionRadius;
    public GameObject currentTarget;
    [SerializeField]ProjectileInteractionMode interactionMode;
    [SerializeField]ProjectileAccelerationMode accelerationMode;
    [SerializeField]HomingMode homingMode;

    [SerializeField]List<GameObject> visuals;

    public int lifeLength;
    int lifeTimer = 0;
    void Start()
    {
        Fric = 0.0f;
        Acc = new Vector2(1,1);
        VisualsRotator.renderers.AddRange(visuals);
    }
    new private void FixedUpdate() 
    {
        currentSpeed = speed;
        lifeTimer++;
        CheckInteractionMode();
        CheckAccelerationMode();
        CheckHomingMode();
        Move();
        if(lifeTimer >= lifeLength)
        {
            Destroy(this.gameObject);
        }
    }
    protected virtual void CheckInteractionMode()
    {
        switch(interactionMode)
        {
            case ProjectileInteractionMode.SUCK:
                //find target
                //float distance = Vector2.Distance(transform.position, playerObject.transform.position)
                /*
                if(distance <= interactionRadius)
                {
                    target.position = Vector2.MoveTowards(targetposition, myposition, speed / distance)
                }
                */
                break;
            case ProjectileInteractionMode.TRAP:
                //find target
                //calculate distance
                /*
                target.position = Vector2.MoveTowards(targetPosition, myposition, speed * distance)
                */
                break;
            default: 
                break;
        }
    }
    protected virtual void CheckAccelerationMode()
    {
        switch(accelerationMode)
        {
            case ProjectileAccelerationMode.ACCELERATE:
                //Speed up projectile overtime
                break;
            case ProjectileAccelerationMode.DEACCELERATE:
                //Slow down projectile overtime
                break;
            default:
                break;
        }
    }
    protected virtual void CheckHomingMode()
    {
        switch(homingMode)
        {
            case HomingMode.HOMING:
                break;
            case HomingMode.REPULSED:
                break;
            case HomingMode.HOMING_REPULSED:
                break;
            default: 
                break;
        }
    }
}
