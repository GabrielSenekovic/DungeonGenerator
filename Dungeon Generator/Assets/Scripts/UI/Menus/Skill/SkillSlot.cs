using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlot : InventorySlot, IPointerClickHandler
{
    public enum EquipState
    {
        NONE,
        EQUIPPED,
        WAITING, //a flag to tell the skillmanager to unequip it
        SKILL, //a flag to tell the skillmanager this is a skill in the skill list
        EQUIPPED_LIST //a flag to tell the skillmanager this is a skill in the skill list that is equipped
    }
    public enum SelectState
    {
        NONE,
        SELECTED,
        WAITING
    }
    public AttackIdentifier attack;
    public EquipState state = EquipState.NONE;
    public SelectState selectState = SelectState.NONE;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && (state == EquipState.EQUIPPED || state == EquipState.SKILL))
        {
            //For selecting a skill
            selectState = SelectState.WAITING;
        }
        if (eventData.button == PointerEventData.InputButton.Right && state == EquipState.EQUIPPED)
        {
            //For unequipping skills
            state = EquipState.WAITING;
        }
    }
    public void UnEquip()
    {
        GetComponent<Image>().color = Color.white;
        state = EquipState.SKILL;
    }
}