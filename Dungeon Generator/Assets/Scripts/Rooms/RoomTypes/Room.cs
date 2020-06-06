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
    public List<List<WallPosition>> m_wallPositions = new List<List<WallPosition>> { };
    /*
     * 0 = top wall
     * 1 = left wall
     * 2 = right wall
     * 3 = bottom wall
     */
    public WallPosition wallPosition; //This will not be here later ofc

    RoomDirections directions;

    public Vector2 CameraBoundaries;
    public Vector2 wallPositionBoundaries = Vector2.zero;

    public RoomData roomData = new RoomData();

    void BuildWallArray()
    {
        for(int i = 0; i < CameraBoundaries.x; i++)
        {
            m_wallPositions.Add(new List<WallPosition> { });
            for(int j = 0; j < CameraBoundaries.y; j++)
            {
                WallPosition newWall = Instantiate(wallPosition, new Vector2(transform.position.x + i, transform.position.y + j), Quaternion.identity, transform);
                m_wallPositions[i].Add(newWall);
                //m_wallPositions[i][j].SetPosition(new Vector2(transform.position.x + i, transform.position.y + j));
            }
        }
    }

    public void OpenAllEntrances()
    {
        if(!directions)
        {
            directions = GetComponent<RoomDirections>();
        }
        directions.OpenAllEntrances();
    }
    public void Initialize(Vector2 RoomSize)
    {
        //This Initialize() function is for the origin room specifically, as it already has its own position
        OpenAllEntrances();
        OnInitialize(RoomSize);
    }

    public void Initialize(Vector2 location, Vector2 RoomSize)
    {
        transform.position = location;
        OnInitialize(RoomSize);
    }
    void OnInitialize(Vector2 RoomSize)
    {
        CameraBoundaries = RoomSize;
        directions = GetComponent<RoomDirections>();
        BuildWallArray();
    }

    public Vector2 GetCameraBoundaries()
    {
        return CameraBoundaries;
    }

    public List<List<WallPosition>> GetWallPositions()
    {
        return m_wallPositions;
    }

    public RoomPosition GetRoomPositionType()
    {
        return roomData.m_roomPosition;
    }

    public RoomDirections GetDirections()
    {
        return directions;
    }
    public List<RoomEntrance> GetOpenUnspawnedEntrances()
    {
        List<RoomEntrance> openEntrances = new List<RoomEntrance>{};
        foreach(RoomEntrance entrance in directions.directions)
        {
            if (entrance.Open && !entrance.Spawned)
            {
                openEntrances.Add(entrance);
            }
        }
        return openEntrances;
    }

    bool GetIsEndRoom()
    {
        List<RoomEntrance> entrances = new List<RoomEntrance> { };
        if(!directions)
        {
            return false;
        }
        foreach(RoomEntrance entrance in directions.directions)
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
            roomData.m_roomPosition = RoomPosition.DeadEnd;
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
            if (roomData.stepsAwayFromMainRoom > 5)
            {
                probabilityList.Add(RoomType.MiniBossRoom);
                probabilityList.Add(RoomType.AmbushRoom);
            }
        }
        roomData.m_type = probabilityList[Random.Range(0, probabilityList.Count)];
    }
    public bool CheckIfRoomIsIndoors(LevelData data)
    {
        List<bool> boolsToCheckIfIndoors = new List<bool> { };

        return true;
    }
    public void SetRoomType(RoomType newType)
    {
        roomData.m_type = newType;
    }
    public RoomType GetRoomType()
    {
        return roomData.m_type;
    }

    public bool GetIfHasOneOpenEntrance()
    {
        if(directions == null)
        {
            return false;
        }
        foreach(RoomEntrance entrance in directions.directions)
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
        //GetComponentInChildren<Number>().OnDisplayNumber(roomData.stepsAwayFromMainRoom);
    }
}
//Code pertaining to the fusion of rooms
public partial class Room: MonoBehaviour
{
    public void ExpandCameraBoundaries(Vector2 boundariesToExpandWith, Vector2 destination)
    {
        if(transform.position.x < destination.x && transform.position.y < destination.y)
        {
        }
        else if(transform.position.x < destination.x)
        {
            CameraBoundaries.x += boundariesToExpandWith.x;
        }
        else if(transform.position.y < destination.y)
        {
            CameraBoundaries.y += boundariesToExpandWith.y;
        }
        Debug.Log("Camera boundaries are now: " + CameraBoundaries);
    }
    public void EmptyCameraBoundaries()
    {
        CameraBoundaries = Vector2.zero;
    }
    public void FuseWallPositions(List<List<WallPosition>> positionsToAdd, Vector2 position, Vector2 RoomSize)
    {
        // Debug.Log("X Reach of this room: " + (transform.position.x + m_wallPositions.Count) + " X Position of other room: " + position.x);
        if (transform.position.x + m_wallPositions.Count == position.x)
        {
            //this room is to the left of the room with the new positions

            for (int i = 0; i < positionsToAdd.Count; i++)
            {
                m_wallPositions.Add(positionsToAdd[i]);
            }
        }

        if (transform.position.y < position.y)
        {
            //This room is below the room with the new positions
            //the index of positionsToAdd must begin at the right x
            Debug.Log("Positions to add [0] size: " + positionsToAdd[0].Count);
            int startIndex = (int)position.x - (int)transform.position.x;
            Debug.Log("Start Index is: " + startIndex);
            for (int i = startIndex; i < startIndex + RoomSize.y; i++)
            {
                //i represents the x value of this rooms walls
                for (int j = 0; j < positionsToAdd[0].Count; j++)
                {
                    //j represents the y value of this rooms walls, which we are adding to
                    //the x index of the wall array were copying from still has to start at 0
                    {
                        m_wallPositions[i].Add(positionsToAdd[i - startIndex][j]);
                    }
                }
            }
            Debug.Log("Index 0 is now: " + m_wallPositions[0].Count);
        }
    }
    public void FuseDirections(RoomDirections newDirections, Vector2 destination)
    {
        if (destination.y > transform.position.y && destination.x == transform.position.x)
        {
            if (directions.directions[0])
            {
                Destroy(directions.directions[0].gameObject);
                directions.directions[0] = newDirections.directions[0];
            }
            if (newDirections.directions[1])
            {
                directions.directions.Add(newDirections.directions[1]);
                newDirections.directions[1].transform.parent = transform;
            }
            if (newDirections.directions[2])
            {
                directions.directions.Add(newDirections.directions[2]);
                newDirections.directions[2].transform.parent = transform;
            }
            if (newDirections.directions[3])
            {
                Destroy(newDirections.directions[3].gameObject);
            }
        }
        if (destination.x > transform.position.x && destination.y == transform.position.y)
        {
            if (directions.directions[1])
            {
                Destroy(directions.directions[1].gameObject);
                directions.directions[1] = newDirections.directions[1];
            }
            if (newDirections.directions[0])
            {
                directions.directions.Add(newDirections.directions[0]);
                newDirections.directions[0].transform.parent = transform;
            }
            directions.directions.Add(newDirections.directions[3]);
            if (newDirections.directions[3])
            {
                newDirections.directions[3].transform.parent = transform;
            }

            if (newDirections.directions[2])
            {
                Destroy(newDirections.directions[2].gameObject);
            }
        }
        if (destination.y > transform.position.y && destination.x > transform.position.x)
        {
            if (directions.directions[4])
            {
                Destroy(directions.directions[4].gameObject);
            }
            if (directions.directions[6])
            {
                Destroy(directions.directions[6].gameObject);
            }
            if (newDirections.directions[0])
            {
                directions.directions[4] = newDirections.directions[0];
                newDirections.directions[0].transform.parent = transform;
            }
            if (newDirections.directions[1])
            {
                directions.directions[6] = newDirections.directions[1];
                newDirections.directions[1].transform.parent = transform;
            }

            if (newDirections.directions[2])
            {
                Destroy(newDirections.directions[2].gameObject);
            }
            if (newDirections.directions[3])
            {
                Destroy(newDirections.directions[3].gameObject);
            }
        }
    }
}
//The building of the room itself
public partial class Room: MonoBehaviour 
{
   public void PlaceDownWalls(WallBlueprints blueprints)
    {
        GameObject wallParent = new GameObject("Walls");
        wallParent.transform.SetParent(transform);

        if (!directions)
        {
            return;
        }

        OnPlaceDownWall(new Vector2(0, -1), (int)CameraBoundaries.x, 0, 1, 0, 0, 0, wallParent.transform, blueprints);
        OnPlaceDownWall(new Vector2(-1, 0), (int)CameraBoundaries.y, 1, 0, 1, 0, 0, wallParent.transform, blueprints);
        OnPlaceDownWall(new Vector2(1, 0), (int)CameraBoundaries.y, 1, 0, 1, (int)CameraBoundaries.x - 1 ,0, wallParent.transform, blueprints);
        OnPlaceDownWall(new Vector2(0, 1), (int)CameraBoundaries.x - 1, 1, 1, 0, 0 ,(int)CameraBoundaries.y - 1, wallParent.transform, blueprints);
        if(roomData.m_layout == RoomLayout.NormalOutdoors)
        {
            PlaceDownInnerWalls(blueprints, wallParent.transform);
            if (roomData.m_layout == RoomLayout.NormalOutdoors)
            {
                for (int i = 0; i < 3; i++)
                {
                    AdjustWalls();
                }
            }
        }
        //DetermineWallVariant();
    }
    public void OnPlaceDownWall(Vector2 entranceDirection, int limit, int startValue, 
    int x_modifier, int y_modifier, int x_offset, int y_offset, Transform parent, WallBlueprints blueprints)
    {
        //The x and y offsets determine how far into the room the walls are
        RoomEntrance temp = null;
        foreach (RoomEntrance entrance in directions.directions)
        {
            if (entrance.DirectionModifier == entranceDirection)
            {
                temp = entrance;
            }
        }
        if(temp == null){return;}
        for (int i = startValue; i < limit; i++)
        {
            if (i == 9 || i == 10) 
            {
                if (temp.Open == true)
                {
                    if (temp.Spawned == true)
                    {
                        continue;
                    }
                }
            }
            GameObject newWall = Instantiate(blueprints.wallBlock, new Vector2(transform.position.x + i * x_modifier + x_offset,transform.position.y + i * y_modifier + y_offset), Quaternion.identity, parent);
            newWall.GetComponentInChildren<SpriteRenderer>().color = GetWallColor();
            Debug.Log("i: " + i + " X_Offset: " + x_offset + " Y_Offset: " + y_offset);
            Debug.Log("X: " + (i*x_modifier+x_offset) + " Y: " + (i*y_modifier+y_offset) + " Room: " + this + " Camera: " + CameraBoundaries);
            m_wallPositions[i*x_modifier+x_offset][i*y_modifier+y_offset].PlaceDown();
        }
    }
    public virtual void PlaceDownInnerWalls(WallBlueprints blueprints, Transform parent)
    {
        //i should count up to the width of the room, j should count to the height
        //Should check to make sure that theres at least a tile to the x or the y, unless its spawning in the middle of the room
        Vector2 roomCenter = new Vector2(CameraBoundaries.x / 2, CameraBoundaries.y / 2);
        Vector2 wallThickness = new Vector2(Random.Range(CameraBoundaries.x/2 - 2, CameraBoundaries.x/2 + 1), Random.Range(CameraBoundaries.y/2 - 2, CameraBoundaries.y/2 + 1));
        for (int i = 1; i < CameraBoundaries.x - 1; i++)
        {
            if(i == 1|| i == CameraBoundaries.x - 1)
            {
                    for (int k = i; k < i; k++)
                    {
                        for (int l = 1; l < 1; l++)
                        {
                            m_wallPositions[k][l].PlaceDown();
                        }
                        for (int l = (int)CameraBoundaries.y - 1; l < CameraBoundaries.y - 1; l++)
                        {
                            m_wallPositions[k][l].PlaceDown();
                        }
                    }
        
            }
            for (int j = 1; j < CameraBoundaries.y - 1; j++)
            {
                if (j == 1|| j == CameraBoundaries.y - 1)
                {
                    for (int k = j; k < j; k++)
                    {
                        for (int l = 1; l < 1; l++)
                        {
                            m_wallPositions[k][l].PlaceDown();
                        }
                        for (int l = (int)CameraBoundaries.x - 1; l < CameraBoundaries.x - 1; l++)
                        {
                            m_wallPositions[k][l].PlaceDown();
                        }
                    }
                }
                if(roomData.m_layout == RoomLayout.NormalOutdoors)
                {
                    BuildLayout(i, j, roomCenter, wallThickness, blueprints, parent);
                }
            }
        }
    }

    public virtual void BuildLayout(int i, int j, Vector2 center, Vector2 wallThickness, WallBlueprints blueprints, Transform parent)
    {
        float distanceToCenter = new Vector2(i - center.x, j - center.y).magnitude;
        if (distanceToCenter > wallThickness.x && distanceToCenter > wallThickness.y)
        {
            int temp = Random.Range(0, 2);
            if (temp == 1)
            {
                GameObject newWall = Instantiate(blueprints.wallBlock, new Vector2(transform.position.x + i,transform.position.y + j), Quaternion.identity, parent);
                newWall.GetComponentInChildren<SpriteRenderer>().color = GetWallColor();
            }
        }
    }
    public void AdjustWalls()
    {
        for (int i = 0; i < CameraBoundaries.x; i++)
        {
            for (int j = 0; j < CameraBoundaries.y; j++)
            {
                int amountOfAdjacentWalls = 0;
                if (i + 1 < CameraBoundaries.x)
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
                if (j + 1 < CameraBoundaries.y)
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
                    m_wallPositions[i][j].PlaceDown();
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
                    if(j+1 < CameraBoundaries.y)
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
                    if (i + 1 < CameraBoundaries.x)
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
        for (int i = 0; i < CameraBoundaries.x; i++)
        {
            for (int j = 0; j < CameraBoundaries.y; j++)
            {
                if (m_wallPositions[i][j].GetIsOccupied())
                {
                    GameObject newWall = Instantiate(blueprints.wallBlock, m_wallPositions[i][j].transform.position, Quaternion.identity, transform);
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
        for (int i = 0; i < CameraBoundaries.x; i++)
        {
            for (int j = 0; j < CameraBoundaries.y; j++)
            {
                if (!m_wallPositions[i][j].GetIsOccupied())
                {
                    GameObject newWall = Instantiate(floorTile, m_wallPositions[i][j].transform.position, Quaternion.identity, transform);
                    newWall.GetComponentInChildren<SpriteRenderer>().color = GetWallColor() * 0.6f;
                }
            }
        }
    }

    protected Color GetWallColor()
    {
        Color newColor = new Color(0, 0, 0);
        switch (roomData.m_type)
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
        if (roomData.m_roomPosition == RoomPosition.DeadEnd)
        {
            newColor += new Color(0.3f, 0.3f, 0.3f);
        }
        return newColor;
    }
}
