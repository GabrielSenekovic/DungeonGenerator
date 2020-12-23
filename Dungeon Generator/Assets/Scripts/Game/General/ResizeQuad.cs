using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ResizeQuad : MonoBehaviour
{
    void Update()
    {
        Vector2 textureSize = GetTextureSize();
        if (GetDimensions() != textureSize)
        {
            //1 1 is 16x16
            // 1 * 16 = 16, 2 * 16 = 32, 32/16 = 2
            //So desired dimension / 16 is the required scale
            MeshRenderer rend = GetComponent<MeshRenderer>();
            transform.localScale = textureSize / 16;
        }
    }
    Vector2 GetDimensions()
    {
        //1 1 is 16x16
        return transform.localScale * 16;
    }
    Vector2 GetTextureSize()
    {
        MeshRenderer rend = GetComponent<MeshRenderer>();
        return new Vector2(rend.sharedMaterial.mainTexture.width, rend.sharedMaterial.mainTexture.height);
    }
}
