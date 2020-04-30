using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementModel : EntityMovementModel
{
    Rigidbody2D myBody;

    Vector2 directionToMove;
    [SerializeField] float speed;

    Vector2 lastDirectionMoved;

    Vector2 directionToPush;
    float pushStrength = 0;
    [SerializeField] float resilience; //How fast the player recovers from being pushed. It functions negatively, the higher the number, the further you fly

    [SerializeField] Vector2 facingDirection;
    bool canMove = true;

    Vector2 constantDirection;

    [SerializeField] bool canAttack;

    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        directionToMove = Vector2.zero; facingDirection = new Vector2(0, -1); constantDirection = new Vector2(0, 0);
        lastDirectionMoved = Vector2.zero;
    }

    public void FixedUpdate()
    {
        Move();
        directionToMove = Vector2.zero;
    }

    public void Move()
    {
        if (canMove)
        {
            myBody.velocity = directionToMove * speed * Time.deltaTime
            + directionToPush * pushStrength * Time.deltaTime
            + constantDirection * speed * Time.deltaTime
            ;
        }
        else
        {
            myBody.velocity = directionToPush * pushStrength * Time.deltaTime
            + constantDirection * speed * Time.deltaTime
            ;
        }
        if (pushStrength > 1)
        {
            RecoverFromPush();
        }
        else if (pushStrength != 0)
        {
            NeutralizePush();
        }
    }
    public void FreezeMovement()
    {
        myBody.velocity = new Vector2(0, 0);
    }
    public void Push(Vector2 direction, float strength)
    {
        directionToPush = direction;
        pushStrength = strength;
    }
    public void RecoverFromPush()
    {
        pushStrength = Mathf.Lerp(0, pushStrength, resilience * Time.deltaTime);
    }
    public void NeutralizePush()
    {
        pushStrength = 0;
        directionToPush = new Vector2(0, 0);
    }
    public void SetDirection(Vector2 direction)
    {
        directionToMove += direction;
        directionToMove.Normalize();

        facingDirection = directionToMove.normalized;
        facingDirection = new Vector2(Mathf.Round(facingDirection.x), Mathf.Round(facingDirection.y));
    }
    public Vector2 GetDirection()
    {
        return directionToMove.normalized;
    }
    public void SetConstantDirection(Vector2 direction)
    {
        constantDirection = direction;
    }
    public Vector2 GetFacingDirection()
    {
        return facingDirection;
    }
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public void SetCanAttack(bool value)
    {
        canAttack = value;
    }
    public bool CanAttack()
    {
        return canAttack;
    }
    public void SetCanMove(bool value)
    {
        canMove = value;
    }
    public void OnDeath()
    {
        Destroy(gameObject);
    }
    public IEnumerator WaitUntilCanAttack(float reloadTime)
    {
        canAttack = false;
        yield return new WaitForSeconds(reloadTime);
        canAttack = true;
    }

}