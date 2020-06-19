using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseRoomBuilder : MonoBehaviour
{
    public List<GameObject> InteriorWalls;
    public List<GameObject> ExteriorWalls;
    [SerializeField] Vector2 RoomSize;
    [SerializeField] Vector2 EntrancePosition;

    public void Start()
    {
        InstantiateInteriorWalls();
        InstantiateExteriorWalls();
    }
    void InstantiateInteriorWalls()
    {
        GameObject corner1 = Instantiate(InteriorWalls[1], transform.position, Quaternion.identity, transform);
        corner1.transform.eulerAngles = new Vector3(-180, -90, 90);
        corner1.transform.position = new Vector3(corner1.transform.position.x, corner1.transform.position.y - 1, corner1.transform.position.z + 1.25f);
        GameObject corner2 = Instantiate(InteriorWalls[1], new Vector3(transform.position.x + RoomSize.x - 2, transform.position.y - 1, transform.position.z + 1.25f), Quaternion.identity, transform);
        corner2.transform.eulerAngles = new Vector3(90, 90, -90);
        GameObject corner3 = Instantiate(InteriorWalls[1], new Vector3(transform.position.x + RoomSize.x - 2, transform.position.y - RoomSize.y + 1, transform.position.z + 1.25f), Quaternion.identity, transform);
        corner3.transform.eulerAngles = new Vector3(0, -90, 90);
        GameObject corner4 = Instantiate(InteriorWalls[1], new Vector3(transform.position.x, transform.position.y - RoomSize.y + 1, transform.position.z + 1.25f), Quaternion.identity, transform);
        corner4.transform.eulerAngles = new Vector3(-90, -90, 90);
        for(int i = 1; i < RoomSize.x - 1; i++)
        {
            GameObject temp = Instantiate(InteriorWalls[0], new Vector3(transform.position.x + i - 1, transform.position.y - 1, transform.position.z + 1.25f), Quaternion.identity, transform);
            temp.transform.eulerAngles = new Vector3(90, 90, -90);
        }
        for(int i = 1; i < RoomSize.y - 1; i++)
        {
            GameObject temp = Instantiate(InteriorWalls[0], new Vector3(transform.position.x + RoomSize.x - 2, transform.position.y-i, transform.position.z + 1.25f), Quaternion.identity, transform);
            temp.transform.eulerAngles = new Vector3(0, -90, 90);
        }
        for(int i = 1; i < RoomSize.y - 1; i++)
        {
            GameObject temp = Instantiate(InteriorWalls[0], new Vector3(transform.position.x, transform.position.y-i - 1, transform.position.z + 1.25f), Quaternion.identity, transform);
            temp.transform.eulerAngles = new Vector3(-180, -90, 90);
        }
        for(int i = 1; i < RoomSize.x - 1; i++)
        {
            if(i != EntrancePosition.x && i != EntrancePosition.y)
            {
                GameObject temp = Instantiate(InteriorWalls[0], new Vector3(transform.position.x + i, transform.position.y-RoomSize.y + 1, transform.position.z + 1.25f), Quaternion.identity, transform);
                temp.transform.eulerAngles = new Vector3(-90, -90, 90);
            }
        }
    }
    void InstantiateExteriorWalls()
    {
        GameObject corner1 = Instantiate(ExteriorWalls[1], transform.position, Quaternion.identity, transform);
        corner1.transform.eulerAngles = new Vector3(-180, -90, 90);
        corner1.transform.position = new Vector3(corner1.transform.position.x + RoomSize.x , corner1.transform.position.y, corner1.transform.position.z + 1.25f);
        GameObject corner2 = Instantiate(ExteriorWalls[1], new Vector3(transform.position.x + RoomSize.x - 1, transform.position.y - RoomSize.y - 1, transform.position.z + 1.25f), Quaternion.identity, transform);
        corner2.transform.eulerAngles = new Vector3(90, 90, -90);
        GameObject corner3 = Instantiate(ExteriorWalls[1], new Vector3(transform.position.x - 2, transform.position.y - RoomSize.y, transform.position.z + 1.25f), Quaternion.identity, transform);
        corner3.transform.eulerAngles = new Vector3(0, -90, 90);
        GameObject corner4 = Instantiate(ExteriorWalls[1], new Vector3(transform.position.x - 1, transform.position.y + 1, transform.position.z + 1.25f), Quaternion.identity, transform);
        corner4.transform.eulerAngles = new Vector3(-90, -90, 90);
        for(int i = 0; i < RoomSize.x; i++)
        {
            GameObject temp = Instantiate(ExteriorWalls[0], new Vector3(transform.position.x + i, transform.position.y + 1, transform.position.z + 1.25f), Quaternion.identity, transform);
            temp.transform.eulerAngles = new Vector3(-90, -90, 90);
        }
        for(int i = 0; i < RoomSize.y; i++)
        {
            GameObject temp = Instantiate(ExteriorWalls[0], new Vector3(transform.position.x + RoomSize.x, transform.position.y-i - 1, transform.position.z + 1.25f), Quaternion.identity, transform);
            temp.transform.eulerAngles = new Vector3(-180, -90, 90);
        }
        for(int i = 0; i < RoomSize.y; i++)
        {
            GameObject temp = Instantiate(ExteriorWalls[0], new Vector3(transform.position.x-2, transform.position.y-i, transform.position.z + 1.25f), Quaternion.identity, transform);
            temp.transform.eulerAngles = new Vector3(0, -90, 90);
        }
        for(int i = 0; i < RoomSize.x; i++)
        {
            if(i != EntrancePosition.x && i != EntrancePosition.y)
            {
                GameObject temp = Instantiate(ExteriorWalls[0], new Vector3(transform.position.x + i - 1, transform.position.y-RoomSize.y - 1, transform.position.z + 1.25f), Quaternion.identity, transform);
               
                temp.transform.eulerAngles = new Vector3(90, 90, -90);
            }
        }
    }
}
