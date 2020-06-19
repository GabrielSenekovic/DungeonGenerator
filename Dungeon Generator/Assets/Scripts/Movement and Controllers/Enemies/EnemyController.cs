using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyMovementState
    {
        CHASING = 0,
        ESCAPING = 1,
        IDLE = 2
    }
    EnemyMovementState movementState = EnemyMovementState.IDLE;

    protected Vector2 movementDirection = Vector2.zero;

    public List<GameObject> visuals;

    private void Awake()
    {
        VisualsRotator.renderers.AddRange(visuals);
    }
}
