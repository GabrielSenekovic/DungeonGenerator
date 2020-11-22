using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Item:ScriptableObject
{
    public enum ItemType
    {
        WeaponItem = 0,
        ArmorItem = 1,
        AccessoryItem = 2,
        IngredientItem = 3, //I have to separate cooked foods from ingredients, since food items are kinda random with their values
        FoodItem = 4,
        PotionItem = 5,
        MaterialItem = 6,
        KeyItem = 7
    }
    public Sprite sprite;
    public List<ItemType> types; //Sometimes an item can be both ingredient and material, for example
    public int size; //determines how much space it takes up in the inventory
}
