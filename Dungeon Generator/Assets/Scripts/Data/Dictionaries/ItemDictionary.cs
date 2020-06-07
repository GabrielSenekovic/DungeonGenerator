using UnityEngine;
using System.Collections;

public enum ItemType //Determines the background color of the icon
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

public class ItemDictionary : MonoBehaviour
{
}
