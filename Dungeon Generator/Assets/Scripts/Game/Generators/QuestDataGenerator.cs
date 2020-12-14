using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class QuestDataGenerator : MonoBehaviour
{
    public EntityMovementModel NPC;
    public QuestData Initialize(int questSeed)
    {
        Random.InitState(questSeed);
        QuestData.MissionType type = (QuestData.MissionType)Random.Range(0, 5);
        QuestData data = GetQuestType(type);
        data.Initialize(type);
        return data;
    }
    QuestData GetQuestType(QuestData.MissionType type)
    {
        QuestData data;
        switch(type)
        {
            case QuestData.MissionType.Backup: 
                data = new BackupQuestData();
                int limit = Random.Range(3, 10);
                for (int i = 0; i < limit;i++)
                {
                    (data as BackupQuestData).NPCsToBackup.Add(new QuestData.NPCInformation(NPC, new Vector2(0,1)));
                }
                break;
            case QuestData.MissionType.Hunt: 
                data = new HuntQuestData();
                (data as HuntQuestData).hunt = new QuestData.NPCInformation(NPC, new Vector2(0, 1));
                break;
            case QuestData.MissionType.Inquiry: 
                data = new InquiryQuestData(); break;
            case QuestData.MissionType.Investigation: 
                data = new InvestigationQuestData(); break;
            case QuestData.MissionType.Recovery: 
                data = new RecoveryQuestData(); break;
            default: data = new QuestData(); break;
        }
        return data;
    }
}
