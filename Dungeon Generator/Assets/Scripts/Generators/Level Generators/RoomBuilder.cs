using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    [SerializeField] WallBlueprints wallBlueprints; //This is a placeholder for what will be later
    [SerializeField] GameObject floorPrefab;

    public void Build(List<Room> rooms, LevelData data)
    {
        if(!DebuggingTools.spawnOnlyBasicRooms)
        {
           // floorPrefab.GetComponentInChildren<SpriteRenderer>().sprite = GetComponent<EntranceLibrary>().GetFloorSprite(data.m_biome);
        }
        BuildRooms(rooms);
        CloseOpenDoors(rooms);
    }

    void BuildRooms(List<Room> rooms)
    {
        for(int i = 0; i < rooms.Count; i++)
        {
            if (!rooms[i].roomData.IsBuilt)
            {
                rooms[i].PlaceDownWalls(wallBlueprints);
                //room.InstantiateWalls(wallBlueprints);
                rooms[i].InstantiateFloor(floorPrefab);
                rooms[i].DisplayDistance();

                foreach (Transform child in rooms[i].transform)
                {
                    if (child.GetComponent<WallPosition>())
                    {
                        Destroy(child.gameObject);
                    }
                }
                rooms[i].roomData.IsBuilt = true;
            }
        }
    }
    void CloseOpenDoors(List<Room> rooms)
    {
        //This function must close all open doors in each room that doesnt lead anywhere
        for(int i = 0; i < rooms.Count; i++)
        {
            if (!rooms[i].GetDirections())
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
