using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecoveryQuestData : QuestData
{
    public class RecoveryObject
    {
        public RecoveryObject(BaseData thing_in, State state_in)
        {
            thing = thing_in;
            state = state_in;
        }
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
        public BaseData thing = null;
        public State state;
    }
    public RecoveryObject thingToRecover;

    public override void Initialize(MissionType type_in)
    {
        missionType = type_in;
        List<RecoveryObject.State> temp = new List<RecoveryObject.State>(){};
        for (uint i = 0; i < 5; i++)
        {
            temp.Add(RecoveryObject.State.Stolen);
            temp.Add(RecoveryObject.State.Lost);
            temp.Add(RecoveryObject.State.Ordered);
        }
        temp.Add(RecoveryObject.State.Mythical_Exists);
        temp.Add(RecoveryObject.State.Mythical_Unreal);
        thingToRecover = new RecoveryObject(new ItemData(ItemGenerator.GetInstance().GenerateItemSprite()), temp[Random.Range(0, temp.Count)]);
    }

    public override string GetQuestDescription()
    {
        string description = "I need help to get something. ";
        switch(thingToRecover.state)
        {
            case RecoveryObject.State.Stolen: description += "It was stolen from me, and I will reward whoever can retrieve my item.";
                break;
            case RecoveryObject.State.Lost: description += "I must have lost it when I was walking around this area. I sincerely hope someone can find it for me...";
                break;
            case RecoveryObject.State.Ordered: description += "I had ordered an item but due to certain circumstances, it has not been possible for mail delivery to get it to me. It's a hassle, but I'll pay whoever can get me my item.";
                break;
            default: description += "This item may not even exist, but... I will pay whoever can get it for me with unknown riches!";
                break;
        }
        description += "It looks like this: ";
        return description;
    }
}
