using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Flora
{
    Cornflower = 0, //Used for salad, tea
    Dandelion = 1, //Used for salad, tea, jam, wine, coffee, dye
    Poppy = 2, //Used to make sleep potion, and bread
    Meadowseet = 3, //Can be thrown to attract enemies, used for wine, beer, jam, healing potions, tea
    Hellebore = 4, //Can be used to make poison, will remove Berserk but cause Poison if consumed
    Rose = 5, //Used for rose water, can become rosehips which is used for soups. Both are used for jam. Can be made into tea. Can be used to make Diarrhodon, which gives you better buffs for a while.
    Cinnamon = 6, //Can be made to use a spice
    Ginger = 7, //Is an ingredient, makes wine, tea and different pastries
    Oak = 8, //Good for house building, boat building and furniture, also for wine barrels and such
    Maple = 9, //Good for making maple syrup, violins to cellos, and their leaves can be fried
    Aspen = 10, //Good for making paper and matches
    Larch = 11, //Good for making small boats, houses
    Spruce = 12, //Good for building houses, its fresh shoots can be eaten and used as ingredient, can be used for violins to cellos, guitars, pianos and harps
    Apple = 13, //Drops apples, of which can be made juice, pies, sauces, apple butter, salads
    Pineapple = 14, 
    Banana = 15, //Banana fiber can be used in textiles, for example, kimonos, but also paper
    Plantain = 16,
    Coconut = 17, //Inside the tree you can get heart of palm. The name of the fibers is Coir, which can be used for ropes, mats, doormats, brushe and sacks. Coconut leaves are used for baskets and arrows. The wood can be used for bridges and huts
    Pomegranate = 18, //Used for knowledge potions, can be used to make grenadine
    Fig = 19 //Enhances magic stats
}

public class FloraDictionary : MonoBehaviour, ISerializationCallbackReceiver
{
    List<Flora> m_keys = new List<Flora> { };
    [SerializeField] List<GameObject> m_objects;

    Dictionary<Flora, GameObject> ObjectFloraDictionary;

    public void OnBeforeSerialize()
    {
        for(int i = 0; i < 20; i++)
        {
            m_keys.Add((Flora)i);
        }
    }

    public void OnAfterDeserialize()
    {
        for(int i = 0; i < m_objects.Count; i++)
        {
            Debug.Log(m_keys[i], m_objects[i].gameObject);
            ObjectFloraDictionary.Add(m_keys[i], m_objects[i]);
        }
    }
}
