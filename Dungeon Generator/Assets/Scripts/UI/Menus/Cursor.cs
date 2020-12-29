using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cursor : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (CustomInputReader.GetPointerEventData().pointerEnter != null &&
                CustomInputReader.GetPointerEventData().pointerEnter.GetComponent<SkillSlot>())
            {
                //Move the clicking here instead
            }
        }
    }
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