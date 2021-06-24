using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DunGenes : MonoBehaviour
{
    static DunGenes instance;

    public static DunGenes Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
