using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public struct NPCInformation
    {
        public EntityMovementModel NPC;
        public Vector2 Room;

        public NPCInformation(EntityMovementModel NPC_in, Vector2 Room_in)
        {
            NPC = NPC_in;
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
