using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

public class EntityMovementModel : MovementModel
{
    public Rigidbody rig() {return this.GetComponent<Rigidbody>();}
    [System.NonSerialized]public List<Vector2> push = new List<Vector2>();
    [System.NonSerialized]public Vector2 directionToMove;
    public Vector2 facingDirection;
    [System.NonSerialized]public float currentSpeed;
    public float speed;
    public bool canMove = true;
    Vector2 dir;

    public Vector2 orbitPoint; //Anything that can move could orbit around something at some point
    public float orbitSpeed;

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
    protected float Fric = 0.1f; //this value chosen because it worked well with explosion pushes

    Animator anim;

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
        anim = GetComponentInChildren<Animator>();
    }
    private void Update() 
    {
        Vector2 facingDirection = GetRelativeFacingDirection();
        if(Mathf.Abs(facingDirection.x) > Mathf.Abs(facingDirection.y))
        {
            facingDirection.y = 0;
        }
        else
        {
            facingDirection.x = 0;
        };
        facingDirection.x = Mathf.RoundToInt(facingDirection.x); facingDirection.y = Mathf.RoundToInt(facingDirection.y);
        if(anim != null)
        {
            anim.SetFloat("DirectionX", facingDirection.x);
            anim.SetFloat("DirectionY", facingDirection.y);
        }
    }
    public void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        Vector2 buffer = Vector2.zero;
        for(int i = 0; i < push.Count; i++)
        {
            buffer += push[i];
            push[i] *= (1.0f / (1.0f + Fric));
        }
            //Gravity pushes should be reset all the time, but for example explosions wont. Therefore, it should be a for loop
       /* foreach( Vector2 v in push)
        {
            buffer += v;
        }*/
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
        if(orbitSpeed != 0)
        {
            transform.RotateAround(orbitPoint, Vector3.forward, orbitSpeed);
            Dir = Quaternion.AngleAxis(orbitSpeed, Vector3.forward) * Dir;
        }
    }//Courtesy of Casper Gustavsson

    public Vector2 GetFacingDirection()
    {
        return facingDirection;
    }
    public Vector2 GetRelativeFacingDirection()
    {
        return Quaternion.Euler(0,0,-CameraMovement.rotationSideways) * facingDirection;
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
