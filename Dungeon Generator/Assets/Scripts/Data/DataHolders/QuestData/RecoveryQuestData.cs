using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryQuestData : QuestData
{
    public class RecoveryObject
    {
        public enum State
        {
            Stolen = 0, //Somebody took it and ran away with it
            Lost = 1, //Questgiver dropped it while out
            Ordered = 2, //Questgiver ordered it from someone but no trade-route is available
            Mythical_Exists = 3, //Nobody knows if it actually exists
            Mythical_Unreal = 4
        }
        //if it is a recoveryMission
        //CharacterData and ItemData inherit from Data
        Data thing = null;
    }
    RecoveryObject thingToRecover;
}
