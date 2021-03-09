using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

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
    public class Fusion
    {
        public List<bool> hasFusedWalls;
        public bool IsDone()
        {
            foreach(bool value in hasFusedWalls)
            {
                if(!value)
                {
                    return false;
                }
            }
            return true;
        }
    }
    public Fusion fusionBools = new Fusion();
    public List<List<WallPosition>> wallPositions = new List<List<WallPosition>> { };
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

    public bool hasFusedWalls = false;
    public Material material;

    private void Start() 
    {
        Initialize(new Vector2(20,20));
    }

    void BuildWallArray()
    {
        for(int i = 0; i < CameraBoundaries.x; i++)
        {
            wallPositions.Add(new List<WallPosition> { });
            for(int j = 0; j < CameraBoundaries.y; j++)
            {
                WallPosition newWall = Instantiate(wallPosition, new Vector2(transform.position.x + i, transform.position.y + j), Quaternion.identity, transform);
                wallPositions[i].Add(newWall);
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
        //OpenAllEntrances();
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
        //Build wall meshes all around the start area in a 30 x 30 square
        GameObject wallObject = new GameObject("Wall");
        wallObject.transform.parent = this.gameObject.transform;
        wallObject.AddComponent<MeshFilter>();
        List<MeshMaker.WallData> wallData = new List<MeshMaker.WallData> 
        {
            new MeshMaker.WallData(new Vector3(0,0,0), 0, 20, 4, 0),
            new MeshMaker.WallData(new Vector3(19.5f,-0.5f,0), -90, 20, 4, 0),
            new MeshMaker.WallData(new Vector3(19,-20), -180, 20, 4, 0),
            new MeshMaker.WallData(new Vector3(-0.5f,-19.5f), -270, 20, 4, 0)
        };
        MeshMaker.CreateWall(wallObject, wallObject.GetComponent<MeshFilter>().mesh, wallData, new Vector2(3,3), 0.05f); //0.05f
        wallObject.AddComponent<MeshRenderer>();
        wallObject.GetComponent<MeshRenderer>().material = material;
        wallObject.transform.localPosition = new Vector3(transform.position.x - RoomSize.x / 2 + 0.5f, transform.position.y + RoomSize.y / 2, 0);
    }

    public Vector2 GetCameraBoundaries()
    {
        return CameraBoundaries;
    }

    public List<List<WallPosition>> GetWallPositions()
    {
        return wallPositions;
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
        //This gets if the room is an endroom. However, this could be set by having the rooms be endrooms when they spawn, unless they get linked
        //And then set rooms being spawned from as no longer being endrooms
        List<RoomEntrance> entrances = new List<RoomEntrance> { };
        if(!directions){return false;}
        foreach(RoomEntrance entrance in directions.directions)
        {
            if(entrance == null){continue;}
            if(entrance.Spawned == true && entrance.Open == true)
            {
                entrances.Add(entrance);
            }
        }
        return entrances.Count == 1;
    } 

    public void ChooseRoomType(LevelData data)
    {
        List<RoomType> probabilityList = new List<RoomType> { }; //A list of roomtypes to choose between
        List<RoomType> roomsToCheck = new List<RoomType>{RoomType.AmbushRoom, RoomType.TreasureRoom, RoomType.RestingRoom, RoomType.NormalRoom}; //A list of roomtypes to check the probability of

        if (GetIsEndRoom())
        {
            roomData.m_roomPosition = RoomPosition.DeadEnd;
            probabilityList.Add(RoomType.TreasureRoom);
            probabilityList.Add(RoomType.AmbushRoom);
        }
        else
        {
            for(int i = 0; i < roomsToCheck.Count; i++)
            {
                for(int j = 0; j < data.GetRoomProbability(roomsToCheck[i]); j++)
                {
                    probabilityList.Add(roomsToCheck[i]);
                }
            }
            if (roomData.stepsAwayFromMainRoom > 5)
            {
                probabilityList.Add(RoomType.MiniBossRoom);
                probabilityList.Add(RoomType.AmbushRoom);
            }
        }
        roomData.m_type = probabilityList[UnityEngine.Random.Range(0, probabilityList.Count)];
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
        if(DebuggingTools.displayFuseRoomDebugLogs)
        {
            Debug.Log("Camera boundaries are now: " + CameraBoundaries);
        }
    }
    public void EmptyCameraBoundaries()
    {
        CameraBoundaries = Vector2.zero;
    }
    public IEnumerator FuseWallPositions(List<List<WallPosition>> positionsToAdd, Vector2 position, Vector2 RoomSize, int index)
    {
        // Debug.Log("X Reach of this room: " + (transform.position.x + m_wallPositions.Count) + " X Position of other room: " + position.x);
        if (transform.position.x + wallPositions.Count == position.x)
        {
            //this room is to the left of the room with the new positions

            for (int i = 0; i < positionsToAdd.Count; i++)
            {
                wallPositions.Add(positionsToAdd[i]);
            }
        }

        if (transform.position.y < position.y)
        {
            //This room is below the room with the new positions
            //the index of positionsToAdd must begin at the right x
            if(DebuggingTools.displayFuseRoomDebugLogs)
            {
                Debug.Log(this + " Positions to add [0] size: " + positionsToAdd[0].Count);
            }
            int startIndex = (int)position.x - (int)transform.position.x;
            if(DebuggingTools.displayFuseRoomDebugLogs)
            { 
                Debug.Log(this + " Start Index is: " + startIndex);
            }
            for (int i = startIndex; i < startIndex + RoomSize.y; i++)
            {
                if(DebuggingTools.displayFuseRoomDebugLogs)
                {
                    Debug.Log(this + " i: " + i);
                }
                //i represents the x value of this rooms walls
                for (int j = 0; j < positionsToAdd[0].Count; j++)
                {
                    //j represents the y value of this rooms walls, which we are adding to
                    //the x index of the wall array were copying from still has to start at 0
                    if(DebuggingTools.displayFuseRoomDebugLogs)
                    {
                        Debug.Log(this + " j: " + i);
                    }
                    //i-startIndex just ensures that x always starts at 0
                    wallPositions[i].Add(positionsToAdd[i - startIndex][j]);
                    yield return new WaitForSeconds(0.01f);
                    
                }
            }
        }
        if(DebuggingTools.displayFuseRoomDebugLogs)
        { 
            for(int i = 0; i < wallPositions.Count; i++)
            {
                Debug.Log(this + " has this many in " + i + ": " + wallPositions[i].Count);
            }
            Debug.Log("<color=magenta> Setting </color>" + this + " to having fused walls");
        }
        fusionBools.hasFusedWalls[index] = true;
    }
    public void FuseEntrances(RoomDirections newDirections, Vector2 destination, Vector2 RoomSize)
    {
        if (destination.y > transform.position.y && destination.x == transform.position.x)
        {
            //If the new room is to the north
            OnFuseEntrances(new Vector4(0,1,2,3), newDirections, (int)RoomSize.y);
        }
        if (destination.x > transform.position.x && destination.y == transform.position.y)
        {
            //If the new room is to the right
            OnFuseEntrances(new Vector4(1,0,3,2), newDirections, (int)RoomSize.x);
        }
        if (destination.y > transform.position.y && destination.x > transform.position.x)
        {
            //if the new room is northeast
            //delete left and south entrances
            if(newDirections.directions[2])
            {
                Destroy(newDirections.directions[2]);
            }
            if(newDirections.directions[3])
            {
                Destroy(newDirections.directions[3]);
            }
            for(int i = 0; i < directions.directions.Count; i++)
            {
                if(directions.directions[i]!=null)
                {
                    if(directions.directions[i].Index == new Vector2(9 + (transform.position.x - destination.x), 10 + (transform.position.x - destination.x)))
                    {
                        //entrance found that must be replaced
                        if(directions.directions[i].DirectionModifier == new Vector2(1,0))
                        {
                            newDirections.directions[i].Index = directions.directions[i].Index;
                            Destroy(directions.directions[i].gameObject);
                            directions.directions[i] = newDirections.directions[1];
                            //is it a right entrance
                        }
                    }
                    if(directions.directions[i].Index == new Vector2(9 + (transform.position.y - destination.y), 10 + (transform.position.y - destination.y)))
                    {
                        //entrance found that must be replaced
                        if(directions.directions[i].DirectionModifier == new Vector2(0,1))
                        {
                            newDirections.directions[i].Index = directions.directions[i].Index;
                            Destroy(directions.directions[i].gameObject);
                            directions.directions[i] = newDirections.directions[0];
                            //is it a north entrance
                        }
                    }
                }
            }
        }
    }
    void OnFuseEntrances(Vector4 indexes, RoomDirections newDirections, int offset)
    {
        if (directions.directions[(int)indexes.x])
        {
            Destroy(directions.directions[(int)indexes.x].gameObject);
            directions.directions[(int)indexes.x] = newDirections.directions[1];
        }
        if (newDirections.directions[(int)indexes.y])
        {
            newDirections.directions[(int)indexes.y].Index = new Vector2(newDirections.directions[(int)indexes.y].Index.x + offset, newDirections.directions[(int)indexes.y].Index.y + offset);
            directions.directions.Add(newDirections.directions[(int)indexes.y]);
            newDirections.directions[(int)indexes.y].transform.parent = transform;
        }
        if (newDirections.directions[(int)indexes.z])
        {
            newDirections.directions[(int)indexes.z].Index = new Vector2(newDirections.directions[(int)indexes.z].Index.x + offset, newDirections.directions[(int)indexes.z].Index.y + offset);
            directions.directions.Add(newDirections.directions[(int)indexes.z]);
            newDirections.directions[(int)indexes.z].transform.parent = transform;
        }
        if (newDirections.directions[(int)indexes.w])
        {
            Destroy(newDirections.directions[(int)indexes.w].gameObject);
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
        OnPlaceDownWall(new Vector2(0, -1), (int)CameraBoundaries.x, 0, 1, 0, 0, 0);
        OnPlaceDownWall(new Vector2(-1, 0), (int)CameraBoundaries.y, 1, 0, 1, 0, 0);
        OnPlaceDownWall(new Vector2(1, 0), (int)CameraBoundaries.y, 1, 0, 1, (int)CameraBoundaries.x - 1 ,0);
        OnPlaceDownWall(new Vector2(0, 1), (int)CameraBoundaries.x - 1, 1, 1, 0, 0 ,(int)CameraBoundaries.y - 1);

        PlaceDownInnerWalls(blueprints, wallParent.transform);
        for (int i = 0; i < 3; i++)
        {
            UnscatterWalls(); //Make sure there are no free standing wall pieces nor empty holes
        } 

        WeedOutUnseeableWalls();
        if(!DebuggingTools.spawnOnlyBasicRooms){DetermineWallVariant();}
        InstantiateWalls(blueprints, wallParent.transform);
    }
    public void OnPlaceDownWall(Vector2 entranceDirection, int limit, int startValue, 
    int x_modifier, int y_modifier, int x_offset, int y_offset)
    {
        //The x and y offsets determine how far into the room the walls are
        List<RoomEntrance> temp = new List<RoomEntrance>(){};
        foreach (RoomEntrance entrance in directions.directions)
        {
            if (entrance.DirectionModifier == entranceDirection)
            {
                temp.Add(entrance);
            }
        }
        for (int i = startValue; i < limit; i++)
        {
            for(int j = 0; j < temp.Count; j++)
            {
                if (i == temp[j].Index.x || i == temp[j].Index.y) 
                {
                    if (temp[j].Open == true)
                    {
                        if (temp[j].Spawned == true)
                        {
                            goto End;
                        }
                    }
                }
            }
            if(DebuggingTools.displayRoomConstructionDebugLogs)
            {
                Debug.Log("i: " + i + " X_Offset: " + x_offset + " Y_Offset: " + y_offset);
                Debug.Log("X: " + (i*x_modifier+x_offset) + " Y: " + (i*y_modifier+y_offset) + " Room: " + this + " Camera: " + CameraBoundaries);
            }
            wallPositions[i*x_modifier+x_offset][i*y_modifier+y_offset].PlaceDown();
            End:;
        }
    }
    public virtual void PlaceDownInnerWalls(WallBlueprints blueprints, Transform parent)
    {
        //i should count up to the width of the room, j should count to the height
        //Should check to make sure that theres at least a tile to the x or the y, unless its spawning in the middle of the room
        Vector2 roomCenter = new Vector2(CameraBoundaries.x / 2, CameraBoundaries.y / 2);
        Vector2 wallThickness = new Vector2(UnityEngine.Random.Range(CameraBoundaries.x/2 - 2, CameraBoundaries.x/2 + 1), UnityEngine.Random.Range(CameraBoundaries.y/2 - 2, CameraBoundaries.y/2 + 1));
        for (int i = 1; i < CameraBoundaries.x - 1; i++)
        {
            if(i == 1|| i == CameraBoundaries.x - 1)
            {
                    for (int k = i; k < i; k++)
                    {
                        for (int l = 1; l < 1; l++)
                        {
                            wallPositions[k][l].PlaceDown();
                        }
                        for (int l = (int)CameraBoundaries.y - 1; l < CameraBoundaries.y - 1; l++)
                        {
                            wallPositions[k][l].PlaceDown();
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
                            wallPositions[k][l].PlaceDown();
                        }
                        for (int l = (int)CameraBoundaries.x - 1; l < CameraBoundaries.x - 1; l++)
                        {
                            wallPositions[k][l].PlaceDown();
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
        float distanceToCenter = new Vector2(i+0.5f - center.x, j+0.5f - center.y).magnitude;
        if (distanceToCenter > wallThickness.x && distanceToCenter > wallThickness.y)
        {
            //if the higher limit is 0, then the code just generates a circle, period
            int temp = UnityEngine.Random.Range(0, 2);
            if (temp == 0)
            {
                wallPositions[i][j].PlaceDown();
            }
        }
    }
    public void UnscatterWalls()
    {
        for (int i = 0; i < CameraBoundaries.x; i++)
        {
            for (int j = 0; j < CameraBoundaries.y; j++)
            {
                int amountOfAdjacentWalls = 0;
                if (i + 1 < CameraBoundaries.x)
                {
                    if (wallPositions[i + 1][j].GetIsOccupied())
                    {
                        amountOfAdjacentWalls++;
                    }
                }
                if (i - 1 >= 0)
                {
                    if (wallPositions[i - 1][j].GetIsOccupied())
                    {
                        amountOfAdjacentWalls++;
                    }
                }
                if (j + 1 < CameraBoundaries.y)
                {
                    if (wallPositions[i][j + 1].GetIsOccupied())
                    {
                        amountOfAdjacentWalls++;
                    }
                }
                if (j - 1 >= 0)
                {
                    if (wallPositions[i][j - 1].GetIsOccupied())
                    {
                        amountOfAdjacentWalls++;
                    }
                }
                if (amountOfAdjacentWalls == 0){wallPositions[i][j].UnPlace();}
                if (amountOfAdjacentWalls >= 3){wallPositions[i][j].PlaceDown();}
            }
        }
    }
    public void WeedOutUnseeableWalls()
    {
        List<WallPosition> positionsToDestroy = new List<WallPosition>(){};
        //If this wall has walls all around it, it cannot be seen
        for(int i = 0; i < wallPositions.Count; i++)
        {
            for(int j = 0; j < wallPositions[i].Count; j++)
            {
                if(GetArrayDiagonalPositioningString(i, j) == "AAAA")
                {
                    positionsToDestroy.Add(wallPositions[i][j]);
                }
            }
        }
        foreach(WallPosition position in positionsToDestroy)
        {
            position.UnPlace();
        }
    }

    public void DetermineWallVariant()
    {
        for(int i = 0; i < wallPositions.Count; i++)
        {
            for(int j = 0; j < wallPositions[i].Count; j++)
            {
                if (wallPositions[i][j].GetIsOccupied())
                {
                    SetWallVariantsFromString(GetArrayPositioningString(i, j), wallPositions[i][j]);
                }
            }
        }

    }
    string GetArrayDiagonalPositioningString(int i, int j)
    {
        string temp = "";
        if(j+1 < CameraBoundaries.y && i+1 < CameraBoundaries.x)
        {
            if (wallPositions[i + 1][j + 1].GetIsOccupied()){temp+='A';}
            else{temp+='B';}
        }
        else
        {
            temp+='B';
        }
        if (i + 1 < CameraBoundaries.x && j - 1 >= 0)
        {
            if (wallPositions[i+1][j -1].GetIsOccupied()){temp+='A';}
            else{temp+='B';}
        }
        else
        {
            temp+='B';
        }
        if (j - 1 >= 0 && i - 1 >= 0)
        {
            if (wallPositions[i - 1][j - 1].GetIsOccupied()){temp+='A';}
            else{temp+='B';}
        }
        else
        {
            temp+='B';
        }
        if (i - 1 >= 0 && j+1 < CameraBoundaries.y)
        {
            if (wallPositions[i-1][j+1].GetIsOccupied()){temp+='A';}
            else{temp+='B';}
        }
        else
        {
            temp+='B';
        }
        return temp;
    }
    string GetArrayPositioningString(int i, int j)
    {
        string temp = "";
        if(j+1 < CameraBoundaries.y)
        {
            if (wallPositions[i][j + 1].GetIsOccupied())
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
            if (wallPositions[i+1][j].GetIsOccupied())
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
            if (wallPositions[i][j - 1].GetIsOccupied())
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
            if (wallPositions[i-1][j].GetIsOccupied())
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
        return temp;
    }
    void SetWallVariantsFromString(string text, WallPosition wall)
    {
        switch (text)
        {
            case "AAAA":
                wall.SetVariant(WallVariant.Cross);
                wall.transform.eulerAngles = new Vector3(-90, -90, 90);
                break; //inner wall
            case "AAAB":
                wall.SetVariant(WallVariant.T);
                wall.transform.eulerAngles = new Vector3(0, -90, 90);
                wall.transform.position = new Vector2( wall.transform.position.x - 1,  wall.transform.position.y);
                break;
            case "AABA":
                wall.SetVariant(WallVariant.T);
                wall.transform.eulerAngles = new Vector3(90, 90, -90);
                wall.transform.position = new Vector2( wall.transform.position.x - 1,  wall.transform.position.y - 1);
                break;
            case "AABB":
                wall.SetVariant(WallVariant.Corner);
                wall.transform.eulerAngles = new Vector3(-90, -90, 90);
                break; 
            case "ABAA":
                wall.SetVariant(WallVariant.T);
                wall.transform.eulerAngles = new Vector3(-180, -90, 90);
                wall.transform.position = new Vector2( wall.transform.position.x,  wall.transform.position.y - 1);
                break;
            case "ABAB":
                wall.SetVariant(WallVariant.Side);
                wall.transform.eulerAngles = new Vector3(-180, -90, 90);
                wall.transform.position = new Vector2( wall.transform.position.x,  wall.transform.position.y - 1);
                break;
            case "ABBA":
                wall.SetVariant(WallVariant.Corner);
                wall.transform.eulerAngles = new Vector3(0, -90, 90);
                wall.transform.position = new Vector2( wall.transform.position.x - 1,  wall.transform.position.y);
                break; 
            case "ABBB": 
                wall.SetVariant(WallVariant.End);
                wall.transform.eulerAngles = new Vector3(0, -90, 90);
                wall.transform.position = new Vector2( wall.transform.position.x - 1,  wall.transform.position.y);
                break;
            case "BAAA":
                wall.SetVariant(WallVariant.T);
                wall.transform.eulerAngles = new Vector3(-90, -90, 90);
                break;
            case "BAAB":
                wall.SetVariant(WallVariant.Corner);
                wall.transform.eulerAngles = new Vector3(-180, -90, 90);
                wall.transform.position = new Vector2( wall.transform.position.x,  wall.transform.position.y - 1);
                break;
            case "BABA":
                wall.SetVariant(WallVariant.Side);
                wall.transform.eulerAngles = new Vector3(-90, -90, 90);
                break;
            case "BABB": 
                wall.SetVariant(WallVariant.End);
                wall.transform.eulerAngles = new Vector3(-90, -90, 90);
                break;
            case "BBAA":
                wall.SetVariant(WallVariant.Corner);
                wall.transform.eulerAngles = new Vector3(90, 90, -90);
                wall.transform.position = new Vector2( wall.transform.position.x - 1,  wall.transform.position.y - 1);
                break;
            case "BBAB": 
                wall.SetVariant(WallVariant.End);
                wall.transform.eulerAngles = new Vector3(-180, -90, 90);
                wall.transform.position = new Vector2( wall.transform.position.x,  wall.transform.position.y - 1);
                break;
            case "BBBA": 
                wall.SetVariant(WallVariant.End);
                wall.transform.eulerAngles = new Vector3(90, 90, -90);
                wall.transform.position = new Vector2( wall.transform.position.x - 1,  wall.transform.position.y - 1);
                break;
            case "BBBB": 
                wall.SetVariant(WallVariant.Column);
                wall.transform.eulerAngles = new Vector3(-90, -90, 90);
                break;
        }
    }
    public void InstantiateWalls(WallBlueprints blueprints, Transform wallParent) //wil be deprecated
    {
        for (int i = 0; i < CameraBoundaries.x; i++)
        {
            for (int j = 0; j < CameraBoundaries.y; j++)
            {
                if (wallPositions[i][j].GetIsOccupied())
                {
                    if (wallPositions[i][j].GetVariant() == WallVariant.None)
                    {
                        GameObject newWall = Instantiate(blueprints.wallBlock, wallPositions[i][j].transform.position, Quaternion.identity, wallParent);
                        newWall.GetComponentInChildren<SpriteRenderer>().color = GetWallColor();
                    }
                    else
                    {
                        try
                        {
                            GameObject newWall = Instantiate(blueprints.walls[(int)wallPositions[i][j].GetVariant() - 1], new Vector3(wallPositions[i][j].transform.position.x + 0.5f, wallPositions[i][j].transform.position.y + 0.5f, wallPositions[i][j].transform.position.z + 1.25f), Quaternion.identity, wallParent);
                            newWall.transform.rotation = wallPositions[i][j].transform.rotation;
                        }
                        catch
                        {
                            GameObject newWall = Instantiate(blueprints.wallBlock, wallPositions[i][j].transform.position, Quaternion.identity, wallParent);
                            newWall.GetComponentInChildren<SpriteRenderer>().color = GetWallColor();
                        }
                    }
                }
            }
        }
    }
    public void InstantiateFloor(GameObject floorTile)
    {
        GameObject floorParent = new GameObject("Floor");
        floorParent.transform.SetParent(transform);
        for (int i = 0; i < CameraBoundaries.x; i++)
        {
            for (int j = 0; j < CameraBoundaries.y; j++)
            {
                if (!wallPositions[i][j].GetIsOccupied())
                {
                    GameObject newWall = Instantiate(floorTile, wallPositions[i][j].transform.position, Quaternion.identity, floorParent.transform);
                    
                    if(DebuggingTools.spawnOnlyBasicRooms)
                    {
                        newWall.GetComponentInChildren<SpriteRenderer>().color = GetWallColor() * 0.6f;
                    }
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
