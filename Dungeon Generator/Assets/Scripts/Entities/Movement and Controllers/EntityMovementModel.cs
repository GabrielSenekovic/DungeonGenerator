using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovementModel : MovementModel
{
    public Rigidbody rig() {return this.GetComponent<Rigidbody>();}
    [System.NonSerialized]public List<Vector2> push = new List<Vector2>();
    [System.NonSerialized]public Vector2 directionToMove;
    [System.NonSerialized]public Vector2 facingDirection;
    [System.NonSerialized]public float currentSpeed;
    public float speed;
    public bool canMove = true;
    Vector2 dir;

    public Vector2 Dir
    {
        get
        {
            dir = dir.normalized;
            return dir;
        }
        set
        {
            dir = value.normalized;
        }
    }
    Vector2 vel;
    
    public Vector2 Vel
    {
        get
        {
            vel = currentSpeed * Dir;
            return vel; 
        }
        set 
        {
            vel = value;
            currentSpeed = vel.magnitude;
            Dir = vel.normalized; 
        }
    }
    [System.NonSerialized]public Vector2 Acc;
    protected float Fric;

    public void AddVelocity(Vector2 vin)
    {
        Vel += vin;
    }

    public int AddPushVector(Vector2 vin)
    {
        push.Add(vin);
        return push.Count - 1;
    }

    public void RemovePushVector(int index)
    {
        push.RemoveAt(index);
    }
    void Awake()
    {
        directionToMove = Vector2.zero; facingDirection = new Vector2(0, -1);
    }
    public void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        Vector2 buffer = Vector2.zero;
        foreach( Vector2 v in push)
        {
            buffer += v;
        }
        if(canMove)
        { 
            rig().velocity = Vel + buffer;
            Vel *= Acc;
            Vel *= (1.0f/(1.0f + Fric));
        }
        else
        {
            rig().velocity = buffer;
        }
    }//Courtesy of Casper Gustavsson

    public Vector2 GetFacingDirection()
    {
        return facingDirection;
    }
    public void OnDeath()
    {
        Destroy(gameObject);
    }
    public void FlipWhenWalkingSideways(Vector2 movementDirection)
    {
        if(movementDirection == new Vector2(-1, 0))
        {
            transform.localScale = new Vector2(1, 1);
        }
        else if(movementDirection == new Vector2(1, 0))
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }
}
