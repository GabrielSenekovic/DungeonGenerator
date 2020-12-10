using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InquiryQuestData : QuestData
{
    public NPCInformation Target;

    public override void Initialize(MissionType type_in)
    {
        missionType = type_in;
    }

    public override string GetQuestDescription()
    {
        return "There is a person I would like to speak to. I need your help to find them.";
    }
    public override bool GetStatus()
    {
        return Target.NPC.GetComponent<NPCInteraction>().isInteractedWith;
    }
}
