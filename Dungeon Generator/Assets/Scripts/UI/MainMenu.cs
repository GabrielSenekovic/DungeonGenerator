using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [System.Serializable]struct ButtonLayout
    {
        [SerializeField] string name;
        [SerializeField] List<GameObject> buttons;
        public void SetActiveAll(bool value)
        {
            foreach(GameObject button in buttons)
            {
                button.SetActive(value);
            }
        }
    }
    [SerializeField] List<ButtonLayout> buttonLayouts;
    //0 == Main Menu
    //1 == Party
    //2 == Equipment
    //3 == Inventory
    //4 == Crafting
    //5 == Abilities
    //6 == Quests
    //7 == Map
    //8 == Bestiary
    //9 == Jukebox
    //10 == Trophies
    //11 == Tutorial
    //12 == Config
    [SerializeField] int currentMenu = 0;
    [SerializeField] int lastMenu = 0;

    public void SwitchMenu(int i)
    {
        buttonLayouts[currentMenu].SetActiveAll(false);
        lastMenu = currentMenu;
        currentMenu = i;
        buttonLayouts[currentMenu].SetActiveAll(true);
    }
    public void Return()
    {
        buttonLayouts[currentMenu].SetActiveAll(false);
        currentMenu = lastMenu;
        buttonLayouts[currentMenu].SetActiveAll(true);
    }
}
