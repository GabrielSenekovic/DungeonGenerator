using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;
    static PlayerController Player;

    [SerializeField] static int m_LevelConstructionSeed = 134523; //Used by the room generator to generate the room
    [SerializeField] static int m_LevelDataSeed = 9562;
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

    public static void SetSeed(int seed)
    {
        Debug.Log("Seed is: " + seed);
        m_LevelConstructionSeed = seed;
    }
    public static int GetConstructionSeed()
    {
        return m_LevelConstructionSeed;
    }
    public static int GetDataSeed()
    {
        return m_LevelDataSeed;
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
