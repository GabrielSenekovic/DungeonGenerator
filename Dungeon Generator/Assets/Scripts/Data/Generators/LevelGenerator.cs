using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LevelGenerator : MonoBehaviour
{
    LevelData m_data;
    RoomBuilder m_builder;

    [SerializeField]List<Room> m_rooms = new List<Room> { };
    
    [SerializeField] Room m_RoomPrefab;

    int m_numberOfRooms = 2;

    bool m_BossSpawned = false;
    Room m_BossRoom;

    int m_furthestDistanceFromSpawn = 0;

    private void Awake()
    {
        if(GameData.Instance != null)
        {
            GameData.SetPlayerPosition(new Vector2(GameData.GetPlayerPosition().x - 10, GameData.GetPlayerPosition().y - 10));
        }
        m_data = GetComponent<LevelData>();
        m_data.Initialize();
        m_builder = GetComponent<RoomBuilder>();

        Random.InitState(GameData.GetConstructionSeed());
        Debug.Log("Seed: " + GameData.GetConstructionSeed());
        Initiate(m_rooms[0]);
        //for(int i = 0; i < 1; i++)
        //{
        //    m_BossSpawned = false;
        //    Initiate(m_BossRoom);
        //}
    }

    void Initiate(Room originRoom)
    {
        originRoom.OpenAllEntrances(); originRoom.Initialize(originRoom.transform.position);
        SpawnRooms(Random.Range((int)m_data.GetRoomAmountCap().x + m_rooms.Count, (int)m_data.GetRoomAmountCap().y + m_rooms.Count));
        FuseRooms();
        AdjustRoomTypes();
        AdjustEntrances();
        m_builder.Build(m_rooms);
    }
    void SpawnRooms(int amountOfRooms)
    {
        //this spawns all rooms
        for (int i = m_rooms.Count; i < amountOfRooms; i++)
        {
            Room originRoom = GetRandomRoomInList();
            m_rooms.Add(Instantiate(m_RoomPrefab, transform));
            //Debug.Log(i);
            m_rooms[i].name = "Room #" + m_numberOfRooms; m_numberOfRooms++;
            Vector2 newCoordinates = GetNewRoomCoordinates(originRoom.GetLocation(), originRoom.GetDirections());
            while(true)
            {
                if(newCoordinates != new Vector2(0,0))
                {
                    m_rooms[i].Initialize(newCoordinates);
                    break;
                }
                else
                {
                    originRoom = GetRandomRoomInList();
                    newCoordinates = GetNewRoomCoordinates(originRoom.GetLocation(), originRoom.GetDirections());
                }
            }
            m_rooms[i].SetDistance(originRoom.GetDistance() + 1);
            if(m_rooms[i].GetDistance() > m_furthestDistanceFromSpawn)
            {
                m_furthestDistanceFromSpawn = m_rooms[i].GetDistance();
            }
            SetEntrances(originRoom, m_rooms[i]);
            LinkRoom(m_rooms[i]);
            OpenRandomEntrances(m_rooms[i]);
        }
    }

    void SetEntrances(Room RoomA, Room RoomB)
    {
        if (RoomA.GetLocation().x > RoomB.GetLocation().x)
        {
            //New room is to the right?
            if(RoomA.GetDirections())
            {
                RoomA.GetDirections().m_directions[2].Open = true;
                RoomA.GetDirections().m_directions[2].Spawned = true;
            }
            if(RoomB.GetDirections())
            {
                RoomB.GetDirections().m_directions[1].Open = true;
                RoomB.GetDirections().m_directions[1].Spawned = true;
            }
        }
        else if(RoomA.GetLocation().x < RoomB.GetLocation().x)
        {
            //New room is to the left?
            if(RoomA.GetDirections())
            {
                RoomA.GetDirections().m_directions[1].Open = true;
                RoomA.GetDirections().m_directions[1].Spawned = true;
            }
            if(RoomB.GetDirections())
            {
                RoomB.GetDirections().m_directions[2].Open = true;
                RoomB.GetDirections().m_directions[2].Spawned = true;
            }
        }
        else
        {
            if(RoomA.GetLocation().y > RoomB.GetLocation().y)
            {
                //New room is down
                if(RoomA.GetDirections())
                {
                    RoomA.GetDirections().m_directions[3].Open = true;
                    RoomA.GetDirections().m_directions[3].Spawned = true;
                }
                if(RoomB.GetDirections())
                {
                    RoomB.GetDirections().m_directions[0].Open = true;
                    RoomB.GetDirections().m_directions[0].Spawned = true;
                }
            }
            else if (RoomA.GetLocation().y < RoomB.GetLocation().y)
            {
                //New room is up
                if(RoomA.GetDirections())
                {
                    RoomA.GetDirections().m_directions[0].Open = true;
                    RoomA.GetDirections().m_directions[0].Spawned = true;
                }
                if(RoomB.GetDirections())
                {
                    RoomB.GetDirections().m_directions[3].Open = true;
                    RoomB.GetDirections().m_directions[3].Spawned = true;
                }
            }
            else
            {
                Debug.LogWarning("These are either the same room, or on top of eachother!");
            }
        }
    }

    Vector2 GetNewRoomCoordinates(Vector2 originCoordinates, RoomDirections directionsOfRoom)
    {
        //This functions chooses one of the unoccupied directions around that room
        //When it does, it should change that rooms m_direction to say that the opposing entrance is both open and spawned
        //Debug.Log("Getting coordinates for new room");
        List<Vector2> possibleCoordinates = new List<Vector2> { };
        for(int i = 0; i < 4; i++)
        {
            if (directionsOfRoom.m_directions[i].Open && !directionsOfRoom.m_directions[i].Spawned)
            {
                if(!CheckIfCoordinatesOccupied(new Vector2(originCoordinates.x + directionsOfRoom.m_directions[i].DirectionModifier.x * 20, originCoordinates.y + directionsOfRoom.m_directions[i].DirectionModifier.y * 20)))
                {
                    possibleCoordinates.Add(new Vector2(originCoordinates.x + directionsOfRoom.m_directions[i].DirectionModifier.x * 20, originCoordinates.y + directionsOfRoom.m_directions[i].DirectionModifier.y * 20));
                }
            }
        }
        int index = Random.Range(0, possibleCoordinates.Count - 1);
        if(possibleCoordinates.Count > 0)
        {
            return possibleCoordinates[index];
        }
        else
        {
            return new Vector2(0,0);
        }
    }

    bool CheckIfCoordinatesOccupied(Vector2 roomPosition)
    {
        foreach (Room room in m_rooms)
        {
            if(room.GetLocation() == roomPosition)
            {
                return true;
            }
        }
        return false;
    }

    void LinkRoom(Room room)
    {
        //This function checks if this given room has another spawned room in any direction that it must link to, before it decides if it should link anywhere else
        //It does this by checking if a room in any direction has an open but not spawned gate in its own direction, in which case it opens its own gate in that direction
        for (int i = 0; i < m_rooms.Count; i++)
        {
            if(m_rooms[i].GetLocation() == new Vector2(room.GetLocation().x + 20, room.GetLocation().y))
            {
                if (m_rooms[i].GetDirections().m_directions[2] == null)
                {
                    continue;
                }
                if (m_rooms[i].GetDirections().m_directions[2].Open)
                {
                    SetEntrances(room, m_rooms[i]);
                    if (m_rooms[i].GetDistance() < room.GetDistance() - 1)
                    {
                        room.SetDistance(m_rooms[i].GetDistance() + 1);
                    }
                }
                else
                {
                    room.GetDirections().m_directions[1].Open = false;
                    room.GetDirections().m_directions[1].Spawned = true;
                }
            }
            else if (m_rooms[i].GetLocation() == new Vector2(room.GetLocation().x - 20, room.GetLocation().y))
            {
                if(m_rooms[i].GetDirections().m_directions[1] == null)
                {
                    continue;
                }
                if (m_rooms[i].GetDirections().m_directions[1].Open)
                {
                    SetEntrances(room, m_rooms[i]);
                    if (m_rooms[i].GetDistance() < room.GetDistance() - 1)
                    {
                        room.SetDistance(m_rooms[i].GetDistance() + 1);
                    }
                }
                else
                {
                    room.GetDirections().m_directions[2].Open = false;
                    room.GetDirections().m_directions[2].Spawned = true;
                }
            }
            else if (m_rooms[i].GetLocation() == new Vector2(room.GetLocation().x, room.GetLocation().y + 20))
            {
                if (m_rooms[i].GetDirections().m_directions[3] == null)
                {
                    continue;
                }
                if (m_rooms[i].GetDirections().m_directions[3].Open)
                {
                    SetEntrances(room, m_rooms[i]);
                    if (m_rooms[i].GetDistance() < room.GetDistance() - 1)
                    {
                        room.SetDistance(m_rooms[i].GetDistance() + 1);
                    }
                }
                else
                {
                    room.GetDirections().m_directions[0].Open = false;
                    room.GetDirections().m_directions[0].Spawned = true;
                }
            }
            else if (m_rooms[i].GetLocation() == new Vector2(room.GetLocation().x, room.GetLocation().y - 20))
            {
                if(m_rooms[i].GetDirections().m_directions[0] == null)
                {
                    continue;
                }
                if (m_rooms[i].GetDirections().m_directions[0].Open)
                {
                    SetEntrances(room, m_rooms[i]);
                    if (m_rooms[i].GetDistance() < room.GetDistance() - 1)
                    {
                        room.SetDistance(m_rooms[i].GetDistance() + 1);
                    }
                }
                else
                {
                    room.GetDirections().m_directions[3].Open = false;
                    room.GetDirections().m_directions[3].Spawned = true;
                }
            }
        }
    }

    void OpenRandomEntrances(Room room)
    {
        List<RoomEntrance> possibleEntrancesToOpen = new List<RoomEntrance> { };
        if(!room.GetDirections())
        {
            return;
        }
        foreach(RoomEntrance entrance in room.GetDirections().m_directions)
        {
            if(entrance.Open == false && entrance.Spawned == false)
            {
                possibleEntrancesToOpen.Add(entrance);
            }
        }
        if (possibleEntrancesToOpen.Count != 0)
        {
            for (int i = 0; i < Random.Range(4 - possibleEntrancesToOpen.Count, 5); i++)
            {
                possibleEntrancesToOpen[Random.Range(0, possibleEntrancesToOpen.Count)].Open = true;
            }
        }
    }

    void FuseRooms()
    {
        //This function chooses a few random rooms and checks if they have rooms to the sides
        //It will then adjust camera boundaries

        int amountOfAttemptsForFusedRooms = Random.Range(1, 10);
        Debug.Log("I will attempt " + amountOfAttemptsForFusedRooms + " times to make fused rooms");

        for(int i = 0; i < amountOfAttemptsForFusedRooms; i++)
        {
            Room roomToFuse = GetRandomRoomInListNorthOrRight();
            Debug.Log("Room to fuse: " + roomToFuse + " Location: " + roomToFuse.GetLocation());

            List<Room> roomsRight = new List<Room> { };
            List<Room> roomsNorth = new List<Room> { }; //going down, the code must check ALL doors in rooms to the right and if they go down

            int amountOfStepsToRight = Random.Range(0, 2);
            int amountOfStepsToNorth = Random.Range(0, 2);
            Debug.Log(amountOfStepsToRight + " and " + amountOfStepsToNorth);

            for (int j = 0; j < amountOfStepsToRight; j++)
            {
                Room newRoom = FindAdjacentRoom(roomToFuse, new Vector2(1, 0));
                if (newRoom != null && newRoom.GetCameraBoundaries() != Vector2.zero)
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
                    if (newRoom != null && newRoom.GetCameraBoundaries() != Vector2.zero)
                    {
                        Debug.Log("This rooms: " + newRoom.name + " camera boundaries are: " + newRoom.GetCameraBoundaries());
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
                    List<Room> newRooms = new List<Room> { };
                    newRooms.Add(FindAdjacentRoom(roomToFuse, new Vector2(0, 1)));
                    if(newRooms[0] == null)
                    {
                        break;
                    }
                    if(newRooms[0].GetCameraBoundaries() == Vector2.zero)
                    {
                        newRooms.Clear();
                        break;
                    }
                    foreach (Room room in roomsRight)
                    {
                        Debug.Log("Adding rooms");
                        if (room.GetDirections().m_directions[0].Open == true && room.GetDirections().m_directions[0].Spawned == true)
                        {
                            Room newRoom = FindAdjacentRoom(room, new Vector2(0, 1));
                            if(newRoom.GetCameraBoundaries() != Vector2.zero)
                            {
                                newRooms.Add(newRoom);
                            }
                        }
                        else
                        {
                            //Not all rooms lead down
                            newRooms.Clear();
                        }
                    }
                    
                    roomsNorth = newRooms;
                    foreach(Room room in roomsNorth)
                    {
                        Debug.Log(room);
                    }
                }
            }

            Vector2 steps = new Vector2(roomsRight.Count, roomsNorth.Count);

            if (roomsRight.Count > 0)
            {
                for (int j = 0; j < steps.x; j++)
                {
                    Debug.Log("Room to fuse with to the right: " + roomsRight[j] + " Location: " + roomsRight[j].GetLocation());

                    roomToFuse.ExpandCameraBoundaries(roomsRight[j].GetCameraBoundaries(), roomsRight[j].transform.position);
                    roomsRight[j].EmptyCameraBoundaries();
                    roomToFuse.FuseWallPositions(roomsRight[j].GetWallPositions(), roomsRight[j].transform.position);
                    roomToFuse.FuseDirections(roomsRight[j].GetDirections(), roomsRight[j].transform.position);
                }
            }
            if (roomsNorth.Count > 0)
            {
                for (int j = 0; j < steps.y; j++)
                {
                    Debug.Log("Room to fuse with to the north: " + roomsNorth[j] + " Location: " + roomsNorth[j].GetLocation());

                    roomToFuse.ExpandCameraBoundaries(roomsNorth[j].GetCameraBoundaries(), roomsNorth[j].transform.position);
                    roomsNorth[j].EmptyCameraBoundaries();
                    roomToFuse.FuseWallPositions(roomsNorth[j].GetWallPositions(), roomsNorth[j].transform.position);
                    roomToFuse.FuseDirections(roomsNorth[j].GetDirections(), roomsNorth[j].transform.position);
                }
            }
        }
    }

    void AdjustRoomTypes()
    {
        //This function fixes walls, puts in obstacles and also enemies, as well as just normal shrubbery and shit
        //Debug.Log("Adjusting Rooms!");
        foreach(Room room in m_rooms)
        {
            room.ChooseRoomType(m_data);
            room.ChooseRoomLayout(m_data);
            if (room.GetRoomPositionType() == RoomPosition.DeadEnd)
            {
                if(room.GetDistance() == m_furthestDistanceFromSpawn && !m_BossSpawned)
                {
                    //Set the room furthest from spawn to be the bossroom
                    room.SetRoomType(RoomType.BossRoom);
                    m_BossRoom = room;
                    m_BossSpawned = true;
                    Debug.Log("Boss spawned in: " + m_BossRoom);
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
                    int random = Random.Range(0, 5);
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
                if (room.GetDistance() == m_furthestDistanceFromSpawn && !m_BossSpawned)
                {
                    room.SetRoomType(RoomType.BossRoom);
                    m_BossRoom = room;
                    m_BossSpawned = true;
                    Debug.Log("Boss spawned in: " + m_BossRoom);
                }
            }
        }
    }
    void AdjustEntrances()
    {
        foreach(Room room in m_rooms)
        {
            switch (room.GetRoomType())
            {
                case RoomType.TreasureRoom:
                    if(room.GetRoomPositionType() != RoomPosition.DeadEnd)
                    {
                        foreach (RoomEntrance entrance in room.GetDirections().m_directions)
                        {
                            if (entrance != null)
                            {
                                entrance.SetEntranceType(EntranceType.BombableWall, GetComponent<EntranceLibrary>());
                            }
                        }
                    }
                    break;
                case RoomType.RestingRoom:
                    break;
                case RoomType.AmbushRoom:
                    foreach (RoomEntrance entrance in room.GetDirections().m_directions)
                    {
                        if(entrance != null)
                        {
                            entrance.SetEntranceType(EntranceType.AmbushDoor, GetComponent<EntranceLibrary>());
                        }
                    }
                    break;
                case RoomType.BossRoom:
                    foreach (RoomEntrance entrance in room.GetDirections().m_directions)
                    {
                        if (entrance != null)
                        {
                            entrance.SetEntranceType(EntranceType.MultiLockedDoor, GetComponent<EntranceLibrary>());
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
    Room GetRandomRoomInList()
    {
        //This functions gets any of the rooms that are already spawned
        //It should make sure that it doesnt have something spawned in each direction
        //Debug.Log("Getting the coordinates of a random spawned room");
        if (m_rooms.Count != 0)
        {
            List<Room> roomWithOpenDoors = new List<Room> { };
            foreach (Room room in m_rooms)
            {
                if (room.GetIfHasOneOpenEntrance())
                {
                    roomWithOpenDoors.Add(room);
                }
            }
            return roomWithOpenDoors[Random.Range(0, roomWithOpenDoors.Count - 1)];
        }
        return m_rooms[0];
    }
    Room GetRandomRoomInListNorthOrRight()
    {
        if (m_rooms.Count != 0)
        {
            List<Room> roomWithOpenDoors = new List<Room> { };
            foreach (Room room in m_rooms)
            {
                if(room.GetDirections().m_directions[2] == null)
                {
                    continue;
                }
                if (room.GetDirections().m_directions[2].Open && room.GetDirections().m_directions[2].Spawned)
                {
                    roomWithOpenDoors.Add(room);
                }
                else if (room.GetDirections().m_directions[0].Open && room.GetDirections().m_directions[0].Spawned)
                {
                    roomWithOpenDoors.Add(room);
                }
            }
            return roomWithOpenDoors[Random.Range(0, roomWithOpenDoors.Count - 1)];
        }
        return m_rooms[0];
    }
    Room FindAdjacentRoom(Room origin)
    {
        RoomEntrance openEntrance = null;
        foreach (RoomEntrance entrance in origin.GetDirections().m_directions)
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
                    Debug.Log("This rooms boundaries was zero");
                    if(newRoom.GetDirections().m_directions[3])
                    {
                        newRoom = FindRoomOfPosition((Vector2)newRoom.transform.position + newRoom.GetDirections().m_directions[3].DirectionModifier * 20);
                    }
                }
            }
            return newRoom;
        }
        return null;
    }
    Room FindAdjacentRoom(Room origin, Vector2 direction)
    {
        return FindRoomOfPosition((Vector2)origin.transform.position + direction * 20);
    }
    Room FindAdjacentRoomNorthOrRight(Room origin)
    {
        RoomEntrance openEntrance = null;
        if (origin.GetDirections().m_directions[0].Open && origin.GetDirections().m_directions[0].Spawned)
        {
            openEntrance = origin.GetDirections().m_directions[0];
        }
        else if (origin.GetDirections().m_directions[1].Open && origin.GetDirections().m_directions[1].Spawned)
        {
            openEntrance = origin.GetDirections().m_directions[1];
        }
        if (openEntrance != null)
        {
            return FindRoomOfPosition((Vector2)origin.transform.position + openEntrance.DirectionModifier * 20);
        }
        return null;
    }
    Room FindRoomOfPosition(Vector2 position)
    {
        foreach (Room room in m_rooms)
        {
            if ((Vector2)room.transform.position == position)
            {
                return room;
            }
        }
        return null;
    }
}