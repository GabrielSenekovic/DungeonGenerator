using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackIdentifier : MonoBehaviour
{
    public enum CastingState
    {
        NONE = 0,
        COMMENCED = 1,
        DONE = 2
    }
    public Sprite icon;
    public float castTime;
    float castTimer;
    [System.NonSerialized] public CastingState state;

    public void UpdateCasting() 
    {        
        if(state == CastingState.COMMENCED)
        {
            castTimer+=0.1f;
            if(castTimer >= castTime)
            {
                state = CastingState.DONE;
                castTimer = 0;
            }
        }
    }
    public void Attack(Vector3 direction, Vector3 source, Collider collider)
    {
        state = CastingState.COMMENCED;
        OnAttack(direction, source, collider);
    }
    protected virtual void OnAttack(Vector3 direction, Vector3 source, Collider collider)
    {

    }
}
