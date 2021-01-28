using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    [System.Serializable]
    public struct SelectionData
    {
        public int selectedSkill;
        public bool fromList;
        public SelectionData(int selectedSkill_in, bool fromList_in)
        {
            selectedSkill = selectedSkill_in;
            fromList = fromList_in;
        }
    }
    public List<PlayerAttackManager> players;

    public int currentPlayer;

    public SkillSlot[] skillSlots;

    public List<SkillSlot> skillListSlots;

    [SerializeField] Transform skillGrid;

    [SerializeField] SkillSlot skillSlotPrefab;

    [SerializeField] Sprite emptySlot;

    public SelectionData selection;

    //There are 4 skills that can be switched between
    //There are also a list of all viable skills. When one skill is equipped, it is then darkened in the list
    //Therefore, somehow we need to keep track of what skills are currently equipped
    //When a skill is equipped from the list, darken the skill in the list and disable it
    //When a skill is dequipped, find it in the list and reable it
    //Get a sprite for an unequipped slot

    private void Start()
    {
        selection = new SelectionData(-1, false);
        for(int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i].GetComponentInChildren<Image>().sprite = players[currentPlayer].attacks[i].attack.icon;
            skillSlots[i].attack = players[currentPlayer].attacks[i].attack;
        }

        for (int i = 0; i < GetComponent<SkillLibrary>().attacks.Count; i++)
        {
            skillListSlots.Add(Instantiate(skillSlotPrefab, skillGrid));
            skillListSlots[i].index += i;
            skillListSlots[i].attack = GetComponent<SkillLibrary>().attacks[i];
            skillListSlots[i].GetComponentInChildren<Image>().sprite = GetComponent<SkillLibrary>().attacks[i].icon;
            CheckIfEquipped(i);
        }
    }
    void CheckIfEquipped(int index)
    {
        for (int j = 0; j < 4; j++)
        {
            if (players[currentPlayer].attacks[j].attack.name == (skillListSlots[index] as SkillSlot).attack.name)
            {
                skillListSlots[index].GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
                skillListSlots[index].state = SkillSlot.EquipState.EQUIPPED_LIST;
            }
        }
    }
    private void Update()
    {
        for(int i = 0; i < skillSlots.Length; i++)
        {
            if(skillSlots[i].state == SkillSlot.EquipState.WAITING)
            {
                if(i == selection.selectedSkill)
                {
                    Deselect(i);
                    selection.selectedSkill = -1;
                }
                UnEquip(skillSlots[i]);
            }
            else if(skillSlots[i].selectState == SkillSlot.SelectState.WAITING)
            {
                SelectSkill(skillSlots[i]);
            }
        }
        for(int i = 0; i < skillListSlots.Count; i++)
        {
            if(skillListSlots[i].selectState == SkillSlot.SelectState.WAITING)
            {
                SelectSkill(skillListSlots[i]);
            }
        }
    }

    void UnEquip(SkillSlot slot)
    {
        players[currentPlayer].attacks[slot.index].attack = null;
        for (int i = 0; i < skillListSlots.Count; i++)
        {
            if (slot.attack.name == skillListSlots[i].attack.name)
            {
                skillListSlots[i].GetComponent<Image>().color = Color.white;
                skillListSlots[i].state = SkillSlot.EquipState.SKILL;
                slot.state = SkillSlot.EquipState.NONE;
                slot.GetComponent<Image>().sprite = emptySlot;
            }
        }
    }

    public void SelectSkill(SkillSlot slot)
    {
        if (slot.state == SkillSlot.EquipState.EQUIPPED)
        {
            //If you have selected a skill from the equipped skill
            if (selection.selectedSkill == -1 && selection.fromList == false) { OnSelect(slot.index); return; }
            //The above checks if you have not selected anything yet
            else if(selection.fromList == false)
            {
                //If you have already selected something from your equipped skills
                AttackIdentifier temp = players[currentPlayer].attacks[slot.index].attack;
                players[currentPlayer].attacks[slot.index].attack = players[currentPlayer].attacks[selection.selectedSkill].attack;
                players[currentPlayer].attacks[selection.selectedSkill].attack = temp;
                slot.attack = players[currentPlayer].attacks[slot.index].attack;
                slot.GetComponent<Image>().sprite = slot.attack.icon;
                skillSlots[selection.selectedSkill].attack = players[currentPlayer].attacks[selection.selectedSkill].attack;
                skillSlots[selection.selectedSkill].GetComponent<Image>().sprite = skillSlots[selection.selectedSkill].attack.icon;
                Deselect(slot.index); //Deselect
                Deselect(selection.selectedSkill);
                selection.selectedSkill = -1;
            }
            else
            {
                //If you have already selected something in the skill list
                //Make it dark in the skill list, and make the skill from the equip bright
                //Replace the sprite and attack in the game
                skillListSlots[selection.selectedSkill].GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
                skillListSlots[selection.selectedSkill].state = SkillSlot.EquipState.EQUIPPED_LIST;
                for (int i = 0; i < skillListSlots.Count; i++)
                {
                    if (slot.attack.name == skillListSlots[i].attack.name)
                    {
                        skillListSlots[i].UnEquip();
                    }
                }
                players[currentPlayer].attacks[slot.index].attack = skillListSlots[selection.selectedSkill].attack;
                slot.attack = skillListSlots[selection.selectedSkill].attack;
                slot.GetComponent<Image>().sprite = players[currentPlayer].attacks[slot.index].attack.icon;
                selection.selectedSkill = -1; selection.fromList = false;
                Deselect(slot.index);
            }
        }
        if(slot.state == SkillSlot.EquipState.SKILL)
        {
            //If you have selected a skill from the skill list

            if (selection.selectedSkill != -1 && selection.fromList == false) 
            { 
                //If you already have a skill from the equipped skills selected, deselect it
                Deselect(selection.selectedSkill); selection.selectedSkill = -1; 
            }
            else if(selection.selectedSkill != -1 && selection.fromList == true) 
            {
                //If you already have a skill from the list selected, deselect it
                skillListSlots[selection.selectedSkill].UnEquip();
                skillListSlots[selection.selectedSkill].selectState = SkillSlot.SelectState.NONE;
                selection.selectedSkill = -1;
            }
            else if(selection.selectedSkill == -1)
            {
                //If you have nothing selected, select the slot
                selection.selectedSkill = slot.index;
                selection.fromList = true;
                skillListSlots[slot.index].GetComponent<Image>().color = Color.blue;
                skillListSlots[slot.index].selectState = SkillSlot.SelectState.SELECTED;
            }
        }
    }
    private void OnSelect(int i)
    {
        selection.selectedSkill = i;
        skillSlots[i].GetComponent<Image>().color = Color.blue ;
        skillSlots[i].selectState = SkillSlot.SelectState.SELECTED;
    }
    void Deselect(int i)
    {
        skillSlots[i].GetComponent<Image>().color = Color.white;
        skillSlots[i].selectState = SkillSlot.SelectState.NONE;
    }
}
