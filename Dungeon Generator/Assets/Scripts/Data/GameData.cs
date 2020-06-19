using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;
    static PlayerController Player;

    public static LevelData currentLevel;

    public static int m_LevelConstructionSeed; //Used by the room generator to generate the room
    public static int m_LevelDataSeed;
    //134523 heart
    private void Awake()
    {
        if(Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
        Player = GetComponentInChildren<PlayerController>();
    }

    public static void SetSeed(int constructionSeed, int levelDataSeed)
    {
        Debug.Log("The construction seed is: " + constructionSeed);
        Debug.Log("The data seed is: " + levelDataSeed);
        m_LevelConstructionSeed = constructionSeed;
        m_LevelDataSeed = levelDataSeed;
    }
    public static Vector2 GetPlayerPosition()
    {
        return Player.transform.position;
    }
    public static void SetPlayerPosition(Vector2 newPosition)
    {
        Player.transform.position = newPosition;
    }
}
