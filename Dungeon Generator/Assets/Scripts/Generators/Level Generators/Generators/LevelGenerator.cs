using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class LevelGenerator : MonoBehaviour
{

    [SerializeField]List<Room> rooms = new List<Room> { };
    List<Room> fusedRooms = new List<Room>{};

    List<Room.Fusion> fusedRoomBools = new List<Room.Fusion>(){};
    
    [SerializeField]protected Room RoomPrefab;

    int numberOfRooms = 1;

    bool bossSpawned = false;
    Room bossRoom;

    int furthestDistanceFromSpawn = 0;

    int amountOfRandomOpenEntrances = 0;

    public bool levelGenerated = false;

    public void GenerateLevel(LevelManager level, Vector2 RoomSize)
    {
        System.DateTime before = System.DateTime.Now;

        UnityEngine.Random.InitState(GameData.m_LevelConstructionSeed);

        rooms.Add(Instantiate(RoomPrefab, Vector3.zero, Quaternion.identity, transform));
        rooms[0].Initialize(RoomSize);

        SpawnRooms(UnityEngine.Random.Range((int)(level.data.m_amountOfRoomsCap.x + rooms.Count),
                                (int)(level.data.m_amountOfRoomsCap.y + rooms.Count)), RoomSize, level.data);

        level.firstRoom = rooms[0];
        level.lastRoom = rooms[rooms.Count - 1];

        //FuseRooms(RoomSize, level.data);
        AdjustRoomTypes(level.data);
        AdjustEntrances(RoomSize);

        System.DateTime after = System.DateTime.Now; 
        System.TimeSpan duration = after.Subtract(before);
        Debug.Log("Time to generate: " + duration.TotalMilliseconds + " milliseconds, which is: " + duration.TotalSeconds + " seconds");
        Debug.Log("Amount of random open entrances: " + amountOfRandomOpenEntrances);
        if(DebuggingTools.spawnOnlyBasicRooms){FindObjectOfType<DebugText>().Display(level.data);}
    }
    public void BuildLevel(LevelData data, Room currentRoom)
    {
        foreach(Room.Fusion fusion in fusedRoomBools)
        {
            if(!fusion.IsDone())
            {
                return;
            }
        }
        Debug.LogWarning("<color=blue>Time to build rooms!</color>");
        levelGenerated = true;
        GetComponent<RoomBuilder>().Build(rooms, data);
        foreach(Room room in rooms)
        {
            if(room != currentRoom && !DebuggingTools.spawnOnlyBasicRooms)
            {
                room.gameObject.SetActive(false);
            }
        }
    }


    public virtual Room ChooseLayout(LevelData data)
    {
        return RoomPrefab;
    }
    void SpawnRooms(int amountOfRooms, Vector2 RoomSize, LevelData data)
    {
        System.DateTime before = System.DateTime.Now;
        //this spawns all rooms
        for (int i = rooms.Count; i < amountOfRooms; i++)
        {
            Tuple<Room, List<RoomEntrance>> originRoom;// = new Tuple<Room, List<RoomEntrance>>(new Room(), new List<RoomEntrance>(){});
            try
            {
                originRoom = GetRandomRoomInList();
            }
            catch
            {
                Debug.Log("Could no longer spawn new rooms");
                break;
            }
            rooms.Add(Instantiate(ChooseLayout(data), transform));
            rooms[i].name = "Room #" + (numberOfRooms+1); numberOfRooms++;
            
            rooms[i].Initialize(GetNewRoomCoordinates(originRoom.Item1.transform.position, originRoom.Item2, RoomSize), RoomSize);
            rooms[i].roomData.stepsAwayFromMainRoom = originRoom.Item1.roomData.stepsAwayFromMainRoom + 1;
            if(rooms[i].roomData.stepsAwayFromMainRoom > furthestDistanceFromSpawn)
            {
                furthestDistanceFromSpawn = rooms[i].roomData.stepsAwayFromMainRoom;
            }
            SetEntrances(originRoom.Item1.transform.position, rooms[i].transform.position, originRoom.Item1.GetDirections().directions, rooms[i].GetDirections().directions);
            LinkRoom(rooms[i], RoomSize);
            OpenRandomEntrances(rooms[i], data.openDoorProbability);
        }
        System.DateTime after = System.DateTime.Now; 
        System.TimeSpan duration = after.Subtract(before);
        Debug.Log("Time to spawn rooms: " + duration.TotalMilliseconds + " milliseconds, which is: " + duration.TotalSeconds + " seconds");
    }

    void SetEntrances(Vector2 A_pos, Vector2 B_pos, List<RoomEntrance> A_entrances, List<RoomEntrance> B_entrances)
    {
        if(OnSetEntrances((int)A_pos.x, (int)B_pos.x, A_entrances, B_entrances, 2, 1)){}
        else if(OnSetEntrances((int)B_pos.x, (int)A_pos.x, B_entrances, A_entrances, 2, 1)){}
        else
        {
            if(OnSetEntrances((int)A_pos.y, (int)B_pos.y, A_entrances, B_entrances, 3, 0)){}
            else if(OnSetEntrances((int)B_pos.y, (int)A_pos.y, B_entrances, A_entrances, 3, 0)){}
            else
            {
                 Debug.LogWarning("These are either the same room, or on top of eachother!");
            }
        }
    }
    bool OnSetEntrances(int A_x, int B_x, List<RoomEntrance> A_entrances, List<RoomEntrance> B_entrances, int A_index, int B_index)
    {
        if(A_x > B_x)
        {
            A_entrances[A_index].Open = true;
            A_entrances[A_index].Spawned = true;
            B_entrances[B_index].Open = true;
            B_entrances[B_index].Spawned = true;
            return true;
        }
        return false;
    }
    Vector2 GetNewRoomCoordinates(Vector2 originCoordinates, List<RoomEntrance> openEntrances, Vector2 RoomSize)
    {
        List<Vector2> possibleCoordinates = new List<Vector2> { };
        foreach(RoomEntrance entrance in openEntrances)
        {
            if(!CheckIfCoordinatesOccupied(new Vector2(originCoordinates.x + entrance.DirectionModifier.x * RoomSize.x, originCoordinates.y + entrance.DirectionModifier.y * RoomSize.y)))
            {
                possibleCoordinates.Add(new Vector2(originCoordinates.x + entrance.DirectionModifier.x * RoomSize.x, originCoordinates.y + entrance.DirectionModifier.y * RoomSize.y));
            }
        }
        return possibleCoordinates[UnityEngine.Random.Range(0, possibleCoordinates.Count - 1)];
    }

    bool CheckIfCoordinatesOccupied(Vector2 roomPosition)
    {
        foreach (Room room in rooms)
        {
            if((Vector2)room.transform.position == roomPosition)
            {
                return true;
            }
        }
        return false;
    }

    void LinkRoom(Room room, Vector2 RoomSize)
    {
        //This function checks if this given room has another spawned room in any direction that it must link to, before it decides if it should link anywhere else
        //It does this by checking if a room in any direction has an open but not spawned gate in its own direction, in which case it opens its own gate in that direction
        for (int i = 0; i < rooms.Count; i++)
        {
            if((Vector2)rooms[i].transform.position == new Vector2(room.transform.position.x + RoomSize.x, room.transform.position.y))
            {
                if (rooms[i].GetDirections().directions[2] == null)
                {
                    continue;
                }
                if (rooms[i].GetDirections().directions[2].Open)
                {
                    SetEntrances(room.transform.position, rooms[i].transform.position, room.GetDirections().directions, rooms[i].GetDirections().directions);
                    if (rooms[i].roomData.stepsAwayFromMainRoom < room.roomData.stepsAwayFromMainRoom - 1)
                    {
                        room.roomData.stepsAwayFromMainRoom = rooms[i].roomData.stepsAwayFromMainRoom + 1;
                    }
                }
                else
                {
                    room.GetDirections().directions[1].Open = false;
                    room.GetDirections().directions[1].Spawned = true;
                }
            }
            else if ((Vector2)rooms[i].transform.position == new Vector2(room.transform.position.x - 20, room.transform.position.y))
            {
                if(rooms[i].GetDirections().directions[1] == null)
                {
                    continue;
                }
                if (rooms[i].GetDirections().directions[1].Open)
                {
                    SetEntrances(room.transform.position, rooms[i].transform.position, room.GetDirections().directions, rooms[i].GetDirections().directions);
                    if (rooms[i].roomData.stepsAwayFromMainRoom < room.roomData.stepsAwayFromMainRoom - 1)
                    {
                        room.roomData.stepsAwayFromMainRoom = rooms[i].roomData.stepsAwayFromMainRoom + 1;
                    }
                }
                else
                {
                    room.GetDirections().directions[2].Open = false;
                    room.GetDirections().directions[2].Spawned = true;
                }
            }
            else if ((Vector2)rooms[i].transform.position == new Vector2(room.transform.position.x, room.transform.position.y + 20))
            {
                if (rooms[i].GetDirections().directions[3] == null)
                {
                    continue;
                }
                if (rooms[i].GetDirections().directions[3].Open)
                {
                    SetEntrances(room.transform.position, rooms[i].transform.position, room.GetDirections().directions, rooms[i].GetDirections().directions);
                    if (rooms[i].roomData.stepsAwayFromMainRoom < room.roomData.stepsAwayFromMainRoom - 1)
                    {
                        room.roomData.stepsAwayFromMainRoom = rooms[i].roomData.stepsAwayFromMainRoom + 1;
                    }
                }
                else
                {
                    room.GetDirections().directions[0].Open = false;
                    room.GetDirections().directions[0].Spawned = true;
                }
            }
            else if ((Vector2)rooms[i].transform.position == new Vector2(room.transform.position.x, room.transform.position.y - 20))
            {
                if(rooms[i].GetDirections().directions[0] == null)
                {
                    continue;
                }
                if (rooms[i].GetDirections().directions[0].Open)
                {
                    SetEntrances(room.transform.position, rooms[i].transform.position, room.GetDirections().directions, rooms[i].GetDirections().directions);
                    if (rooms[i].roomData.stepsAwayFromMainRoom < room.roomData.stepsAwayFromMainRoom - 1)
                    {
                        room.roomData.stepsAwayFromMainRoom = rooms[i].roomData.stepsAwayFromMainRoom + 1;
                    }
                }
                else
                {
                    room.GetDirections().directions[3].Open = false;
                    room.GetDirections().directions[3].Spawned = true;
                }
            }
        }
    }

    void OpenRandomEntrances(Room room, int openDoorProbability)
    {
        //This will open a random amount of doors in the newly spawned room
        List<RoomEntrance> possibleEntrancesToOpen = new List<RoomEntrance> { };

        foreach(RoomEntrance entrance in room.GetDirections().directions)
        {
            if(entrance.Open == false && entrance.Spawned == false)
            {
                possibleEntrancesToOpen.Add(entrance);
            }
        }
        if (possibleEntrancesToOpen.Count > 0)
        {
            for (int i = UnityEngine.Random.Range(0, possibleEntrancesToOpen.Count - 1); i < UnityEngine.Random.Range(i+1, openDoorProbability); i++)
            {
                amountOfRandomOpenEntrances++;
                possibleEntrancesToOpen[UnityEngine.Random.Range(0, possibleEntrancesToOpen.Count)].Open = true;
            }
        }
    }

    void FuseRooms(Vector2 RoomSize, LevelData data)
    {
        //This function chooses a few random rooms and checks if they have rooms to the sides
        //It will then adjust camera boundaries

        int amountOfAttemptsForFusedRooms = UnityEngine.Random.Range((int)data.roomOpenness.x, (int)data.roomOpenness.y);
        if(DebuggingTools.displayFuseRoomDebugLogs)
        {
            Debug.Log("I will attempt " + amountOfAttemptsForFusedRooms + " times to make fused rooms");
        }

        for(int i = 0; i < amountOfAttemptsForFusedRooms; i++)
        {
            Room roomToFuse = GetRandomRoomInListNorthOrRight();
            if(roomToFuse == null)
            {
                continue;
            }
            fusedRooms.Add(roomToFuse);
            roomToFuse.fusionBools.hasFusedWalls = new List<bool>();

            if(DebuggingTools.displayFuseRoomDebugLogs)
            {
                Debug.Log("Room to fuse: " + roomToFuse + " Location: " + roomToFuse.transform.position + " Camera: " + roomToFuse.CameraBoundaries);
            }
            
            List<Room> roomsRight = new List<Room> { };
            List<Room> roomsNorth = new List<Room> { }; //going down, the code must check ALL doors in rooms to the right and if they go down
            List<Room> diagonalRooms = new List<Room>{};

            int amountOfStepsToRight = UnityEngine.Random.Range(3, 4);
            int amountOfStepsToNorth = UnityEngine.Random.Range(3, 4);

            if(DebuggingTools.displayFuseRoomDebugLogs)
            {
                Debug.Log(amountOfStepsToRight + " and " + amountOfStepsToNorth);
            }

            for (int j = 0; j < amountOfStepsToRight; j++)
            {
                Room newRoom = FindAdjacentRoom(roomToFuse, new Vector2(1, 0));
                if(roomsRight.Count !=0)
                {
                    newRoom = FindAdjacentRoom(roomsRight[roomsRight.Count-1], new Vector2(1, 0));
                }
                if (newRoom != null && newRoom.GetCameraBoundaries() == RoomSize)
                {
                    roomsRight.Add(newRoom);
                }
                else
                {
                    break;
                }
            }
            if (roomsRight.Count == 0)
            {
                for (int j = 0; j < amountOfStepsToNorth; j++)
                {
                    Room newRoom = FindAdjacentRoom(roomToFuse, new Vector2(0, 1));
                    if(roomsNorth.Count !=0)
                    {
                        newRoom = FindAdjacentRoom(roomsNorth[roomsNorth.Count-1], new Vector2(0,1));
                    }
                    if (newRoom != null && newRoom.GetCameraBoundaries() == RoomSize)
                    {
                        if(DebuggingTools.displayFuseRoomDebugLogs)
                        {
                            Debug.Log("This rooms: " + newRoom.name + " camera boundaries are: " + newRoom.GetCameraBoundaries());
                        }
                        roomsNorth.Add(newRoom);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                for (int j = 0; j < amountOfStepsToNorth; j++)
                {
                    Room newRoom = FindAdjacentRoom(roomToFuse, new Vector2(0, 1));
                    //If theres both rooms to the north and to the right
                    if(roomsNorth.Count > 0)
                    {
                        newRoom = FindAdjacentRoom(roomsNorth[roomsNorth.Count-1], new Vector2(0, 1));
                    }
                    if(newRoom == null)
                    {
                        goto Exit;
                    }
                    //adds a room that is up to check that it isnt null
                    if(newRoom.GetCameraBoundaries() == RoomSize)
                    {
                        if(DebuggingTools.displayFuseRoomDebugLogs)
                        {
                            Debug.Log(roomToFuse + " Adding rows " + (j+1) + " steps to the north"); 
                        }
                        List<Room> newRooms2 = new List<Room>{};
                        //when it has checked that this room is valid, it must also check every room to the right of this room
                        for(int k = 0; k < roomsRight.Count; k++)
                        {
                            if(DebuggingTools.displayFuseRoomDebugLogs)
                            {
                                Debug.Log(roomToFuse + " Checking if room " + (k+1) + " in row " + (j+1) + " is valid");
                            }
                            Room newRoom2 = FindAdjacentRoom(newRoom, new Vector2(1,0));
                            if(newRooms2.Count > 0)
                            {
                                newRoom2 = FindAdjacentRoom(newRooms2[newRooms2.Count-1], new Vector2(1,0));
                            }
                            if(newRoom2 == null || fusedRooms.Contains(newRoom2))
                            {
                                if(DebuggingTools.displayFuseRoomDebugLogs)
                                {
                                    Debug.Log(roomToFuse + "There was no diagonal room here. This row will be destroyed: Row " + (j+1));
                                }
                                newRooms2.Clear();
                                goto Exit;
                            }
                            if(newRoom2.GetCameraBoundaries() == RoomSize)
                            {
                                newRooms2.Add(newRoom2);
                            }
                        }
                        roomsNorth.Add(newRoom);
                        diagonalRooms.AddRange(newRooms2);
                    }
                    else
                    {
                        goto Exit;
                    }
                }
            }
            Exit:

            OnFuseRooms(roomToFuse, roomsRight, roomsNorth, diagonalRooms, RoomSize);
        }
    }
    void OnFuseRooms(Room roomToFuse, List<Room> roomsRight, List<Room> roomsNorth, List<Room> diagonalRooms, Vector2 RoomSize)
    {
        Vector2 steps = new Vector2(roomsRight.Count, roomsNorth.Count);
        if(steps == Vector2.zero)
        {
            roomToFuse.hasFusedWalls = true;
        }
        fusedRoomBools.Add(roomToFuse.fusionBools);
         if (roomsRight.Count > 0)
            {
                Debug.Log(steps.x);
                for (int j = 0; j < steps.x; j++)
                {
                    roomToFuse.fusionBools.hasFusedWalls.Add(false);
                    if(DebuggingTools.displayFuseRoomDebugLogs)
                    {
                        Debug.Log("Room to fuse with to the right: " + roomsRight[j] + " Location: " + roomsRight[j].transform.position + " Camera: " + roomsRight[j].CameraBoundaries);
                    }
                    roomToFuse.ExpandCameraBoundaries(roomsRight[j].GetCameraBoundaries(), roomsRight[j].transform.position);
                    roomsRight[j].EmptyCameraBoundaries();
                    StartCoroutine(roomToFuse.FuseWallPositions(roomsRight[j].GetWallPositions(), roomsRight[j].transform.position, RoomSize, j));
                    roomToFuse.FuseEntrances(roomsRight[j].GetDirections(), roomsRight[j].transform.position, RoomSize);
                }
            }
            if (roomsNorth.Count > 0)
            {
                for (int j = 0; j < steps.y; j++)
                {
                    roomToFuse.fusionBools.hasFusedWalls.Add(false);
                    if(DebuggingTools.displayFuseRoomDebugLogs)
                    {
                        Debug.Log("Room to fuse with to the north: " + roomsNorth[j] + " Location: " + roomsNorth[j].transform.position +" Camera: " + roomsNorth[j].CameraBoundaries);
                    }
                    roomToFuse.ExpandCameraBoundaries(roomsNorth[j].GetCameraBoundaries(), roomsNorth[j].transform.position);
                    roomsNorth[j].EmptyCameraBoundaries();
                    StartCoroutine(roomToFuse.FuseWallPositions(roomsNorth[j].GetWallPositions(), roomsNorth[j].transform.position, RoomSize, j + (int)steps.x));
                    roomToFuse.FuseEntrances(roomsNorth[j].GetDirections(), roomsNorth[j].transform.position, RoomSize);
                }
            }
            if(diagonalRooms.Count > 0)
            {
                if(DebuggingTools.displayFuseRoomDebugLogs)
                {
                    Debug.Log("Amount of Diagonal Rooms: " + diagonalRooms.Count);
                }
                for(int j = 0; j < diagonalRooms.Count; j++)
                {
                    roomToFuse.fusionBools.hasFusedWalls.Add(false);
                    if(DebuggingTools.displayFuseRoomDebugLogs)
                    {
                        Debug.Log("Diagonal Room to fuse with: " + diagonalRooms[j] + " Location: " + diagonalRooms[j].transform.position + " Camera: " + diagonalRooms[j].CameraBoundaries);
                    }
                    diagonalRooms[j].EmptyCameraBoundaries();
                    StartCoroutine(roomToFuse.FuseWallPositions(diagonalRooms[j].GetWallPositions(), diagonalRooms[j].transform.position, RoomSize, j + (int)steps.x + (int)steps.y));
                    roomToFuse.FuseEntrances(diagonalRooms[j].GetDirections(), diagonalRooms[j].transform.position, RoomSize);
                }
            }
    }

    void AdjustRoomTypes(LevelData data)
    {
        //This function fixes walls, puts in obstacles and also enemies, as well as just normal shrubbery and shit
        //Debug.Log("Adjusting Rooms!");
        foreach(Room room in rooms)
        {
            room.ChooseRoomType(data);
            if (room.GetRoomPositionType() == RoomPosition.DeadEnd)
            {
                if(room.roomData.stepsAwayFromMainRoom == furthestDistanceFromSpawn && !bossSpawned)
                {
                    //Set the room furthest from spawn to be the bossroom
                    room.SetRoomType(RoomType.BossRoom);
                    bossRoom = room;
                    bossSpawned = true;
                    Debug.Log("Boss spawned in: " + bossRoom);
                }
                if(room.GetRoomType() == RoomType.TreasureRoom)
                {
                    //Put an ambushroom next to a treasure room that is a dead end
                    Room roomToChange = FindAdjacentRoom(room);
                    if(roomToChange != null)
                    { 
                        roomToChange.SetRoomType(RoomType.AmbushRoom);
                    }
                    else
                    {
                        Debug.Log("Room was null!");
                    }
                }
                if (room.GetRoomType() == RoomType.AmbushRoom)
                {
                    //Put a safe room next to an ambush room that is a dead end
                    int random = UnityEngine.Random.Range(0, 5);
                    if(random == 0)
                    {
                        Room roomToChange = FindAdjacentRoom(room);
                        if (roomToChange != null)
                        {
                            roomToChange.SetRoomType(RoomType.RestingRoom);
                        }
                    }
                }
                if(room.GetRoomType() == RoomType.BossRoom)
                {
                    //Put a safe room next to a boss room
                    Room roomToChange = FindAdjacentRoom(room);
                    if (roomToChange != null)
                    {
                        roomToChange.SetRoomType(RoomType.RestingRoom);
                    }
                }
            }
            else
            {
                if (room.roomData.stepsAwayFromMainRoom == furthestDistanceFromSpawn && !bossSpawned)
                {
                    room.SetRoomType(RoomType.BossRoom);
                    bossRoom = room;
                    bossSpawned = true;
                    Debug.Log("Boss spawned in: " + bossRoom);
                }
            }
        }
    }
    void AdjustEntrances(Vector2 RoomSize)
    {
        foreach(Room room in rooms)
        {
            switch (room.GetRoomType())
            {
                case RoomType.TreasureRoom:
                    if(room.GetRoomPositionType() != RoomPosition.DeadEnd)
                    {
                        foreach (RoomEntrance entrance in room.GetDirections().directions)
                        {
                            if (entrance != null)
                            {
                                entrance.SetEntranceType(RoomEntrance.EntranceType.BombableWall, GetComponent<EntranceLibrary>());
                                entrance.transform.position = new Vector2(room.transform.position.x + entrance.DirectionModifier.x * entrance.Index.x + RoomSize.x/2, room.transform.position.y + entrance.DirectionModifier.y * entrance.Index.x + RoomSize.y/2);
                            }
                        }
                    }
                    break;
                case RoomType.RestingRoom:
                    break;
                case RoomType.AmbushRoom:
                    foreach (RoomEntrance entrance in room.GetDirections().directions)
                    {
                        if(entrance != null)
                        {
                            entrance.SetEntranceType(RoomEntrance.EntranceType.AmbushDoor, GetComponent<EntranceLibrary>());
                            entrance.transform.position = new Vector2(room.transform.position.x + entrance.DirectionModifier.x * entrance.Index.x + RoomSize.x/2, room.transform.position.y + entrance.DirectionModifier.y * entrance.Index.x + RoomSize.y/2);
                        }
                    }
                    break;
                case RoomType.BossRoom:
                    foreach (RoomEntrance entrance in room.GetDirections().directions)
                    {
                        if (entrance != null)
                        {
                            entrance.SetEntranceType(RoomEntrance.EntranceType.MultiLockedDoor, GetComponent<EntranceLibrary>());
                            entrance.transform.position = new Vector2(room.transform.position.x + entrance.DirectionModifier.x * entrance.Index.x + RoomSize.x/2, room.transform.position.y + entrance.DirectionModifier.y * entrance.Index.x + RoomSize.y/2);
                        }
                    }
                    break;
                case RoomType.MiniBossRoom:
                    break;
                default: break;
            }
        }
    }
}
//The following part of the class finds and returns rooms
public partial class LevelGenerator : MonoBehaviour
{
    Tuple<Room, List<RoomEntrance>> GetRandomRoomInList()
    {
        //This functions gets any of the rooms that are already spawned
        //It should make sure that it doesnt have something spawned in each direction

        List<Tuple<Room, List<RoomEntrance>>> roomsWithOpenDoors = new List<Tuple<Room, List<RoomEntrance>>>{};

        foreach (Room room in rooms)
        {
            List<RoomEntrance> openEntrances = room.GetOpenUnspawnedEntrances();
            if (openEntrances.Count > 0)
            {
                roomsWithOpenDoors.Add(new Tuple<Room, List<RoomEntrance>>(room, openEntrances));
            }
        }
        return roomsWithOpenDoors[UnityEngine.Random.Range(0, roomsWithOpenDoors.Count - 1)];
    }
    Room GetRandomRoomInListNorthOrRight()
    {
        if (rooms.Count != 0)
        {
            List<Room> roomWithOpenDoors = new List<Room> { };
            foreach (Room room in rooms)
            {
                if(room.GetDirections().directions[2] == null || room.CameraBoundaries == Vector2.zero || fusedRooms.Contains(room))
                {
                    continue;
                }
                if (room.GetDirections().directions[2].Open && room.GetDirections().directions[2].Spawned)
                {
                    roomWithOpenDoors.Add(room);
                }
                else if (room.GetDirections().directions[0].Open && room.GetDirections().directions[0].Spawned)
                {
                    roomWithOpenDoors.Add(room);
                }
            }
            if(roomWithOpenDoors.Count == 0)
            {
                return null;
            }
            return roomWithOpenDoors[UnityEngine.Random.Range(0, roomWithOpenDoors.Count - 1)];
        }
        return rooms[0];
    }
    Room FindAdjacentRoom(Room origin)
    {
        RoomEntrance openEntrance = null;
        foreach (RoomEntrance entrance in origin.GetDirections().directions)
        {
            if(entrance == null)
            {
                continue;
            }
            if (entrance.Open && entrance.Spawned)
            {
                openEntrance = entrance;
            }
        }
        if (openEntrance != null)
        {
            Room newRoom = FindRoomOfPosition((Vector2)origin.transform.position + openEntrance.DirectionModifier * 20);
            if (newRoom != null)
            {
                if (newRoom.GetCameraBoundaries() == Vector2.zero)
                {
                    if(DebuggingTools.displayFuseRoomDebugLogs)
                    { 
                        Debug.Log("This rooms boundaries was zero");
                    }
                    if(newRoom.GetDirections().directions[3])
                    {
                        newRoom = FindRoomOfPosition((Vector2)newRoom.transform.position + newRoom.GetDirections().directions[3].DirectionModifier * 20);
                    }
                }
            }
            return newRoom;
        }
        return null;
    }
    public Room FindAdjacentRoom(Room origin, Vector2 direction)
    {
        return FindRoomOfPosition((Vector2)origin.transform.position + direction * 20);
    }
    Room FindAdjacentRoomNorthOrRight(Room origin)
    {
        RoomEntrance openEntrance = null;
        if (origin.GetDirections().directions[0].Open && origin.GetDirections().directions[0].Spawned)
        {
            openEntrance = origin.GetDirections().directions[0];
        }
        else if (origin.GetDirections().directions[1].Open && origin.GetDirections().directions[1].Spawned)
        {
            openEntrance = origin.GetDirections().directions[1];
        }
        if (openEntrance != null)
        {
            return FindRoomOfPosition((Vector2)origin.transform.position + openEntrance.DirectionModifier * 20);
        }
        return null;
    }
    Room FindRoomOfPosition(Vector2 position)
    {
        foreach (Room room in rooms)
        {
            if ((Vector2)room.transform.position == position)
            {
                return room;
            }
        }
        Debug.Log("Position: " + position);
        return null;
    }
    public void DestroyLevel()
    {
        for(int i = rooms.Count -1; i >= 0; i--)
        {
            Destroy(rooms[i].gameObject);
        }
        rooms.Clear();
        numberOfRooms = 1;
    }
}