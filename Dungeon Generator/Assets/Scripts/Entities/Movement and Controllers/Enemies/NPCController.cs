using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    protected int directionShiftTimer = 0;
    public int directionShiftFrequency;

    protected Vector2[] directions = 
        new Vector2[4] { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
    public enum NPCMovementState
    {
        CHASING = 0,
        ESCAPING = 1,
        IDLE = 2
    }
    NPCMovementState movementState = NPCMovementState.IDLE;

    protected Vector2 movementDirection = Vector2.zero;

    public List<GameObject> visuals;

    private void Awake()
    {
        VisualsRotator.renderers.AddRange(visuals);
    }
    private void Start()
    {
    }
    private void FixedUpdate()
    {
        directionShiftTimer++;
        if (directionShiftTimer == directionShiftFrequency)
        {
            directionShiftTimer = 0;
            movementDirection = directions[Random.Range(0, 4)];
        }
        GetComponent<EntityMovementModel>().Dir = movementDirection;
        GetComponent<EntityMovementModel>().facingDirection = movementDirection;
        GetComponent<EntityMovementModel>().currentSpeed = GetComponent<EntityMovementModel>().speed;
    }
}
