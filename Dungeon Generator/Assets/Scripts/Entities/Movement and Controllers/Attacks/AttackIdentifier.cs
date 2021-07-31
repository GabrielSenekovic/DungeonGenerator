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

    public bool UpdateCasting() 
    {        
        if(state == CastingState.COMMENCED)
        {
            castTimer+=0.1f;
            if(castTimer >= castTime)
            {
                state = CastingState.DONE;
                castTimer = 0;
                Initialize();
                return true; //Yes you may cast
            }
        }
        return false; //No you may not cast
    }
    public void Attack()
    {
        state = CastingState.COMMENCED;
        //OnAttack(direction, source, collider);
    }

    public virtual void Initialize()
    {

    }

    public virtual void OnFixedUpdate(Vector3 direction, Vector3 source, Collider collider)
    {
        UpdateCasting();
    }
    protected virtual void OnAttack(Vector3 direction, Vector3 source, Collider collider)
    {

    }
    protected virtual void OnAttack(Vector3 direction, Vector3 source, Vector3 origin, float orbitSpeed, Collider collider)
    {

    }
}
