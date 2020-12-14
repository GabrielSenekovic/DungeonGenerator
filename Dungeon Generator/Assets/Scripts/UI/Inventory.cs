using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] Transform inventoryGrid;
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    List<Item> inventorySlots_Item = new List<Item>();
    [SerializeField] ItemGenerator itemGenerator;
    //Either show them as a continual list with a weight value, or visually as differently sized boxes

    int selectedSlot = -1;

    void Start()
    {
        inventorySlots_Item.Add(null);
        for (int i = 0; i < 30-1; i++)
        {
            inventorySlots.Add(Instantiate(inventorySlots[0], inventoryGrid));
            inventorySlots[i].index += i;
            inventorySlots_Item.Add(null);
        }
        if(DebuggingTools.fillInventoryWithRandomItems)
        {
            for(int i = 0; i < inventorySlots.Count; i++)
            {
                Sprite temp = itemGenerator.GenerateItemSprite();
                if (temp == null) { return; }
                AddItem(GenerateRandomItem(temp));
            }
        }
    }
    public Item GenerateRandomItem(Sprite sprite)
    {
        Item temp = new Item();
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
                inventorySlots[i].GetComponentInChildren<Image>().sprite = item.sprite;
                return;
            }
        }
    }

    public void SelectItem(InventorySlot slot)
    {
        if(selectedSlot == -1) { selectedSlot = slot.index; return; }
        else
        {
            Item temp = inventorySlots_Item[slot.index];
            inventorySlots_Item[slot.index] = inventorySlots_Item[selectedSlot];
            inventorySlots_Item[selectedSlot] = temp;
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
