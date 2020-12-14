using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    public void OnClick(Image sprite)
    {
        if(GetComponentInChildren<Image>().sprite == null)
        {
            GetComponentInChildren<Image>().sprite = sprite.sprite;
            GetComponentInChildren<Image>().color = Color.white;
            GetComponentInChildren<Image>().SetNativeSize();
            sprite.sprite = null; sprite.color = Color.clear;
        }
        else
        {
            if(sprite.sprite == null)
            {
                sprite.sprite = GetComponentInChildren<Image>().sprite;
                sprite.color = Color.white;
                GetComponentInChildren<Image>().sprite = null;
                GetComponentInChildren<Image>().color = Color.clear;
            }
            else
            {
                Sprite temp = GetComponentInChildren<Image>().sprite;
                GetComponentInChildren<Image>().sprite = sprite.sprite;
                GetComponentInChildren<Image>().color = Color.white;
                GetComponentInChildren<Image>().SetNativeSize();
                sprite.sprite = temp; sprite.color = Color.white;
            }
        }
    }
    public void FixedUpdate()
    {
        transform.position = Input.mousePosition;
    }
}
