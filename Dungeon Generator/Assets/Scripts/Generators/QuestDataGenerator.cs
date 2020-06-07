using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDataGenerator : MonoBehaviour
{
    public QuestData Initialize(int questSeed)
    {
        Random.InitState(questSeed);
        QuestData data = GetQuestType();
        data.Initialize();
        return data;
    }
    QuestData GetQuestType()
    {
        QuestData.MissionType type = (QuestData.MissionType)Random.Range(0, 5);
        switch(type)
        {
            case QuestData.MissionType.Backup: return new BackupQuestData();
            case QuestData.MissionType.Hunt: return new HuntQuestData();
            case QuestData.MissionType.Inquiry: return new InquiryQuestData();
            case QuestData.MissionType.Investigation: return new InvestigationQuestData();
            case QuestData.MissionType.Recovery: return new RecoveryQuestData();
            default: return new QuestData();
        }
    }
}
