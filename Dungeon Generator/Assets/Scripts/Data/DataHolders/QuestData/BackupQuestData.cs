using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackupQuestData : MonoBehaviour
{
     //is it a mission to go backup someone or a team/guard an object or place?
    //it is a type of mission to travel all the way to a point where a huge battle will take place/is taking place or has taken place(in which case youre too late)
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
}
