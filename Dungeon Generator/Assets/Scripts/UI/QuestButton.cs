using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestButton : MonoBehaviour
{
    public int index;
    public QuestSelect select;

    public void RevealDetails()
    {
        select.index = index;
        select.RevealDetails(index);
    }
    public void LoadLevel()
    {
        select.LoadLevel(index);
    }
}
