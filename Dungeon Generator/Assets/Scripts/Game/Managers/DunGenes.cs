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

    public bool isStartArea; //Only for debug

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

    private void Start() 
    {
        if(isStartArea)
        {
            FindObjectOfType<LevelGenerator>().GenerateStartArea();
        }
    }
}
