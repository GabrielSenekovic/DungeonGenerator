using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotationSpeed;
    private void FixedUpdate() 
    {
        transform.Rotate(0,0, rotationSpeed);
    }
}
