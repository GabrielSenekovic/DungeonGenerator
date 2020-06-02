using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviour
{
    [System.NonSerialized] public SpriteRenderer sprite;
    public TreeGenerator.TreeData.TreeType type;
    private void Start() 
    {
        sprite = GetComponent<SpriteRenderer>();
    }
}
