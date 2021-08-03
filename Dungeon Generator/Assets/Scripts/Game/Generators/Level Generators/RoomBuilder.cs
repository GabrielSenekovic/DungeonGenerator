using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder : MonoBehaviour
{

    public void Build(List<Room> rooms, LevelData data)
    {
        BuildRooms(rooms);
        CloseOpenDoors(rooms);
    }

    void BuildRooms(List<Room> rooms)
    {
        for(int i = 0; i < rooms.Count; i++)
        {
            if (!rooms[i].roomData.IsBuilt)
            {
                rooms[i].DisplayDistance();
                rooms[i].roomData.IsBuilt = true;
            }
        }
    }
    void CloseOpenDoors(List<Room> rooms)
    {
        //This function must close all open doors in each room that doesnt lead anywhere
        for(int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].GetDirections() == null)
            {
                continue;
            }
            for (int j = 0; j < rooms[i].GetDirections().directions.Count; j++)
            {
                if(rooms[i].GetRoomType() == RoomType.BossRoom)
                {
                    continue;
                }
                if(rooms[i].GetDirections().directions[j] == null)
                {
                    continue;
                }
                if (!rooms[i].GetDirections().directions[j].Spawned || (!rooms[i].GetDirections().directions[j].Open && rooms[i].GetDirections().directions[j].Spawned))
                {
                   Destroy(rooms[i].GetDirections().directions[j].gameObject);
                   rooms[i].GetDirections().directions[j] = null;
                }
            }
        }
    }
}
