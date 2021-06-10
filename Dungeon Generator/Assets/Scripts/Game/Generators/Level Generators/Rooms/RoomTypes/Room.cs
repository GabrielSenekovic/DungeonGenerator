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
    [System.Serializable]public struct RoomDebug 
    {
        public enum RoomBuildMode
        {
            NONE,
            CHECK_TEMPLATE,

            BOTH
        }
        public RoomBuildMode CheckTemplate;
        public Color floorColor;
        public Color wallColor;
    }
    public class RoomTemplate
    {
        //This class is given to the CreateWalls in order to draw the meshes for walls
        //It is also given to the CreateFloor in order to draw the floor
        public class TileTemplate
        {
            public int identity; //0 for void, 1 for wall, 2 for floor
            public bool read;

            public TileTemplate(int identity_in)
            {
                identity = identity_in;
                read = false;
            }
            public void SetIdentity(int newIdentity)
            {
                identity = newIdentity;
            }
            public void SetRead(bool value)
            {
                read = value;
            }
        }
        public Vector2Int size;
        public List<TileTemplate> positions;
        public RoomTemplate(Vector2Int size_in, List<TileTemplate> positions_in)
        {
            size = size_in;
            positions = positions_in;
        }
        public bool IsPositionWithinBounds(Vector2Int pos)
        {
            if(pos.x >= 0 && pos.x < size.x && pos.y <= 0 && pos.y > -size.y )
            {
                return true;
            }
            return false;
        }
        public Tuple<bool, Vector2Int> HasWallNeighbor(Vector2Int pos, int rotation)
        {
            //Debug.Log("Checking if position: " + pos + "Has any free neighbors");
            Vector2Int direction = Vector2Int.zero;
            bool value = true;
            if(rotation == 0 && IsPositionWithinBounds(new Vector2Int(pos.x + 1, pos.y)) && positions[pos.x + 1 + size.x * -pos.y].identity == 1 && !positions[pos.x + 1 + size.x * -pos.y].read)
            {
                direction = new Vector2Int(1, 0);
            }
            else if(rotation == 180 && IsPositionWithinBounds(new Vector2Int(pos.x - 1, pos.y)) && positions[pos.x - 1 + size.x * -pos.y].identity == 1 && !positions[pos.x - 1 + size.x * -pos.y].read)
            {
                direction = new Vector2Int(-1, 0);
            }
            else if(rotation == 270 && IsPositionWithinBounds(new Vector2Int(pos.x, pos.y + 1)) && positions[pos.x + size.x * (-pos.y - 1)].identity == 1 && !positions[pos.x + size.x * (-pos.y - 1)].read)
            {
                direction = new Vector2Int(0, -1);
            }
            else if(rotation == 90 && IsPositionWithinBounds(new Vector2Int(pos.x, pos.y - 1)) && positions[pos.x + size.x * (-pos.y + 1)].identity == 1 && !positions[pos.x + size.x * (-pos.y + 1)].read)
            {
                direction = new Vector2Int(0, 1);
            }
            else
            {
                //Debug.Log(pos.x + size.x * (-pos.y - 1));
                //Debug.Log("This wall has no unread neighbors");
                value = false;
            }
           // Debug.Log("It has a neighbor in this direction: " + direction);
            return new Tuple<bool, Vector2Int>(value, direction);
        }
    }
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
    public Material wallMaterial;
    public Material floorMaterial; //Will be the same material later

    public Color floorAndWallColor;

    public RoomDebug debug;

    public GameObject debugFloor;

    private void Start() 
    {
        Initialize(new Vector2(20,20));
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
        CreateWalls(CreateRoomTemplate(RoomSize));
        wallMaterial.color = floorAndWallColor;
        floorMaterial.color = floorAndWallColor;
    }

    RoomTemplate CreateRoomTemplate(Vector2 RoomSize)
    {
        RoomTemplate template = new RoomTemplate(new Vector2Int((int)RoomSize.x, (int)RoomSize.y), new List<RoomTemplate.TileTemplate>());
        //Create a perfect square room
        for(int i = 0; i < RoomSize.x; i++)
        {
            template.positions.Add(new RoomTemplate.TileTemplate(1));
        }
        for(int i = 1; i < RoomSize.y - 1; i++)
        {
            for(int j = 0; j < RoomSize.x; j++)
            {
                if(j == 0 || j == RoomSize.x - 1)
                {
                    template.positions.Add(new RoomTemplate.TileTemplate(1)); //1 is a wall
                }
                else
                {
                    template.positions.Add(new RoomTemplate.TileTemplate(2)); //2 is a floor
                }
            }
        }
        for(int i = 0; i < RoomSize.x; i++)
        {
            template.positions.Add(new RoomTemplate.TileTemplate(1));
        }
        return template;
    }
    List<MeshMaker.WallData> ReadTemplate(RoomTemplate template)
    {
        //Find a wall that has a floor next to it
        Vector2Int pos = new Vector2Int(-1,-1);
        for(int x = 0; x < template.size.x; x++)
        {
            for(int y = 0; y < template.size.y; y++)
            {
                //Check if there is floor diagonally right down, because walls can only be drawn from left to right
                //If there are none, rotate the search three times. If there still are none, then there is an error
                if(x < template.size.x && template.positions[x + 1 + template.size.x * (y + 1)].identity == 2)
                {
                   // Debug.Log("Index: " + (x + 1 + template.size.x * (y + 1)));
                    pos = new Vector2Int(x, y); break; 
                }
                    //if none of the directions are a floor, then it is a void
                   // template.positions[x + template.size.x * y].SetIdentity(0);
            }
            if(pos != new Vector2Int(-1,-1))
            {
                break;
            }
        }

        //Now we should have a piece of a wall we can start from
        //!Now, follow the walls and create new WallData everytime it turns once

        //Find direction to follow
        List<MeshMaker.WallData> data = new List<MeshMaker.WallData>();

        int currentAngle = 0;

        Tuple<bool, Vector2Int> returnData = template.HasWallNeighbor(pos, currentAngle); //Item2 is the direction to go to
        template.positions[pos.x + template.size.x * pos.y].SetRead(true);
        Vector2Int startPosition = pos;

        //Debug.Log("Start making wall from: " + pos);

        while(returnData.Item1) //If there is a wall neighbor, proceed
        {
            startPosition = pos;
            //Follow that direction until its empty
            int steps = 1;
            //Debug.Log("Checking index: " + (pos.x + returnData.Item2.x + template.size.x * (-pos.y + returnData.Item2.y)));
            while(template.IsPositionWithinBounds(new Vector2Int(pos.x + returnData.Item2.x, pos.y - returnData.Item2.y)) && template.positions[pos.x + returnData.Item2.x + template.size.x * (-pos.y + returnData.Item2.y)].identity == 1) //While the position in the next direction is a wall
            {
                //Debug.Log("Looping the while");
                steps++;
                pos = new Vector2Int(pos.x + returnData.Item2.x, pos.y - returnData.Item2.y);
                //Debug.Log("Checking index: " + (pos.x + template.size.x * -pos.y));
                template.positions[pos.x + template.size.x * -pos.y].SetRead(true);
            }
            
            // Debug.Log("Amount of steps: " + steps);
            if(currentAngle == 0)
            {
                data.Add(new MeshMaker.WallData(new Vector3(startPosition.x,startPosition.y,0), currentAngle, steps, 4, 0));
            }
            if(currentAngle == 90)
            {
                data.Add(new MeshMaker.WallData(new Vector3(startPosition.x + 0.5f, startPosition.y - 0.5f,0), -currentAngle, steps, 4, 0));
            }
            if(currentAngle == 180)
            {
                data.Add(new MeshMaker.WallData(new Vector3(startPosition.x,startPosition.y - 1,0), -currentAngle, steps, 4, 0));
            }
            if(currentAngle == 270)
            {
                data.Add(new MeshMaker.WallData(new Vector3(startPosition.x - 0.5f,startPosition.y - 0.5f,0), -currentAngle, steps, 4, 0));
            }
            currentAngle += 90; //This code can only do inner corners atm, not outer corners
            returnData = template.HasWallNeighbor(pos, currentAngle);
        }
      //  Debug.Log("There are this amount of walls: " + data.Count);

        return data;
    }
    void CreateWalls(RoomTemplate template)
    {
        GameObject wallObject = new GameObject("Wall");
        wallObject.transform.parent = this.gameObject.transform;
        wallObject.AddComponent<MeshFilter>();

        if(debug.CheckTemplate == RoomDebug.RoomBuildMode.NONE || debug.CheckTemplate == RoomDebug.RoomBuildMode.BOTH)
        {
            List<MeshMaker.WallData> wallData = ReadTemplate(template);
            MeshMaker.CreateWall(wallObject, wallObject.GetComponent<MeshFilter>().mesh, wallData, new Vector2(3,3), 0.05f); //0.05f

            wallObject.AddComponent<MeshRenderer>();
            wallObject.GetComponent<MeshRenderer>().material = wallMaterial;
        }
        if(debug.CheckTemplate == RoomDebug.RoomBuildMode.CHECK_TEMPLATE || debug.CheckTemplate == RoomDebug.RoomBuildMode.BOTH)
        {
            DEBUG_TemplateCheck(template, wallObject.transform);
        }
        wallObject.transform.localPosition = new Vector3(transform.position.x - template.size.x / 2 + 0.5f, transform.position.y + template.size.y / 2, 0);
    }
    void DEBUG_TemplateCheck(RoomTemplate template, Transform wallObject)
    {
        for(int x = 0; x < template.size.x; x++)
        {
            for(int y = 0; y < template.size.y; y++)
            {
                GameObject temp = Instantiate(debugFloor, new Vector3(x,-y,-1), Quaternion.identity, wallObject);
                temp.GetComponent<MeshRenderer>().material.color = template.positions[x + template.size.x * y].identity == 2 ? debug.floorColor : 
                template.positions[x + template.size.x * y].read ? debug.wallColor: Color.white;
            }
        }
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
