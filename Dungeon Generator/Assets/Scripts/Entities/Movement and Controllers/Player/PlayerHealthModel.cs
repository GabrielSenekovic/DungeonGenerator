using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthModel : HealthModel
{
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(1);
        }
    }
}
