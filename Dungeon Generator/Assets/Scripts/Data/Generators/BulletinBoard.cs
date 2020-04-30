using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class BulletinBoard : InteractableBase
{
    int[] seeds = new int[5];
    bool m_active = true;

    int[] GenerateNewSeeds()
    {
        return new int[5] { 0, 0, 0, 0, 0 };
    }
    int GenerateSeed()
    {
        return Random.Range(1, 10);
    }

    public override void OnInteract()
    {
        //Open bulletin board screen
        //For now, just load a randomass level
        if(m_active)
        {
            LoadLevel(0);
            m_active = false;
        }
    }

    public void LoadLevel(int index)
    {
        //GameData.SetSeed(seeds[index]);
        GameData.SetSeed(GenerateSeed());
        SceneManager.LoadSceneAsync("Level");
    }
}
