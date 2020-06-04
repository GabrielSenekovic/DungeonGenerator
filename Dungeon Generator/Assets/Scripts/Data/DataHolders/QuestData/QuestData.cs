using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData
{
    public enum MissionType
    {
        Recovery = 0,
        Inquiry = 1,
        Backup = 2,
        Investigation = 3,
        Hunt = 4

    }
    CharacterData questGiver = null;
    public MissionType missionType;
    bool mustEscortSomeone = false;
    //escort to recovery mission - they probably want to make sure youre not stealing it
    //escort to speaking mission - they want to be part of the discussion
    //escort to backup mission - they are part of the backup
    //escort to investigation mission - theyre probably an archeologist
    //escort to hunting mission - its personal
}
