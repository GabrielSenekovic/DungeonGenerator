using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Vector2 RoomSize = new Vector2(20,20);

    public LevelData data;
    public Room firstRoom;
    public Room lastRoom;

    private void Awake() 
    {
        //GameData.m_LevelConstructionSeed = Random.Range(0, int.MaxValue);
        //GameData.m_LevelDataSeed = Random.Range(0, int.MaxValue);
        if(GameData.Instance != null)
        {
            GameData.SetPlayerPosition(new Vector2(GameData.GetPlayerPosition().x + RoomSize.x/2, GameData.GetPlayerPosition().y + RoomSize.y/2));
        }
    }
    private void Start() 
    {
        data = GetComponent<LevelDataGenerator>().Initialize(GameData.m_LevelDataSeed);
        data.dungeon = DebuggingTools.isDungeon;
        if(DebuggingTools.checkForBrokenSeeds)
        {
            try
            {
                GetComponent<LevelGenerator>().GenerateLevel(this, RoomSize);
            }
            catch
            {
                Debug.LogError("<color=red>Error: Found broken seed when generating!:</color> " + GameData.m_LevelConstructionSeed + " and: " + GameData.m_LevelDataSeed);
                Debug.Break();
            }
        }
        else
        {
            GetComponent<LevelGenerator>().GenerateLevel(this, RoomSize);
        }
    }
    private void Update()
    {
        if(DebuggingTools.checkForBrokenSeeds)
        {
            try
            {
                GetComponent<LevelGenerator>().BuildLevel(data);
            }
            catch
            {
                Debug.LogError("<color=red>Error: Found broken seed when building!:</color> " + GameData.m_LevelConstructionSeed + " and: " + GameData.m_LevelDataSeed);
                Debug.Break();
            }
        }
        else
        {
            GetComponent<LevelGenerator>().BuildLevel(data);
        }
    }
}
