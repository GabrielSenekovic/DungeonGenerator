using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum RoomType
{
    NormalRoom = 0,
    AmbushRoom = 1,
    TreasureRoom = 2, //without puzzle
    PuzzleRoom = 3, //Solve puzzle to get treasure
    BossRoom = 4,
    MiniBossRoom = 5,
    RestingRoom = 6 //Room where enemies cant spawn, and where you can set up a tent. Sometimes theres a merchant here
}

public enum RoomPosition
{
    None = 0,
    DeadEnd = 1
}

public enum RoomLayout
{
    NormalOutdoors = 0,
    NormalIndoors = 1,
    Corridor = 2
}
//Core code
public partial class Room: MonoBehaviour
{
    List<List<WallPosition>> m_wallPositions = new List<List<WallPosition>> { };
    /*
     * 0 = top wall
     * 1 = left wall
     * 2 = right wall
     * 3 = bottom wall
     */
    [SerializeField] WallPosition m_wallPosition; //This will not be here later ofc

    RoomDirections m_directions;

    [SerializeField] RoomType m_type = RoomType.NormalRoom;
    RoomPosition m_roomPosition = RoomPosition.None;
    RoomLayout m_layout = RoomLayout.NormalOutdoors;

    bool m_Indoors = false;

    [SerializeField] Vector2 m_cameraBoundaries;

    int m_StepsAwayFromMainRoom = 0;

    void BuildWallArray()
    {
        for(int i = 0; i < m_cameraBoundaries.x; i++)
        {
            m_wallPositions.Add(new List<WallPosition> { });
            for(int j = 0; j < m_cameraBoundaries.y; j++)
            {
                WallPosition newWall = Instantiate(m_wallPosition, new Vector2(transform.position.x + i, transform.position.y + j), Quaternion.identity, transform);
                m_wallPositions[i].Add(newWall);
                m_wallPositions[i][j].SetPosition(new Vector2(transform.position.x + i, transform.position.y + j));
            }
        }
    }

    public bool IsBuilt = false;

    public void SetDistance(int distance)
    {
        m_StepsAwayFromMainRoom = distance;
    }
    public int GetDistance()
    {
        return m_StepsAwayFromMainRoom;
    }

    public void OpenAllEntrances()
    {
        if(!m_directions)
        {
            m_directions = GetComponent<RoomDirections>();
        }
        m_directions.OpenAllEntrances();
    }

    public void Initialize(Vector2 location)
    {
        m_cameraBoundaries = new Vector2(20, 20);
        m_directions = GetComponent<RoomDirections>();
        BuildWallArray();
        transform.position = location;
    }

    public Vector2 GetCameraBoundaries()
    {
        return m_cameraBoundaries;
    }

    public List<List<WallPosition>> GetWallPositions()
    {
        return m_wallPositions;
    }

    public Vector2 GetLocation()
    {
        return transform.position;
    }

    public RoomPosition GetRoomPositionType()
    {
        return m_roomPosition;
    }

    public RoomDirections GetDirections()
    {
        return m_directions;
    }

    bool GetIsEndRoom()
    {
        List<RoomEntrance> entrances = new List<RoomEntrance> { };
        if(!m_directions)
        {
            return false;
        }
        foreach(RoomEntrance entrance in m_directions.m_directions)
        {
            if(entrance == null)
            {
                continue;
            }
            if(entrance.Spawned == true && entrance.Open == true)
            {
                entrances.Add(entrance);
            }
        }
        return entrances.Count == 1;
    } 

    public void ChooseRoomType(LevelData data)
    {
        List<RoomType> probabilityList = new List<RoomType> { };

        if (GetIsEndRoom())
        {
            m_roomPosition = RoomPosition.DeadEnd;
            probabilityList.Add(RoomType.TreasureRoom);
            probabilityList.Add(RoomType.AmbushRoom);
        }
        else
        {
            for(int i = 0; i < data.GetAmbushRoomProbability(); i++)
            {
                probabilityList.Add(RoomType.AmbushRoom);
            }
            for (int i = 0; i < data.GetTreasureRoomProbability(); i++)
            {
                probabilityList.Add(RoomType.TreasureRoom);
            }
            for (int i = 0; i < data.GetSafeRoomProbability(); i++)
            {
                probabilityList.Add(RoomType.RestingRoom);
            }
            for (int i = 0; i < data.GetNormalRoomProbability(); i++)
            {
                probabilityList.Add(RoomType.NormalRoom);
            }
            if (m_StepsAwayFromMainRoom > 5)
            {
                probabilityList.Add(RoomType.MiniBossRoom);
                probabilityList.Add(RoomType.AmbushRoom);
            }
        }
        m_type = probabilityList[Random.Range(0, probabilityList.Count)];
    }
    public void ChooseRoomLayout(LevelData data)
    {
        if(data.m_location == LevelLocation.Dungeon)
        {
            m_Indoors = CheckIfRoomIsIndoors(data);
            List<RoomLayout> layouts = new List < RoomLayout >{ };
            if(m_Indoors)
            {
                if(data.GetMood(0) == Mood.Decrepit || data.GetMood(1) == Mood.Decrepit)
                {
                    for(int i = 0; i < 3; i++)
                    {
                        layouts.Add(RoomLayout.NormalOutdoors);
                    }
                    if(m_roomPosition != RoomPosition.DeadEnd && m_type != RoomType.AmbushRoom)
                    {
                        layouts.Add(RoomLayout.Corridor);
                    }
                }
                for(int i = 0; i < 3; i++)
                {
                    layouts.Add(RoomLayout.NormalIndoors);
                    layouts.Add(RoomLayout.NormalIndoors);
                    layouts.Add(RoomLayout.NormalIndoors);
                }
                if (m_roomPosition != RoomPosition.DeadEnd && m_type != RoomType.AmbushRoom)
                {
                    layouts.Add(RoomLayout.Corridor);
                }
            }
            int temp = Random.Range(0, layouts.Count);
            m_layout = layouts[temp];
        }
    }
    public bool CheckIfRoomIsIndoors(LevelData data)
    {
        List<bool> boolsToCheckIfIndoors = new List<bool> { };

        return true;
    }
    public void SetRoomType(RoomType newType)
    {
        m_type = newType;
    }
    public RoomType GetRoomType()
    {
        return m_type;
    }

    public bool GetIfHasOneOpenEntrance()
    {
        if(m_directions == null)
        {
            return false;
        }
        foreach(RoomEntrance entrance in m_directions.m_directions)
        {
            if(entrance == null)
            {
                continue;
            }
            if (entrance.Open && !entrance.Spawned)
            {
                return true;
            }
        }
        return false;
    }
    public void DisplayDistance()
    {
        //GetComponentInChildren<Number>().OnDisplayNumber(m_StepsAwayFromMainRoom);
    }
}
//Code pertaining to the fusion of rooms
public partial class Room: MonoBehaviour
{
    public void ExpandCameraBoundaries(Vector2 boundariesToExpandWith, Vector2 destination)
    {
        if (m_cameraBoundaries.x <= boundariesToExpandWith.x)
        {
            if (transform.position.x < destination.x)
            {
                m_cameraBoundaries.x += boundariesToExpandWith.x;
            }
        }
        if (m_cameraBoundaries.y <= boundariesToExpandWith.y)
        {
            if (transform.position.y < destination.y)
            {
                m_cameraBoundaries.y += boundariesToExpandWith.y;
            }
        }
    }
    public void EmptyCameraBoundaries()
    {
        m_cameraBoundaries = Vector2.zero;
    }
    public void FuseWallPositions(List<List<WallPosition>> positionsToAdd, Vector2 position)
    {
        // Debug.Log("X Reach of this room: " + (transform.position.x + m_wallPositions.Count) + " X Position of other room: " + position.x);
        if (transform.position.x + m_wallPositions.Count == position.x)
        {
            //this room is to the left of the room with the new positions

            for (int i = 0; i < positionsToAdd.Count; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    positionsToAdd[i][j].SetPosition(new Vector2(positionsToAdd[i][j].GetPosition().x + 20, positionsToAdd[i][j].GetPosition().y));
                }
                m_wallPositions.Add(positionsToAdd[i]);
            }
        }

        if (transform.position.y < position.y)
        {
            //This room is below the room with the new positions
            //the index of positionsToAdd must begin at the right x
            int startIndex = (int)position.x - (int)transform.position.x;
            Debug.Log("Start Index is: " + startIndex);
            for (int i = startIndex; i < startIndex + 20; i++)
            {
                //i represents the x value of this rooms walls
                for (int j = 0; j < positionsToAdd[0].Count; j++)
                {
                    //j represents the y value of this rooms walls, which we are adding to
                    //the x index of the wall array were copying from still has to start at 0
                    {
                        positionsToAdd[i - startIndex][j].SetPosition(new Vector2(positionsToAdd[i - startIndex][j].GetPosition().x + startIndex, positionsToAdd[i - startIndex][j].GetPosition().y + 20));
                        m_wallPositions[i].Add(positionsToAdd[i - startIndex][j]);
                    }
                }
            }

        }
        //Debug.Log("Wall Position Length: " + m_wallPositions.Count + " and at [0]: " + m_wallPositions[0].Count);
        //  Debug.Log("Wall Position Length: " + m_wallPositions.Count + " and at [31]: " + m_wallPositions[31].Count);
    }
    public void FuseDirections(RoomDirections newDirections, Vector2 destination)
    {
        if (destination.y > transform.position.y && destination.x == transform.position.x)
        {
            if (m_directions.m_directions[0])
            {
                Destroy(m_directions.m_directions[0].gameObject);
                m_directions.m_directions[0] = newDirections.m_directions[0];
            }
            if (newDirections.m_directions[1])
            {
                m_directions.m_directions.Add(newDirections.m_directions[1]);
                newDirections.m_directions[1].transform.parent = transform;
            }
            if (newDirections.m_directions[2])
            {
                m_directions.m_directions.Add(newDirections.m_directions[2]);
                newDirections.m_directions[2].transform.parent = transform;
            }
            if (newDirections.m_directions[3])
            {
                Destroy(newDirections.m_directions[3].gameObject);
            }
        }
        if (destination.x > transform.position.x && destination.y == transform.position.y)
        {
            if (m_directions.m_directions[1])
            {
                Destroy(m_directions.m_directions[1].gameObject);
                m_directions.m_directions[1] = newDirections.m_directions[1];
            }
            if (newDirections.m_directions[0])
            {
                m_directions.m_directions.Add(newDirections.m_directions[0]);
                newDirections.m_directions[0].transform.parent = transform;
            }
            m_directions.m_directions.Add(newDirections.m_directions[3]);
            if (newDirections.m_directions[3])
            {
                newDirections.m_directions[3].transform.parent = transform;
            }

            if (newDirections.m_directions[2])
            {
                Destroy(newDirections.m_directions[2].gameObject);
            }
        }
        if (destination.y > transform.position.y && destination.x > transform.position.x)
        {
            if (m_directions.m_directions[4])
            {
                Destroy(m_directions.m_directions[4].gameObject);
            }
            if (m_directions.m_directions[6])
            {
                Destroy(m_directions.m_directions[6].gameObject);
            }
            if (newDirections.m_directions[0])
            {
                m_directions.m_directions[4] = newDirections.m_directions[0];
                newDirections.m_directions[0].transform.parent = transform;
            }
            if (newDirections.m_directions[1])
            {
                m_directions.m_directions[6] = newDirections.m_directions[1];
                newDirections.m_directions[1].transform.parent = transform;
            }

            if (newDirections.m_directions[2])
            {
                Destroy(newDirections.m_directions[2].gameObject);
            }
            if (newDirections.m_directions[3])
            {
                Destroy(newDirections.m_directions[3].gameObject);
            }
        }
    }
}
//The building of the room itself
public partial class Room: MonoBehaviour 
{
    public void PlaceDownWalls()
    {
        PlaceDownWallFrame();
        PlaceDownInnerWalls();
        if (m_layout == RoomLayout.NormalOutdoors)
        {
            for (int i = 0; i < 3; i++)
            {
                AdjustWalls();
            }
        }
        DetermineWallVariant();
    }

    public void PlaceDownWallFrame()
    {
        int roomWidth = (int)m_cameraBoundaries.x;
        int roomHeigth = (int)m_cameraBoundaries.y;
        int j = 0;

        List<RoomEntrance> entrancesToTheSouth = new List<RoomEntrance> { };

        if (!m_directions)
        {
            return;
        }
        foreach (RoomEntrance entrance in m_directions.m_directions)
        {
            if (entrance.DirectionModifier == new Vector2(0, -1))
            {
                entrancesToTheSouth.Add(entrance);
            }
        }

        for (int i = 0; i < roomWidth; i++)
        {
            if (i == 9 + j * 20)
            {
                entrancesToTheSouth[j].gameObject.transform.position = new Vector2(transform.position.x + i, transform.position.y);
                if (entrancesToTheSouth[j].Open == true)
                {
                    if (entrancesToTheSouth[j].Spawned == true)
                    {
                        continue;
                    }
                }
            }
            if (i == 10 + j * 20)
            {
                j++;
                if (entrancesToTheSouth[j - 1].Open == true)
                {
                    if (entrancesToTheSouth[j - 1].Spawned == true)
                    {
                        continue;
                    }
                }
            }
            m_wallPositions[i][0].PlaceDown(new Vector2(transform.position.x + i, transform.position.y));
        }
        j = 0;

        List<RoomEntrance> entrancesToTheRight = new List<RoomEntrance> { };

        foreach (RoomEntrance entrance in m_directions.m_directions)
        {
            if (entrance.DirectionModifier == new Vector2(-1, 0))
            {
                entrancesToTheRight.Add(entrance);
            }
        }
        for (int i = 1; i < roomHeigth; i++)
        {
            if (i == 9 + j * 20)
            {
                entrancesToTheRight[j].gameObject.transform.position = new Vector2(transform.position.x, transform.position.y + i);
                if (entrancesToTheRight[j].Open == true)
                {
                    if (entrancesToTheRight[j].Spawned == true)
                    {
                        continue;
                    }
                }
            }
            if (i == 10 + j * 20)
            {
                j++;
                if (entrancesToTheRight[j - 1].Open == true)
                {
                    if (entrancesToTheRight[j - 1].Spawned == true)
                    {
                        continue;
                    }
                }
            }
            m_wallPositions[0][i].PlaceDown(new Vector2(transform.position.x, transform.position.y + i));
        }
        j = 0;

        List<RoomEntrance> entrancesToTheLeft = new List<RoomEntrance> { };

        foreach (RoomEntrance entrance in m_directions.m_directions)
        {
            if (entrance.DirectionModifier == new Vector2(1, 0))
            {
                entrancesToTheLeft.Add(entrance);
            }
        }
        for (int i = 1; i < roomHeigth; i++)
        {
            if (i == 9 + j * 20)
            {
                entrancesToTheLeft[j].gameObject.transform.position = new Vector2(transform.position.x + roomWidth - 1, transform.position.y + i);
                if (entrancesToTheLeft[j].Open == true)
                {
                    if (entrancesToTheLeft[j].Spawned == true)
                    {
                        continue;
                    }
                }
            }
            if (i == 10 + j * 20)
            {
                j++;
                if (entrancesToTheLeft[j - 1].Open == true)
                {
                    if (entrancesToTheLeft[j - 1].Spawned == true)
                    {
                        continue;
                    }
                }
            }
            //if(gameObject.name == "Room #16")
            //{
            //    Debug.Log("Trying to put wall down at: " + (roomWidth - 1) + ", " + i);
            //    Debug.Log(m_wallPositions[31].Count);
            //}
            m_wallPositions[roomWidth - 1][i].PlaceDown(new Vector2(transform.position.x + roomWidth - 1, transform.position.y + i));
        }

        j = 0;

        List<RoomEntrance> entrancesToTheNorth = new List<RoomEntrance> { };

        foreach (RoomEntrance entrance in m_directions.m_directions)
        {
            if (entrance.DirectionModifier == new Vector2(0, 1))
            {
                entrancesToTheNorth.Add(entrance);
            }
        }

        for (int i = 1; i < roomWidth - 1; i++)
        {
            if (i == 9 + j * 20)
            {
                entrancesToTheNorth[j].gameObject.transform.position = new Vector2(transform.position.x + i, transform.position.y + roomHeigth - 1);
                if (entrancesToTheNorth[j].Open == true)
                {
                    if (entrancesToTheNorth[j].Spawned == true)
                    {
                        continue;
                    }
                }
            }
            if (i == 10 + j * 20)
            {
                j++;
                if (entrancesToTheNorth[j - 1].Open == true)
                {
                    if (entrancesToTheNorth[j - 1].Spawned == true)
                    {
                        continue;
                    }
                }
            }
            m_wallPositions[i][roomHeigth - 1].PlaceDown(new Vector2(transform.position.x + i, transform.position.y + roomHeigth - 1));
        }
    }
    public void PlaceDownInnerWalls()
    {
        //i should count up to the width of the room, j should count to the height
        //Should check to make sure that theres at least a tile to the x or the y, unless its spawning in the middle of the room
        Vector2 roomCenter = new Vector2(m_cameraBoundaries.x / 2, m_cameraBoundaries.y / 2);
        Vector2 wallThickness = new Vector2(Random.Range(m_cameraBoundaries.x/2 - 2, m_cameraBoundaries.x/2 + 1), Random.Range(m_cameraBoundaries.y/2 - 2, m_cameraBoundaries.y/2 + 1));
        int frameCorridorThickness = 0;
        int frameCorridorOffset = 0;
        int corridorThickness = 0;
        bool checkered = false;
        if(m_layout == RoomLayout.Corridor)
        {
            corridorThickness = Random.Range(1, 4);
            int temp = Random.Range(0, 4);
            if(temp == 0)
            {
                frameCorridorThickness = Random.Range(2, 4);
                frameCorridorOffset = Random.Range(0, 3);
                int temp2 = Random.Range(0, 2);
                if(temp2 == 0)
                {
                    checkered = true;
                }
            }
        }
        for (int i = 1; i < m_cameraBoundaries.x - 1; i++)
        {
            if(i == 1 + frameCorridorOffset || i == m_cameraBoundaries.x - 1 - frameCorridorThickness - frameCorridorOffset )
            {
                if(!checkered)
                {
                    for (int k = i; k < i + frameCorridorThickness; k++)
                    {
                        for (int l = 1; l < 1 + frameCorridorOffset; l++)
                        {
                            m_wallPositions[k][l].PlaceDown(new Vector2(transform.position.x + k, transform.position.y + l));
                        }
                        for (int l = (int)m_cameraBoundaries.y - 1 - frameCorridorOffset; l < m_cameraBoundaries.y - 1; l++)
                        {
                            m_wallPositions[k][l].PlaceDown(new Vector2(transform.position.x + k, transform.position.y + l));
                        }
                    }
                }
                i += frameCorridorThickness;
            }
            for (int j = 1; j < m_cameraBoundaries.y - 1; j++)
            {
                if (j == 1 + frameCorridorOffset || j == m_cameraBoundaries.y - 1 - frameCorridorThickness - frameCorridorOffset)
                {
                    if(!checkered)
                    {
                        for (int k = j; k < j + frameCorridorThickness; k++)
                        {
                            for (int l = 1; l < 1 + frameCorridorOffset; l++)
                            {
                                m_wallPositions[l][k].PlaceDown(new Vector2(transform.position.x + l, transform.position.y + k));
                            }
                            for (int l = (int)m_cameraBoundaries.x - 1 - frameCorridorOffset; l < m_cameraBoundaries.x - 1; l++)
                            {
                                m_wallPositions[l][k].PlaceDown(new Vector2(transform.position.x + l, transform.position.y + k));
                            }
                        }
                    }
                    j += frameCorridorThickness;
                }
                if (i - 3 < 0 || i + (int)m_cameraBoundaries.x - 3 > m_cameraBoundaries.x - 1)
                {
                    if (i > m_cameraBoundaries.x / 2 - 2 - corridorThickness)
                    {
                        //Controls right corridor of room
                        if (!m_wallPositions[(int)m_cameraBoundaries.x - 1][j].GetIsOccupied())
                        {
                            continue;
                        }
                        else
                        {
                            if(j - corridorThickness > 0 && j + corridorThickness < m_cameraBoundaries.y - 1)
                                //This only makes sure the index isnt out of range
                            {
                                bool toContinue = false;
                                for(int k = 0; k <= corridorThickness; k++) //Im not sure it makes sense to do this for every single j, but for now it works, but Id rather do this more methodically down the road
                                {
                                    if (!m_wallPositions[(int)m_cameraBoundaries.x - 1][j + k].GetIsOccupied())
                                    {
                                        toContinue = true;
                                    }
                                    else if (!m_wallPositions[(int)m_cameraBoundaries.x - 1][j - k].GetIsOccupied())
                                    {
                                        toContinue = true;
                                    }
                                }
                                if(toContinue)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    if (i < m_cameraBoundaries.x / 2 +1 + corridorThickness)
                    {
                        //Controls left corridor of room
                        if (!m_wallPositions[0][j].GetIsOccupied())
                        {
                            continue;
                        }
                        else
                        {
                            if (j - corridorThickness > 0 && j + corridorThickness < m_cameraBoundaries.y - 1)
                            {
                                bool toContinue = false;
                                for (int k = 0; k <= corridorThickness; k++) //Im not sure it makes sense to do this for every single j, but for now it works, but Id rather do this more methodically down the road
                                {
                                    if (!m_wallPositions[0][j + k].GetIsOccupied())
                                    {
                                        toContinue = true;
                                    }
                                    else if (!m_wallPositions[0][j - k].GetIsOccupied())
                                    {
                                        toContinue = true;
                                    }
                                }
                                if (toContinue)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
                if (j - 3 < 0 || j + (int)m_cameraBoundaries.y - 3 > m_cameraBoundaries.y - 1)
                {
                    if (j > m_cameraBoundaries.y / 2 -2) //-2
                    {
                        if (!m_wallPositions[i][(int)m_cameraBoundaries.y - 1].GetIsOccupied())
                        {
                            continue;
                        }
                        else
                        {
                            if (i - corridorThickness > 0 && i + corridorThickness < m_cameraBoundaries.x - 1)
                            //This only makes sure the index isnt out of range
                            {
                                bool toContinue = false;
                                for (int k = 0; k <= corridorThickness; k++) //Im not sure it makes sense to do this for every single j, but for now it works, but Id rather do this more methodically down the road
                                {
                                    if (!m_wallPositions[i + k][(int)m_cameraBoundaries.y - 1].GetIsOccupied())
                                    {
                                        toContinue = true;
                                    }
                                    else if (!m_wallPositions[i - k][(int)m_cameraBoundaries.y - 1].GetIsOccupied())
                                    {
                                        toContinue = true;
                                    }
                                }
                                if (toContinue)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    if (j < m_cameraBoundaries.y / 2+1 ) //+2
                    {
                        if (!m_wallPositions[i][0].GetIsOccupied())
                        {
                            continue;
                        }
                        else
                        {
                            if (i - corridorThickness > 0 && i + corridorThickness < m_cameraBoundaries.x - 1)
                            //This only makes sure the index isnt out of range
                            {
                                bool toContinue = false;
                                for (int k = 0; k <= corridorThickness; k++) //Im not sure it makes sense to do this for every single j, but for now it works, but Id rather do this more methodically down the road
                                {
                                    if (!m_wallPositions[i + k][0].GetIsOccupied())
                                    {
                                        toContinue = true;
                                    }
                                    else if (!m_wallPositions[i - k][0].GetIsOccupied())
                                    {
                                        toContinue = true;
                                    }
                                }
                                if (toContinue)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
                if(m_layout == RoomLayout.NormalOutdoors)
                {
                    BuildNormalLayout(i, j, roomCenter, wallThickness);
                }
                else if(m_layout == RoomLayout.Corridor)
                {
                    BuildCorridorLayout(i, j);
                }
            }
        }
    }

    public void BuildNormalLayout(int i, int j, Vector2 center, Vector2 wallThickness)
    {
        float distanceToCenter = new Vector2(i - center.x, j - center.y).magnitude;
        if (distanceToCenter > wallThickness.x && distanceToCenter > wallThickness.y)
        {
            int temp = Random.Range(0, 2);
            if (temp == 1)
            {
                m_wallPositions[i][j].PlaceDown(new Vector2(transform.position.x + i, transform.position.y + j));
            }
        }
    }
    public void BuildCorridorLayout(int i, int j)
    {
         m_wallPositions[i][j].PlaceDown(new Vector2(transform.position.x + i, transform.position.y + j));
    }

    public void AdjustWalls()
    {
        for (int i = 0; i < m_cameraBoundaries.x; i++)
        {
            for (int j = 0; j < m_cameraBoundaries.y; j++)
            {
                int amountOfAdjacentWalls = 0;
                if (i + 1 < m_cameraBoundaries.x)
                {
                    if (m_wallPositions[i + 1][j].GetIsOccupied())
                    {
                        amountOfAdjacentWalls++;
                    }
                }
                if (i - 1 >= 0)
                {
                    if (m_wallPositions[i - 1][j].GetIsOccupied())
                    {
                        amountOfAdjacentWalls++;
                    }
                }
                if (j + 1 < m_cameraBoundaries.y)
                {
                    if (m_wallPositions[i][j + 1].GetIsOccupied())
                    {
                        amountOfAdjacentWalls++;
                    }
                }
                if (j - 1 >= 0)
                {
                    if (m_wallPositions[i][j - 1].GetIsOccupied())
                    {
                        amountOfAdjacentWalls++;
                    }
                }
                if (amountOfAdjacentWalls == 0)
                {
                    m_wallPositions[i][j].UnPlace();
                }
                if (amountOfAdjacentWalls >= 3)
                {
                    m_wallPositions[i][j].PlaceDown(new Vector2(transform.position.x + i, transform.position.y + j));
                }
            }
        }
    }

    public void DetermineWallVariant()
    {
        for(int i = 0; i < m_wallPositions.Count; i++)
        {
            for(int j = 0; j < m_wallPositions[i].Count; j++)
            {
                string temp = "";
                if (m_wallPositions[i][j].GetIsOccupied())
                {
                    if(j+1 < m_cameraBoundaries.y)
                    {
                        if (m_wallPositions[i][j + 1].GetIsOccupied())
                        {
                            temp+='A';
                        }
                        else
                        {
                            temp+='B';
                        }
                    }
                    else
                    {
                        temp+='B';
                    }
                    if (i + 1 < m_cameraBoundaries.x)
                    {
                        if (m_wallPositions[i+1][j].GetIsOccupied())
                        {
                            temp+='A';
                        }
                        else
                        {
                            temp+='B';
                        }
                    }
                    else
                    {
                        temp+='B';
                    }
                    if (j - 1 >= 0)
                    {
                        if (m_wallPositions[i][j - 1].GetIsOccupied())
                        {
                            temp+='A';
                        }
                        else
                        {
                            temp+='B';
                        }
                    }
                    else
                    {
                        temp+='B';
                    }
                    if (i - 1 >= 0)
                    {
                        if (m_wallPositions[i-1][j].GetIsOccupied())
                        {
                            temp+='A';
                        }
                        else
                        {
                            temp+='B';
                        }
                    }
                    else
                    {
                        temp+='B';
                    }
                }
                switch (temp)
                {
                    case "AAAA":
                        m_wallPositions[i][j].SetVariant(WallVariant.TopCorner);
                        break; //inner wall
                    case "AAAB":
                        m_wallPositions[i][j].SetVariant(WallVariant.Side);
                        break;
                    case "AABA":
                        m_wallPositions[i][j].SetVariant(WallVariant.Bottom);
                        break;
                    case "AABB":
                        m_wallPositions[i][j].SetVariant(WallVariant.BottomLeft);
                        break; 
                    case "ABAA":
                        m_wallPositions[i][j].SetVariant(WallVariant.Side);
                        break;
                    case "ABAB":
                        m_wallPositions[i][j].SetVariant(WallVariant.Side);
                        break;
                    case "ABBA":
                        m_wallPositions[i][j].SetVariant(WallVariant.BottomRight);
                        break; 
                    case "ABBB": break;
                    case "BAAA":
                        m_wallPositions[i][j].SetVariant(WallVariant.TopCorner); //this is the top room
                        break;
                    case "BAAB":
                        m_wallPositions[i][j].SetVariant(WallVariant.TopCorner);
                        break;
                    case "BABA":
                        m_wallPositions[i][j].SetVariant(WallVariant.Bottom);
                        break;
                    case "BABB": break;
                    case "BBAA":
                        m_wallPositions[i][j].SetVariant(WallVariant.TopCorner);
                        break;
                    case "BBAB": break;
                    case "BBBA": break;
                    case "BBBB": break; //pillar
                }
            }
        }

    }

    public void InstantiateWalls(WallBlueprints blueprints)
    {
        for (int i = 0; i < m_cameraBoundaries.x; i++)
        {
            for (int j = 0; j < m_cameraBoundaries.y; j++)
            {
                if (m_wallPositions[i][j].GetIsOccupied())
                {
                    GameObject newWall = Instantiate(blueprints.wallBlock, new Vector2(transform.position.x, transform.position.y) + m_wallPositions[i][j].GetPosition(), Quaternion.identity, transform);
                    newWall.GetComponentInChildren<SpriteRenderer>().color = GetWallColor();
                    if (m_wallPositions[i][j].GetVariant() != WallVariant.None)
                    {
                        //Wall newWall = Instantiate(blueprints.GetWall(m_wallPositions[i][j].GetVariant()), new Vector2(transform.position.x, transform.position.y) + m_wallPositions[i][j].GetPosition(), Quaternion.identity, transform);

                        //for(int k = 0; k < newWall.GetAmountOfRenderers(); k++)
                        //{
                        //   newWall.ChangeColor(GetWallColor(), k);
                        //}
                    }
                }
            }
        }
    }
    public void InstantiateFloor(GameObject floorTile)
    {
        for (int i = 0; i < m_cameraBoundaries.x; i++)
        {
            for (int j = 0; j < m_cameraBoundaries.y; j++)
            {
                if (!m_wallPositions[i][j].GetIsOccupied())
                {
                    GameObject newWall = Instantiate(floorTile, new Vector2(transform.position.x, transform.position.y) + m_wallPositions[i][j].GetPosition(), Quaternion.identity, transform);
                    newWall.GetComponentInChildren<SpriteRenderer>().color = GetWallColor() * 0.6f;
                }
            }
        }
    }

    Color GetWallColor()
    {
        Color newColor = new Color(0, 0, 0);
        switch (m_type)
        {
            case RoomType.NormalRoom: newColor = new Color(0.1481844f, 0.513463f, 0.6981132f); break;
            case RoomType.RestingRoom: newColor = new Color(0.5947402f, 0.9339623f, 0.60267f); break;
            case RoomType.AmbushRoom:
                newColor = new Color(0.3773585f, 0.05161981f, 0.05161981f);
                break;
            case RoomType.BossRoom: newColor = new Color(0, 0, 0); break;
            case RoomType.MiniBossRoom: newColor = new Color(0.5f, 0.5f, 0.5f); break;
            case RoomType.PuzzleRoom: newColor = new Color(0.6980392f, 0.4338003f, 0.1490196f); break;
            case RoomType.TreasureRoom: newColor = new Color(0.9245283f, 0.7630849f, 0.1526344f); break;
            default: return new Color(0, 0, 0);
        }
        if (m_roomPosition == RoomPosition.DeadEnd)
        {
            newColor += new Color(0.3f, 0.3f, 0.3f);
        }
        return newColor;
    }
}
