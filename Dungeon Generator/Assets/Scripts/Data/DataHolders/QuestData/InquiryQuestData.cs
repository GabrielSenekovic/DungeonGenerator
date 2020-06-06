using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InquiryQuestData : QuestData
{
    CharacterData personToSpeakTo;

    public override string GetQuestDescription()
    {
        return "There is a person I would like to speak to. I need your help to find them.";
    }
}
