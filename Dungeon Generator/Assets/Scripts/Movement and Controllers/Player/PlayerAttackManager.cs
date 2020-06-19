using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackManager : AttackManager
{
    [System.Serializable]public class Attack
    {
        public AttackIdentifier attack;
        public KeyCode key;

    }
    public Attack[] attacks;

    private void Start() 
    {
        currentAttack = attacks[0].attack;
        currentAttack.state = AttackIdentifier.CastingState.DONE;
    }

    private void Update() 
    {
        currentAttack.UpdateCasting();
        if(currentAttack.state == AttackIdentifier.CastingState.DONE)
        {
            for(int i = 0; i < 4; i++)
            {
                if(Input.GetKeyDown(attacks[i].key))
                {
                    currentAttack = attacks[i].attack;
                    currentAttack.Attack(GetComponent<PlayerMovementModel>().facingDirection, transform.position, GetComponent<Collider>());
                }
            }
        }
    }
}