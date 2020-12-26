using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public List<PlayerAttackManager> players;

    public int currentPlayer;

    public InventorySlot[] skillSlots;

    public InventorySlot[] skillListSlots;

    public int selectedSkill;

    //There are 4 skills that can be switched between
    //There are also a list of all viable skills. When one skill is equipped, it is then darkened in the list
    //Therefore, somehow we need to keep track of what skills are currently equipped
    //When a skill is equipped from the list, darken the skill in the list and disable it
    //When a skill is dequipped, find it in the list and reable it
    //Get a sprite for an unequipped slot

    private void Start()
    {
        selectedSkill = -1;
        for(int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i].GetComponentInChildren<Image>().sprite = players[currentPlayer].attacks[i].attack.icon;
            //skills[i] = players[currentPlayer].attacks[i].attack;
        }
    }

    public void SelectSkill(InventorySlot slot)
    {
        if (selectedSkill == -1) { selectedSkill = slot.index; return; }
        else
        {
            AttackIdentifier temp = players[currentPlayer].attacks[slot.index].attack;
            players[currentPlayer].attacks[slot.index].attack = players[currentPlayer].attacks[selectedSkill].attack;
            players[currentPlayer].attacks[selectedSkill].attack = temp;
        }
    }
}
