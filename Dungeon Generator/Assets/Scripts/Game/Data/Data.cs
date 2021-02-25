using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseData:ScriptableObject
{
}

public class ItemData:BaseData
{
    public Sprite sprite;
    public ItemData(Sprite sprite_in)
    {
        sprite = sprite_in;
    }
}
