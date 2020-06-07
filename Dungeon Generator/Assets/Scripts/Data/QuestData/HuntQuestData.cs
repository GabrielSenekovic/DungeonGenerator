using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntQuestData : QuestData
{
    Data target;

    public override string GetQuestDescription()
    {
        return "I need someone to find this person and destroy them. Whatever it takes.";
    }
}
