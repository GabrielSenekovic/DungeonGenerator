using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

[System.Serializable]
public class BackupQuestData : QuestData
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

    public List<NPCInformation> NPCsToBackup;
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
