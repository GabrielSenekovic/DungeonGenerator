using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ResizeQuad : MonoBehaviour
{
    [SerializeField] Vector2 scale;
    void Update()
    {
        Vector2 textureSize = GetTextureSize();
        if (GetDimensions() != textureSize)
        {
            scale.x = scale.x <= 0 ? 1 : scale.x; scale.y = scale.y <= 0 ? 1 : scale.y;
            //1 1 is 16x16
            // 1 * 16 = 16, 2 * 16 = 32, 32/16 = 2
            //So desired dimension / 16 is the required scale
            MeshRenderer rend = GetComponent<MeshRenderer>();
            transform.localScale = textureSize / 16 * scale;
        }
    }
    Vector2 GetDimensions()
    {
        //1 1 is 16x16
        //Debug.Log(transform.localScale * scale * 16);
        return transform.localScale * 16 / scale;
    }
    Vector2 GetTextureSize()
    {
        MeshRenderer rend = GetComponent<MeshRenderer>();
        return new Vector2(rend.sharedMaterial.mainTexture.width, rend.sharedMaterial.mainTexture.height);
    }
}
