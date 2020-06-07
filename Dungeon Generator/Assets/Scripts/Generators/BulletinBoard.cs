using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class BulletinBoard : InteractableBase
{
    [SerializeField]CanvasGroup questScreen;
    int[] seeds = new int[5];
    bool m_active = true;

    Tuple<int[], int[], int[]> GenerateNewSeeds() //here temporarily
    {
        return new Tuple<int[], int[], int[]>
        (
            new int[5]{UnityEngine.Random.Range(0, int.MaxValue),UnityEngine.Random.Range(0, int.MaxValue),UnityEngine.Random.Range(0, int.MaxValue),UnityEngine.Random.Range(0, int.MaxValue),UnityEngine.Random.Range(0, int.MaxValue)}, 
            new int[5]{UnityEngine.Random.Range(0, int.MaxValue),UnityEngine.Random.Range(0, int.MaxValue),UnityEngine.Random.Range(0, int.MaxValue),UnityEngine.Random.Range(0, int.MaxValue),UnityEngine.Random.Range(0, int.MaxValue)},
            new int[5]{UnityEngine.Random.Range(0, int.MaxValue),UnityEngine.Random.Range(0, int.MaxValue),UnityEngine.Random.Range(0, int.MaxValue),UnityEngine.Random.Range(0, int.MaxValue),UnityEngine.Random.Range(0, int.MaxValue)}
        );
    }

    public override void OnInteract()
    {
        questScreen.GetComponent<QuestSelect>().Initialize(GenerateNewSeeds());
        UIManager.OpenOrClose(questScreen);
    }
}
