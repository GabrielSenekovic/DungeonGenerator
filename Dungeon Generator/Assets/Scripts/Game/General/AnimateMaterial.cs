using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateMaterial : MonoBehaviour
{
    public Vector2 materialOffset;
    private void Update() 
    {
        GetComponent<MeshRenderer>().sharedMaterial.SetTextureOffset("_BaseMap", materialOffset);
    }
}
