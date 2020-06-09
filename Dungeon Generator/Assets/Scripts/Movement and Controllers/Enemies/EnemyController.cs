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
}
