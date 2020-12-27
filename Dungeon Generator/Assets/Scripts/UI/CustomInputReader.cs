using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomInputReader : StandaloneInputModule
{
    public static PointerEventData GetPointerEventData(int pointerId = -1)
    {
        PointerEventData eventData;
        _instance.GetPointerData(pointerId, out eventData, true);
        return eventData;
    }

    private static CustomInputReader _instance;

    protected override void Awake()
    {
        base.Awake();
        _instance = this;
    }
}
