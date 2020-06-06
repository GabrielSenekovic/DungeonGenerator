using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelData data;
    Vector2 RoomSize = new Vector2(20,20);

    public Room firstRoom;
    public Room lastRoom;

    private void Awake() 
    {
        if(GameData.Instance != null)
        {
            GameData.SetPlayerPosition(new Vector2(GameData.GetPlayerPosition().x - RoomSize.x/2, GameData.GetPlayerPosition().y - RoomSize.y/2));
        }
    }
    private void Start() 
    {
        data = GetComponent<LevelDataGenerator>().Initialize(Random.Range(0, int.MaxValue));
        if(GetComponent<DebuggingTools>())
        {
            data.m_location = GetComponent<DebuggingTools>().location;
        }
        GetComponent<LevelGenerator>().GenerateLevel(this, RoomSize);
    }
}
