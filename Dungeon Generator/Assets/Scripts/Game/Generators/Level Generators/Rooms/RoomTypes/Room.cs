﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;


[System.Serializable]public class RoomDirections
{
    public List<RoomEntrance> directions;

    public RoomDirections(Transform transform, RoomEntrance entrance)
    {
        directions = new List<RoomEntrance>();
        for(int i = 0; i < 4; i++)
        {
            directions.Add(UnityEngine.Object.Instantiate(entrance, transform));
        }
        directions[0].SetDirectionModifier(new Vector2(0, 1)); directions[0].name = "North Entrance";
        directions[1].SetDirectionModifier(new Vector2(1, 0)); directions[1].name = "Right Entrance";
        directions[2].SetDirectionModifier(new Vector2(-1, 0)); directions[2].name = "Left Entrance";
        directions[3].SetDirectionModifier(new Vector2(0, -1)); directions[3].name = "South Entrance";
    }
    public void OpenAllEntrances()
    {
        for(int i = 0; i < directions.Count; i++)
        {
            directions[i].Open = true;
        }
    }
}
[System.Serializable]public class RoomData
{
    public RoomType m_type = RoomType.NormalRoom;
    public RoomPosition m_roomPosition = RoomPosition.None;
    public bool m_Indoors = false;
    public int stepsAwayFromMainRoom = 0;
    public bool IsBuilt = false;
}
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

            public Vector2Int divisions; //This also only does something if the identity is a wall
            //If divide into multiple parts, like, three by three quads on one wall tile on outdoor walls for instance. Usually, on indoor walls, its completely flat

            public TileTemplate(int identity_in, Vector2Int divisions_in)
            {
                identity = identity_in;
                read = false;
                divisions = divisions_in;
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
        public bool indoors;
        public RoomTemplate(Vector2Int size_in, List<TileTemplate> positions_in, bool indoors_in)
        {
            size = size_in;
            positions = positions_in;
            indoors = indoors_in;
            CreateRoomTemplate();
        }
        void CreateRoomTemplate()
        {
            //In here it will be determined if the room is a circle, if it is a corridor, etc
            Vector2 roomCenter = new Vector2(size.x / 2, size.y / 2);
            Vector2 wallThickness = new Vector2(UnityEngine.Random.Range(size.x/2 - 4, size.x/2), UnityEngine.Random.Range(size.y/2 - 4, size.y/2));
            Debug.Log("Thickness of wall: " + wallThickness);

            for(int y = 0; y < size.y; y++)
            {
                for(int x = 0; x < size.x; x++)
                {
                    Vector2Int divisions = new Vector2Int(1,1);
                    if(!indoors){divisions = new Vector2Int(3,3);}
                    positions.Add(new RoomTemplate.TileTemplate(2, divisions));

                    CreateRoomTemplate_Square(new Vector2(2,2), x, y); //?Basic thickness. Can't be thinner than 2
                    //CreateRoomTemplate_Circle(roomCenter, wallThickness, x, y);
                    CreateRoomTemplate_Cross(wallThickness, x, y);
                }
            }
            UnscatterWalls();
            WeedOutUnseeableWalls(); //Later only when indoors
        }
        void CreateRoomTemplate_Circle(Vector2 center, Vector2 wallThickness, int x, int y)
        {
            float distanceToCenter = new Vector2(x+0.5f - center.x, y+0.5f - center.y).magnitude;
            if (distanceToCenter > wallThickness.x && distanceToCenter > wallThickness.y)
            {
                //if the higher limit is 0, then the code just generates a circle, period
                int temp = UnityEngine.Random.Range(0, 2);
                if (temp == 0)
                {
                    positions[x + (int)size.x * y].identity = 1;
                }
            }
        }
        void CreateRoomTemplate_Square(Vector2 wallThickness, int x, int y)
        {
            if(x < wallThickness.x || x > size.x - wallThickness.x - 1||
               y < wallThickness.y || y > size.y - wallThickness.y - 1)
            {
                positions[x + (int)size.x * y].identity = 1;
            }
        }
        void CreateRoomTemplate_Cross(Vector2 wallThickness, int x, int y)
        {
            if((x < wallThickness.x || x > size.x - wallThickness.x -1)&&
               (y < wallThickness.y || y > size.y - wallThickness.y -1))
            {
                positions[x + (int)size.x * y].identity = 1;
            }
        }

        void UnscatterWalls()
        {
            //First, removes stray positions
            for(int x = 0; x < size.x; x++)
            {
                for(int y = 0; y < size.y; y++)
                {
                    bool deleteWall = true;
                    for(int i = -1; i < 2; i++)
                    {
                        for(int j = -1; j < 2; j++)
                        {
                            if((i == 0 && j == 0) || i != 0 && j != 0){continue;} //Only check directly up or directly down
                            if(IsPositionWithinBounds(new Vector2Int(x + i, -y + j)))
                            {
                                if(!(positions[x + i + (int)size.x * (y + -j)].identity == 2)) //If the position is not a floor
                                {
                                    deleteWall = false; //Then dont delete
                                }
                            }
                        }
                    }
                    if(deleteWall)
                    {
                        positions[x + (int)size.x * y].identity = 2;
                    }
                }
            }
            //Second, fill in holes
            for(int x = 0; x < size.x; x++)
            {
                for(int y = 0; y < size.y; y++)
                {
                    int amountOfWallNeighbors = 0;
                    for(int i = -1; i < 2; i++)
                    {
                        for(int j = -1; j < 2; j++)
                        {
                            if(i == 0 && j == 0){continue;}
                            if(IsPositionWithinBounds(new Vector2Int(x + i, -y + j)))
                            {
                                if(positions[x + i + (int)size.x * (y + -j)].identity == 1) //If the position is a wall
                                {
                                    amountOfWallNeighbors++; //Then count up
                                }
                            }
                        }
                    }
                    if(amountOfWallNeighbors > 5)
                    {
                        positions[x + (int)size.x * y].identity = 1;
                    }
                }
            }
            //Shave off excess wall
            for(int x = 0; x < size.x; x++)
            {
                for(int y = 0; y < size.y; y++)
                {
                    int amountOfFloorNeighbors = 0;
                    for(int i = -1; i < 2; i++)
                    {
                        for(int j = -1; j < 2; j++)
                        {
                            if(i == 0 && j == 0){continue;}
                            if(IsPositionWithinBounds(new Vector2Int(x + i, -y + j)))
                            {
                                if(positions[x + i + (int)size.x * (y + -j)].identity == 2) //If the position is a floor
                                {
                                    amountOfFloorNeighbors++; //Then count up
                                }
                            }
                        }
                    }
                    if(amountOfFloorNeighbors > 5)
                    {
                        positions[x + (int)size.x * y].identity = 2;
                    }
                }
            }
        }
        void WeedOutUnseeableWalls()
        {
            //Cleans out walls so they become void
            for(int x = 0; x < size.x; x++)
            {
                for(int y = 0; y < size.y; y++)
                {
                    bool isVoid = true;
                    for(int i = -1; i < 2; i++)
                    {
                        for(int j = -1; j < 2; j++)
                        {
                            if(i == 0 && j == 0){continue;}
                            if(IsPositionWithinBounds(new Vector2Int(x + i, -y + j)))
                            {
                                if(!(positions[x + i + (int)size.x * (y + -j)].identity == 1 || positions[x + i + (int)size.x * (y + -j)].identity == 0))
                                {
                                    isVoid = false;
                                }
                            }
                        }
                    }
                    if(isVoid)
                    {
                        positions[x + (int)size.x * y].identity = 0;
                    }
                }
            }
        }
        /*void CreateRoomTemplate()
        {
            //Create a cross shaped room
            for(int i = 0; i < size.x; i++)
            {
                positions.Add(new RoomTemplate.TileTemplate(1));
            }
            for(int i = 1; i < size.y - 1; i++)
            {
                if(i > 4 && i < size.y - 4)
                {
                    for(int j = 0; j < size.x; j++)
                    {
                        if(j == 0 || j == size.x - 1)
                        {
                            positions.Add(new RoomTemplate.TileTemplate(1)); //1 is a wall
                        }
                        else
                        {
                            positions.Add(new RoomTemplate.TileTemplate(2)); //2 is a floor
                        }
                    }
                }
                else
                {
                    for(int j = 0; j < size.x; j++)
                    {
                        if(j < 4 || j > size.x - 4)
                        {
                            positions.Add(new RoomTemplate.TileTemplate(1)); //1 is a wall
                        }
                        else
                        {
                            positions.Add(new RoomTemplate.TileTemplate(2)); //2 is a floor
                        }
                    }
                }
            }
            for(int i = 0; i < size.x; i++)
            {
                positions.Add(new RoomTemplate.TileTemplate(1));
            }
            //Cleans out walls so they become void
            for(int x = 0; x < size.x; x++)
            {
                for(int y = 0; y < size.y; y++)
                {
                    bool isVoid = true;
                    for(int i = -1; i < 2; i++)
                    {
                        for(int j = -1; j < 2; j++)
                        {
                            if(i == 0 && j == 0){continue;}
                            if(IsPositionWithinBounds(new Vector2Int(x + i, -y + j)))
                            {
                                if(!(positions[x + i + (int)size.x * (y + -j)].identity == 1 || positions[x + i + (int)size.x * (y + -j)].identity == 0))
                                {
                                    isVoid = false;
                                }
                            }
                        }
                    }
                    if(isVoid)
                    {
                        positions[x + (int)size.x * y].identity = 0;
                    }
                }
            }
            
            //Create a perfect square room
            /*for(int i = 0; i < RoomSize.x; i++)
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
        }*/
        public bool IsPositionWithinBounds(Vector2Int pos)
        {
            if(pos.x >= 0 && pos.x < size.x && pos.y <= 0 && pos.y > -size.y )
            {
                return true;
            }
           // Debug.Log(pos + " <color=red>is not within bounds</color>");
            return false;
        }
        public Tuple<bool, Vector2Int, int> HasWallNeighbor(Vector2Int pos, int rotation)
        {
            //Debug.Log("Checking if position: " + pos + "Has any free neighbors");
            Vector2Int direction = Vector2Int.zero;
            bool value = true;
            int rotationDir = 0;
            if(rotation == 270)
            {
                if(IsPositionWithinBounds(new Vector2Int(pos.x + 1, pos.y)) && positions[pos.x + 1 + size.x * -pos.y].identity == 1 && !positions[pos.x + 1 + size.x * -pos.y].read)
                {
                    direction = new Vector2Int(1, 0);
                    rotationDir = 1;
                }
                else if(IsPositionWithinBounds(new Vector2Int(pos.x - 1, pos.y)) && positions[pos.x - 1 + size.x * -pos.y].identity == 1 && !positions[pos.x - 1 + size.x * -pos.y].read)
                {
                    direction = new Vector2Int(-1, 0);
                    rotationDir = -1;
                }
                else
                {
                    value = false;
                }
            }
            else if(rotation == 90)
            {
                if(IsPositionWithinBounds(new Vector2Int(pos.x + 1, pos.y)) && positions[pos.x + 1 + size.x * -pos.y].identity == 1 && !positions[pos.x + 1 + size.x * -pos.y].read)
                {
                    direction = new Vector2Int(1, 0);
                    rotationDir = -1;
                }
                else if(IsPositionWithinBounds(new Vector2Int(pos.x - 1, pos.y)) && positions[pos.x - 1 + size.x * -pos.y].identity == 1 && !positions[pos.x - 1 + size.x * -pos.y].read)
                {
                    direction = new Vector2Int(-1, 0);
                    rotationDir = 1;
                }
                else
                {
                    value = false;
                }
            }
            else if(rotation == 180)
            {
                if(IsPositionWithinBounds(new Vector2Int(pos.x, pos.y + 1)) && positions[pos.x + size.x * (-pos.y - 1)].identity == 1 && !positions[pos.x + size.x * (-pos.y - 1)].read)
                {
                    direction = new Vector2Int(0, -1);
                    rotationDir = 1;
                }
                else if(IsPositionWithinBounds(new Vector2Int(pos.x, pos.y - 1)) && positions[pos.x + size.x * (-pos.y + 1)].identity == 1 && !positions[pos.x + size.x * (-pos.y + 1)].read)
                {
                    direction = new Vector2Int(0, 1);
                    rotationDir = -1;
                }
                else
                {
                    value = false;
                }
            }
            else if(rotation == 0)
            {
                if(IsPositionWithinBounds(new Vector2Int(pos.x, pos.y + 1)) && positions[pos.x + size.x * (-pos.y - 1)].identity == 1 && !positions[pos.x + size.x * (-pos.y - 1)].read)
                {
                    direction = new Vector2Int(0, -1);
                    rotationDir = -1;
                }
                else if(IsPositionWithinBounds(new Vector2Int(pos.x, pos.y - 1)) && positions[pos.x + size.x * (-pos.y + 1)].identity == 1 && !positions[pos.x + size.x * (-pos.y + 1)].read)
                {
                    direction = new Vector2Int(0, 1);
                    rotationDir = 1;
                }
                else
                {
                    value = false;
                }
            }
            Debug.Log("It has a neighbor in this direction: " + direction);
            return new Tuple<bool, Vector2Int, int>(value, direction, rotationDir);
        }
        
        public List<MeshMaker.WallData> ExtractWalls()
        {
            //Find a wall that has a floor next to it
            Debug.Log("Extracting walls");
            Vector2Int pos = new Vector2Int(-1,-1);
            int currentAngle = 0; //Current angle should only be 0 if the floor found points down.

            for(int x = 0; x < size.x; x++)
            {
                for(int y = 0; y < size.y; y++)
                {
                    //Check if there is floor diagonally right down, because walls can only be drawn from left to right
                    //If there are none, rotate the search three times. If there still are none, then there is an error
                    if(IsPositionWithinBounds(new Vector2Int(x + 1, -y - 1)) && x < size.x && positions[x + 1 + size.x * (y + 1)].identity == 2)
                    {
                        pos = new Vector2Int(x, -y); 
                        currentAngle = 90;
                        break; 
                    }
                        //if none of the directions are a floor, then it is a void
                    // template.positions[x + template.size.x * y].SetIdentity(0);
                }
                if(pos != new Vector2Int(-1,-1))
                {
                    break;
                }
            }
            Debug.Log("The position found to start from, that has a floor next to it: " + pos);

            //Now we should have a piece of a wall we can start from
            //!Now, follow the walls and create new WallData everytime it turns once

            //Find direction to follow
            List<MeshMaker.WallData> data = new List<MeshMaker.WallData>();

            Tuple<bool, Vector2Int, int> returnData = HasWallNeighbor(pos, currentAngle); //Item2 is the direction to go to
            currentAngle += 90 * returnData.Item3; //This code can only do inner corners atm, not outer corners
            currentAngle = (int)Math.Mod(currentAngle, 360);
            positions[pos.x + size.x * -pos.y].SetRead(true);
            Vector2Int startPosition = pos;

            while(returnData.Item1) //If there is a wall neighbor, proceed
            {
                startPosition = pos;
                //Follow that direction until its empty
                int steps = 1;
                Debug.Log("Checking index: " + (pos.x + returnData.Item2.x + size.x * (-pos.y + returnData.Item2.y)));
                while(IsPositionWithinBounds(new Vector2Int(pos.x + returnData.Item2.x, pos.y - returnData.Item2.y)) && positions[pos.x + returnData.Item2.x + size.x * (-pos.y + returnData.Item2.y)].identity == 1) //While the position in the next direction is a wall
                {
                    steps++;
                    pos = new Vector2Int(pos.x + returnData.Item2.x, pos.y - returnData.Item2.y);
                    Debug.Log("Checking index: " + (pos.x + size.x * -pos.y));
                    positions[pos.x + size.x * -pos.y].SetRead(true);
                }
                
                int isThisWallFollowingOuterCorner = 0;
                if(returnData.Item3 < 0 && data.Count > 0)
                {
                    //If lastWall is less than 0, then this is the following wall after an outer corner, so it must be moved up and shortened
                    isThisWallFollowingOuterCorner = 1;
                }
                returnData = HasWallNeighbor(pos, currentAngle);
                int isThisWallEndingWithOuterCorner = 0;
                if(returnData.Item3 < 0)
                {
                    //If Item3 is less than 0, then this is an outer corner, so the wall shouldn't go the whole way
                    isThisWallEndingWithOuterCorner = 1;
                }
                
                Debug.Log("<color=green>Amount of steps: </color>" + steps + " angle: " + currentAngle);
                if(currentAngle == 0)
                {
                    data.Add(new MeshMaker.WallData(new Vector3(startPosition.x + isThisWallFollowingOuterCorner,startPosition.y,0), currentAngle, steps - isThisWallEndingWithOuterCorner - isThisWallFollowingOuterCorner, 4, 0, positions[pos.x + size.x * -pos.y].divisions));
                }
                if(currentAngle == 90)
                {
                    data.Add(new MeshMaker.WallData(new Vector3(startPosition.x + 0.5f, startPosition.y - 0.5f - isThisWallFollowingOuterCorner,0), -currentAngle, steps - isThisWallEndingWithOuterCorner - isThisWallFollowingOuterCorner, 4, 0, positions[pos.x + size.x * -pos.y].divisions));
                }
                if(currentAngle == 180)
                {
                    data.Add(new MeshMaker.WallData(new Vector3(startPosition.x - isThisWallFollowingOuterCorner,startPosition.y - 1,0), -currentAngle, steps - isThisWallEndingWithOuterCorner - isThisWallFollowingOuterCorner, 4, 0, positions[pos.x + size.x * -pos.y].divisions));
                }
                if(currentAngle == 270)
                {
                    data.Add(new MeshMaker.WallData(new Vector3(startPosition.x - 0.5f,startPosition.y - 0.5f + isThisWallFollowingOuterCorner ,0), -currentAngle, steps - isThisWallEndingWithOuterCorner - isThisWallFollowingOuterCorner, 4, 0, positions[pos.x + size.x * -pos.y].divisions)); // y - 0.5f
                }
                //Sometimes it has to decrease by 90, so it has to know what direction the next wall goes in (fuck)
                currentAngle += 90 * returnData.Item3; //This code can only do inner corners atm, not outer corners
                currentAngle = (int)Math.Mod(currentAngle, 360);
            }
            Debug.Log("There is this amount of walls: " + data.Count);

            return data;
        }
        public List<Vector3Int> ExtractFloor()
        {
            List<Vector3Int> returnData = new List<Vector3Int>();
            for(int x = 0; x < size.x; x++)
            {
                for(int y = 0; y < size.y; y++)
                {
                    if(positions[x + size.x * y].identity == 2 || positions[x + size.x * y].identity == 1 ) //If this position is a floor or a wall
                    {
                        returnData.Add(new Vector3Int(x,-y -1, 0));
                    }
                    else //If it is a void
                    {
                        returnData.Add(new Vector3Int(x,-y -1, 4));
                    }
                }
            }
            return returnData;
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

    public RoomDirections directions;

    public Vector2 CameraBoundaries;
    public Vector2 wallPositionBoundaries = Vector2.zero;

    public RoomData roomData = new RoomData();

    public bool hasFusedWalls = false;

    public Color floorAndWallColor;

    public RoomDebug debug;

    public GameObject debugFloor;

    public RoomEntrance roomEntrance;

    public void OpenAllEntrances()
    {
        if(directions == null)
        {
            directions = new RoomDirections(transform, roomEntrance);
        }
        directions.OpenAllEntrances();
    }
    public void Initialize(Vector2 RoomSize, Material wallMaterial, Material floorMaterial, bool indoors)
    {
        Debug.Log("<color=green>Initializing the Origin Room</color>");
        //This Initialize() function is for the origin room specifically, as it already has its own position
        OnInitialize(RoomSize, wallMaterial, floorMaterial, indoors);
        OpenAllEntrances();
    }

    public void Initialize(Vector2 location, Vector2 RoomSize, Material wallMaterial, Material floorMaterial, bool indoors)
    {
        transform.position = location;
        OnInitialize(RoomSize, wallMaterial, floorMaterial, indoors);
    }
    void OnInitialize(Vector2 RoomSize, Material wallMaterial, Material floorMaterial, bool indoors)
    {
        CameraBoundaries = RoomSize;
        directions = new RoomDirections(transform, roomEntrance);
        //Build wall meshes all around the start area in a 30 x 30 square
        CreateRoom(new RoomTemplate(new Vector2Int((int)RoomSize.x, (int)RoomSize.y), new List<RoomTemplate.TileTemplate>(), indoors), wallMaterial, floorMaterial);
        wallMaterial.color = floorAndWallColor;
        floorMaterial.color = floorAndWallColor;
    }

    void CreateRoom(RoomTemplate template, Material wallMaterial_in, Material floorMaterial_in)
    {
        //This shouldn't actually get called until the doors have all been finalized, which is only when the whole dungeon is done
        //So this should not get called in OnInitialize!!
        Color color = new Color32((byte)UnityEngine.Random.Range(125, 220),(byte)UnityEngine.Random.Range(125, 220),(byte)UnityEngine.Random.Range(125, 220), 255);
        Material wallMaterial = new Material(wallMaterial_in.shader);
        wallMaterial.CopyPropertiesFromMaterial(wallMaterial_in);
        if(template.indoors)
        {
            wallMaterial.mainTexture = floorMaterial_in.mainTexture;
            wallMaterial.color = color + Color.white / 10;
        }
        Material floorMaterial = new Material(floorMaterial_in.shader);
        floorMaterial.CopyPropertiesFromMaterial(floorMaterial_in);
        floorMaterial.color = color;
        CreateWalls(template, wallMaterial);
        CreateFloor(template, floorMaterial);
    }
    void CreateWalls(RoomTemplate template, Material wallMaterial)
    {
        GameObject wallObject = new GameObject("Wall");
        wallObject.transform.parent = this.gameObject.transform;
        wallObject.AddComponent<MeshFilter>();

        if(debug.CheckTemplate == RoomDebug.RoomBuildMode.NONE || debug.CheckTemplate == RoomDebug.RoomBuildMode.BOTH)
        {
            MeshMaker.CreateWall(wallObject, wallObject.GetComponent<MeshFilter>().mesh, template.ExtractWalls());

            wallObject.AddComponent<MeshRenderer>();
            wallObject.GetComponent<MeshRenderer>().material = wallMaterial;
        }
        if(debug.CheckTemplate == RoomDebug.RoomBuildMode.CHECK_TEMPLATE || debug.CheckTemplate == RoomDebug.RoomBuildMode.BOTH)
        {
            DEBUG_TemplateCheck(template, wallObject.transform);
        }
        wallObject.transform.localPosition = new Vector3(- template.size.x / 2 + 0.5f, template.size.y / 2, 0);
    }
    void CreateFloor(RoomTemplate template, Material floorMaterial)
    {
        GameObject floorObject = new GameObject("Floor");
        floorObject.transform.parent = this.gameObject.transform;
        floorObject.AddComponent<MeshFilter>();

        MeshMaker.CreateSurface(template.ExtractFloor(), floorObject.GetComponent<MeshFilter>().mesh);
        floorObject.transform.localPosition = new Vector3(- template.size.x / 2, template.size.y / 2, 0);

        floorObject.AddComponent<MeshRenderer>();
        floorObject.GetComponent<MeshRenderer>().material = floorMaterial;

        MeshCollider mc = floorObject.AddComponent<MeshCollider>();
        mc.sharedMesh = floorObject.GetComponent<MeshFilter>().mesh;
    }
    void DEBUG_TemplateCheck(RoomTemplate template, Transform wallObject)
    {
        for(int x = 0; x < template.size.x; x++)
        {
            for(int y = 0; y < template.size.y; y++)
            {
                GameObject temp = Instantiate(debugFloor, new Vector3(x,-y - 0.5f,-1), Quaternion.identity, wallObject);
                temp.GetComponent<MeshRenderer>().material.color = template.positions[x + template.size.x * y].identity == 2 ? debug.floorColor : template.positions[x + template.size.x * y].identity == 0 ? Color.black:
                template.positions[x + template.size.x * y].read ? debug.wallColor: Color.white;
            }
        }
    }

    public Vector2 GetCameraBoundaries()
    {
        return CameraBoundaries;
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
        if(directions == null){return false;}
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
        if (directions == null)
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

        //WeedOutUnseeableWalls();
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
               /* if(roomData.m_layout == RoomLayout.NormalOutdoors)
                {
                    //BuildLayout(i, j, roomCenter, wallThickness, blueprints, parent);
                }*/
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
