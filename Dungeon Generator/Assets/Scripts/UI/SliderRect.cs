using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderRect : MonoBehaviour
{
    // Start is called before the first frame update public class ScrollPosition : $$anonymous$$onoBehaviour {
     public Slider slider;
     public ScrollRect scrollRect;
     // Use this for initialization
     
    public void ChangeScrollPos()
    {
        scrollRect.verticalNormalizedPosition = 1 -slider.value;
    }
}