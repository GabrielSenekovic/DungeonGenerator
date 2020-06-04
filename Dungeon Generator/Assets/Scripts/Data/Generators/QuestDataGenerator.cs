using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDataGenerator : MonoBehaviour
{
    public QuestData Initialize(int questSeed)
    {
        QuestData data = new QuestData();
        Random.InitState(questSeed);
        data.missionType = (QuestData.MissionType)Random.Range(0, 5);
        return data;
    }
}
