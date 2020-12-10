using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InvestigationQuestData : QuestData
{
    public override string GetQuestDescription()
    {
        return "We need someone to investigate the strange happenings at this location.";
    }
}
