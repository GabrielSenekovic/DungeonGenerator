using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : LevelGenerator
{
    [SerializeField]CorridorRoom CorridorRoomPrefab;
    public override Room ChooseLayout(LevelData data)
    {
        /*RoomLayout layout = RoomLayout.NormalOutdoors;
        List<RoomLayout> layouts = new List < RoomLayout >{ };
        if(data.GetMood(0) == Mood.Decrepit || data.GetMood(1) == Mood.Decrepit)
        {
            for(int i = 0; i < 3; i++)
            {
                layouts.Add(RoomLayout.NormalOutdoors);
            }
        }
        for(int i = 0; i < 3; i++)
        {
            layouts.Add(RoomLayout.NormalIndoors);
            layouts.Add(RoomLayout.NormalIndoors);
            layouts.Add(RoomLayout.NormalIndoors);
            layouts.Add(RoomLayout.Corridor);
        }
        int temp = UnityEngine.Random.Range(0, layouts.Count);
        layout = layouts[temp];
        switch(layout)
        {
            case RoomLayout.NormalIndoors: RoomPrefab.roomData.m_layout = RoomLayout.NormalIndoors; return RoomPrefab;
            case RoomLayout.NormalOutdoors: RoomPrefab.roomData.m_layout = RoomLayout.NormalOutdoors; return RoomPrefab;
            case RoomLayout.Corridor: return CorridorRoomPrefab;
        }*/
        return CorridorRoomPrefab;
    }
}
