using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorRoom : Room
{
    public override void PlaceDownInnerWalls(WallBlueprints blueprints, Transform parent)
    {
        //i should count up to the width of the room, j should count to the height
        //Should check to make sure that theres at least a tile to the x or the y, unless its spawning in the middle of the room
        if(roomData.m_roomPosition != RoomPosition.DeadEnd && roomData.m_type != RoomType.AmbushRoom)
        {
            int frameCorridorThickness = 0;
            int frameCorridorOffset = 0;
            int corridorThickness = 0;
            bool checkered = false;
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
            for (int i = 1; i < CameraBoundaries.x - 1; i++)
            {
                if(i == 1 + frameCorridorOffset || i == CameraBoundaries.x - 1 - frameCorridorThickness - frameCorridorOffset )
                {
                    if(!checkered)
                    {
                        OnFillOutCheckeredEdges(i, frameCorridorThickness, frameCorridorOffset, blueprints, parent, false);
                    }
                    i += frameCorridorThickness;
                }
                for (int j = 1; j < CameraBoundaries.y - 1; j++)
                {
                    if (j == 1 + frameCorridorOffset || j == CameraBoundaries.y - 1 - frameCorridorThickness - frameCorridorOffset)
                    {
                        if(!checkered)
                        {
                            OnFillOutCheckeredEdges(j, frameCorridorThickness, frameCorridorOffset, blueprints, parent, true);
                        }
                        j += frameCorridorThickness;
                    }
                    if(GetIfCorridor(i, j, corridorThickness))
                    {
                    continue;
                    }
                    GameObject newWall = Instantiate(blueprints.wallBlock, new Vector2(transform.position.x + i,transform.position.y + j), Quaternion.identity, parent);
                    newWall.GetComponentInChildren<SpriteRenderer>().color = GetWallColor();
                }
            }
        }
    }
    public void OnFillOutCheckeredEdges(int i, int frameCorridorThickness, int frameCorridorOffset, WallBlueprints blueprints, Transform parent, bool reverse)
    {
        //If a corridor room spawns a square corridor inside it, it will portrude on each edge. These must be filled up if the room isnt checkered
        if(!reverse)
        {
            for (int k = i; k < i + frameCorridorThickness; k++)
            {
                for (int l = 1; l < 1 + frameCorridorOffset; l++)
                {
                    wallPositions[k][l].PlaceDown();
                    GameObject newWall = Instantiate(blueprints.wallBlock, new Vector2(transform.position.x + k,transform.position.y + l), Quaternion.identity, parent);
                    newWall.GetComponentInChildren<SpriteRenderer>().color = GetWallColor();
                }
                for (int l = (int)CameraBoundaries.y - 1 - frameCorridorOffset; l < CameraBoundaries.y - 1; l++)
                {
                    wallPositions[k][l].PlaceDown();
                    GameObject newWall = Instantiate(blueprints.wallBlock, new Vector2(transform.position.x + k,transform.position.y + l), Quaternion.identity, parent);
                    newWall.GetComponentInChildren<SpriteRenderer>().color = GetWallColor();
                }
            }
        }
        else
        {
            for (int k = i; k < i + frameCorridorThickness; k++)
            {
                for (int l = 1; l < 1 + frameCorridorOffset; l++)
                {
                    wallPositions[l][k].PlaceDown();
                    GameObject newWall2 = Instantiate(blueprints.wallBlock, new Vector2(transform.position.x + l,transform.position.y + k), Quaternion.identity, parent);
                    newWall2.GetComponentInChildren<SpriteRenderer>().color = GetWallColor();
                }
                for (int l = (int)CameraBoundaries.x - 1 - frameCorridorOffset; l < CameraBoundaries.x - 1; l++)
                {
                    wallPositions[l][k].PlaceDown();
                    GameObject newWall2 = Instantiate(blueprints.wallBlock, new Vector2(transform.position.x + l,transform.position.y + k), Quaternion.identity, parent);
                    newWall2.GetComponentInChildren<SpriteRenderer>().color = GetWallColor();
                }
            }
        }
    }
    public bool GetIfCorridor(int i, int j, int corridorThickness)
    {
        //This code chops out the corridor out of the room
        //It is dependent on the frame being PlaceDown(). It doesnt work otherwise. When you remove the wallPosition thingies, keep this in mind
        if (i - 3 < 0 || i + (int)CameraBoundaries.x - 3 > CameraBoundaries.x - 1)
        {
            if (i > CameraBoundaries.x / 2 - 2 - corridorThickness)
            {
                //Controls right corridor of room
                if (!wallPositions[(int)CameraBoundaries.x - 1][j].GetIsOccupied())
                {
                    return true;
                }
                else
                {
                    if(j - corridorThickness > 0 && j + corridorThickness < CameraBoundaries.y - 1)
                        //This only makes sure the index isnt out of range
                    {
                        bool toContinue = false;
                        for(int k = 0; k <= corridorThickness; k++) //Im not sure it makes sense to do this for every single j, but for now it works, but Id rather do this more methodically down the road
                        {
                            if (!wallPositions[(int)CameraBoundaries.x - 1][j + k].GetIsOccupied())
                            {
                                toContinue = true;
                            }
                            else if (!wallPositions[(int)CameraBoundaries.x - 1][j - k].GetIsOccupied())
                            {
                                toContinue = true;
                            }
                        }
                        if(toContinue)
                        {
                            return true;
                        }
                    }
                }
            }
            if (i < CameraBoundaries.x / 2 +1 + corridorThickness)
            {
                //Controls left corridor of room
                if (!wallPositions[0][j].GetIsOccupied())
                {
                    return true;
                }
                else
                {
                    if (j - corridorThickness > 0 && j + corridorThickness < CameraBoundaries.y - 1)
                    {
                        bool toContinue = false;
                        for (int k = 0; k <= corridorThickness; k++) //Im not sure it makes sense to do this for every single j, but for now it works, but Id rather do this more methodically down the road
                        {
                            if (!wallPositions[0][j + k].GetIsOccupied())
                            {
                                toContinue = true;
                            }
                            else if (!wallPositions[0][j - k].GetIsOccupied())
                            {
                                toContinue = true;
                            }
                        }
                        if (toContinue)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        if (j - 3 < 0 || j + (int)CameraBoundaries.y - 3 > CameraBoundaries.y - 1)
        {
            if (j > CameraBoundaries.y / 2 -2) //-2
            {
                if (!wallPositions[i][(int)CameraBoundaries.y - 1].GetIsOccupied())
                {
                    return true;
                }
                else
                {
                    if (i - corridorThickness > 0 && i + corridorThickness < CameraBoundaries.x - 1)
                    //This only makes sure the index isnt out of range
                    {
                        bool toContinue = false;
                        for (int k = 0; k <= corridorThickness; k++) //Im not sure it makes sense to do this for every single j, but for now it works, but Id rather do this more methodically down the road
                        {
                            if (!wallPositions[i + k][(int)CameraBoundaries.y - 1].GetIsOccupied())
                            {
                                toContinue = true;
                            }
                            else if (!wallPositions[i - k][(int)CameraBoundaries.y - 1].GetIsOccupied())
                            {
                                toContinue = true;
                            }
                        }
                        if (toContinue)
                        {
                            return true;
                        }
                    }
                }
            }
            if (j < CameraBoundaries.y / 2+1 ) //+2
            {
                if (!wallPositions[i][0].GetIsOccupied())
                {
                    return true;
                }
                else
                {
                    if (i - corridorThickness > 0 && i + corridorThickness < CameraBoundaries.x - 1)
                    //This only makes sure the index isnt out of range
                    {
                        bool toContinue = false;
                        for (int k = 0; k <= corridorThickness; k++) //Im not sure it makes sense to do this for every single j, but for now it works, but Id rather do this more methodically down the road
                        {
                            if (!wallPositions[i + k][0].GetIsOccupied())
                            {
                                toContinue = true;
                            }
                            else if (!wallPositions[i - k][0].GetIsOccupied())
                            {
                                toContinue = true;
                            }
                        }
                        if (toContinue)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
}
