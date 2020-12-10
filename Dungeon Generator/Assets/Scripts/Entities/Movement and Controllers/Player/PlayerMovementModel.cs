using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementModel : EntityMovementModel
{
    private void Start() 
    {
        Fric = 0.0f;
        Acc = new Vector2(1,1);
        Dir = new Vector2(1,0); 
    }

}