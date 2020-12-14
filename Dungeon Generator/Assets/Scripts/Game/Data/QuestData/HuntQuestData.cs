using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HuntQuestData : QuestData
{
    public NPCInformation hunt;
    public override bool GetStatus()
    {
        return hunt.NPC.GetComponent<HealthModel>().GetHealthPercentage() <= 0;
    }
    public override string GetQuestDescription()
    {
        return "I need someone to find this person and destroy them. Whatever it takes.";
    }
}
