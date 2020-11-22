using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public enum InventoryState
    {
        All = 0,
        Items = 1,
        Weaponry = 2,
        Armory = 3,
        Accesories = 4,
        Key_Items = 5
    }
    [SerializeField] InventoryState state;
    bool list = false;
    [SerializeField] Sprite emptySlot;
    [SerializeField] GameObject inventorySlot;
    [SerializeField] Transform inventoryGrid;
    List<GameObject> inventorySlots = new List<GameObject>();
    List<Item> inventorySlots_Item = new List<Item>();
    //Either show them as a continual list with a weight value, or visually as differently sized boxes

    void Start()
    {
        for(int i = 0; i < 30; i++)
        {
            inventorySlots.Add(Instantiate(inventorySlot, inventoryGrid));
            inventorySlots_Item.Add(null);
        }
        if(DebuggingTools.fillInventoryWithRandomItems)
        {
            Sprite temp = ItemGenerator.GenerateItemSprite();
            if(temp == null) { return; }
            AddItem(GenerateRandomItem(temp));
        }
    }
    public Item GenerateRandomItem(Sprite sprite)
    {
        Item temp = null;
        temp.types.Add(Item.ItemType.IngredientItem);
        temp.sprite = sprite;
        temp.size = 1;

        return temp;
    }
    public void AddItem(Item item)
    {
        for(int i = 0; i < inventorySlots.Count; i++)
        {
            if(inventorySlots_Item[i] == null)
            {
                inventorySlots_Item[i] = item;
                return;
            }
        }
    }
    public void ChangeState(int value)
    {
        state = (InventoryState)(value % 6);
    }
    //Compressing only works in not-list mode
    public void Compress()
    {
        //Pushes items together so you can visually "free up space"
    }
    //Sorting only works in list mode
    public void SortByName()
    {

    }
    public void SortByTypeAndName()
    {
        //Sort by type, and then within those types, sort by name
    }
}
