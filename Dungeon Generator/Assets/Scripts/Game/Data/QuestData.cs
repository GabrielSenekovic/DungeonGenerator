using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class QuestDataGenerator : MonoBehaviour
{
    public static QuestData Initialize(int questSeed)
    {
        UnityEngine.Random.InitState(questSeed);
        QuestData.MissionType type = (QuestData.MissionType)UnityEngine.Random.Range(0, 5);
        QuestData data = GetQuestType(type);
        data.Initialize(type);
        return data;
    }
    static QuestData GetQuestType(QuestData.MissionType type)
    {
        QuestData data;
        switch(type)
        {
            case QuestData.MissionType.Backup: 
                data = new BackupQuestData();
                int limit = UnityEngine.Random.Range(3, 10);
                for (int i = 0; i < limit;i++)
                {
                    (data as BackupQuestData).NPCsToBackup.Add(new QuestData.NPCInformation(/*NPC,*/ new Vector2(0,1)));
                }
                break;
            case QuestData.MissionType.Hunt: 
                data = new HuntQuestData();
                (data as HuntQuestData).hunt = new QuestData.NPCInformation(/*NPC,*/ new Vector2(0, 1));
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
[System.Serializable]public class QuestData
{
    public struct NPCInformation
    {
        public MovementModel NPC;
        public Vector2 Room;

        public NPCInformation(MovementModel NPC_in, Vector2 Room_in)
        {
            NPC = NPC_in;
            Room = Room_in;
        }
        public NPCInformation(Vector2 Room_in)
        {
            NPC = null;
            Room = Room_in;
        }
    }
    public enum MissionType
    {
        Recovery = 0, //Find and recover item or person
        Inquiry = 1, //Find person and ask them something
        Backup = 2, //Find a group of people that need help
        Investigation = 3, //Find out information about a location
        Hunt = 4, //Find a person on the run 
        Escort = 5, //puts mustEscortSomeone as true by default. You only need to take them from point A to point B without them dying
        Delivery = 6 //Bring an item to a person or place
    }
    CharacterData questGiver = null;
    public MissionType missionType;
    bool mustEscortSomeone = false;
    //escort to recovery mission - they probably want to make sure youre not stealing it
    //escort to speaking mission - they want to be part of the discussion
    //escort to backup mission - they are part of the backup
    //escort to investigation mission - theyre probably an archeologist
    //escort to hunting mission - its personal

    public QuestData()
    {
    }
    public virtual void Initialize(MissionType type_in)
    {
    }

    public virtual string GetQuestDescription()
    {
        return "";
    }

    public virtual bool GetStatus()
    {
        return false;
    }
}
[System.Serializable]public class BackupQuestData : QuestData
{
     //is it a mission to go backup someone or a team/guard an object or place?
    //it is a type of mission to travel all the way to a point where a huge battle will take place/is taking place or has taken place(in which case youre too late)
   
        //Either, this is a settlement, in which case you come to help
        //Or this isnt settled, in which case the people are probably scavangers or foragers
        //If its very far from a settlement, they will most likely be adventurers
    public enum State
    {
        Early = 0, //you have arrived before the danger
        Mid = 1, //you have arrived during the disaster
        Late = 2, //the disaster has already taken place, scavenge to save what you can
        TooLate = 3 //nothing can be done. There was nothing you could have done. Report back.
    }
    public enum Disaster
    {
        Battle = 0, //an army needs backup for a fight
        Siege = 1, //guards needs backup to protect a village/keep/fortress
        Conflagration = 2, //theres a forest fire that must be put out
        Flood = 3, //theres been a terrible flood
    }
    public State state;
    public Disaster disaster;

    public List<NPCInformation> NPCsToBackup = new List<NPCInformation>();
    //A timer of sorts will show you the collective HP of all the NPCs you have to backup.
    //The progress of the timer will determine your level of success.
    //If it turns out badly, it might add an event to your list of events

    public override void Initialize(MissionType type_in)
    {
        missionType = type_in;
        List<State> temp = new List<State>(){};
        for(int i = 0; i < 5; i++)
        {
            temp.Add(State.Early);
            temp.Add(State.Mid);
        }
        for(int i = 0; i < 3; i++)
        {
            temp.Add(State.Late);
        }
        temp.Add(State.TooLate);
        state = temp[UnityEngine.Random.Range(0, temp.Count)];
        List<Disaster> temp2 = new List<Disaster>(){};
        for(int i = 0; i < 4; i++)
        {
            temp2.Add((Disaster)i);
        }
        disaster = temp2[UnityEngine.Random.Range(0,temp2.Count)];
    }

    public override string GetQuestDescription()
    {
        string description = "We need help! ";
        switch(disaster)
        {
            case Disaster.Battle: 
                switch(state)
                {
                    case State.Early: description += "We are not enough people to fight at this location. Side with us to protect our honor!"; break;
                    default: description += "There might not be much time left. We need backup to fight at this location. If we do not get help soon, all hope might be lost..."; break;
                }
                break;
            case Disaster.Siege:
                switch(state)
                {
                    case State.Early: description += "We have word that we might be attacked soon and we require backup in case the worst happens."; break;
                    default: description += "There might not be much time left. We need backup to protect the keep. If we do not get help soon, all hope might be lost..."; break;
                }
                break;
            case Disaster.Conflagration: description += "A fire has flared up and threatens to destroy the environment! If it gets to wreak havoc for too long, who knows what might happen...";
                break;
            case Disaster.Flood: description += "A terrible flood has covered the lands with devastating water! We require backup to evacuate everyone from the premises.";
                break;
            default: return description;
        }
        return description;
    }
    public override bool GetStatus()
    {
        switch(state)
        {
            case State.Early: return GetDisasterStatus();
            case State.Mid: return GetDisasterStatus();
            case State.Late: return GetDamageControlStatus();
                //Help them with what they need
            case State.TooLate: return false; 
                //Just play a cutscene where you assess the damage and return true
            default: return false;
        }
    }
    public bool GetDisasterStatus()
    {
        switch (disaster)
        {
            //Evacuate when flood or fire is going on. Might evacuate during siege. May fight during battle and siege.
            case Disaster.Battle: return false;
            case Disaster.Conflagration: return false;
            case Disaster.Flood: return false;
            case Disaster.Siege: return false;
            default: return false;
        }
    }
    public bool GetDamageControlStatus()
    {
        switch (disaster)
        {
            //Confirm deaths, help gather debree, might have to find certain items
            case Disaster.Battle: 
                return false;
            case Disaster.Conflagration: 
                return false;
            case Disaster.Flood: 
                return false;
            case Disaster.Siege: 
                return false;
            default: return false;
        }
    }
}
[System.Serializable]public class HuntQuestData : QuestData
{
    public NPCInformation hunt;
    public override bool GetStatus()
    {
        return hunt.NPC.GetComponent<HealthModel>().GetHealthPercentage() <= 0;
    }
    public override string GetQuestDescription()
    {
        return "I need someone to find this person and destroy them. Whatever it takes.";
    }
}
[System.Serializable]public class RecoveryQuestData : QuestData
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
        thingToRecover = new RecoveryObject(new ItemData(ItemGenerator.GetInstance().GenerateItemSprite()), temp[UnityEngine.Random.Range(0, temp.Count)]);
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
[System.Serializable]public class InvestigationQuestData : QuestData
{
    public override string GetQuestDescription()
    {
        return "We need someone to investigate the strange happenings at this location.";
    }
}
[System.Serializable]public class InquiryQuestData : QuestData
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
        if(Target.NPC != null)
        {
            return Target.NPC.GetComponent<NPCInteraction>().isInteractedWith;
        }
        else
        {
            return false;
        }
    }
}