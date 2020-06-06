using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomDirections : MonoBehaviour
{
    [SerializeField] RoomEntrance m_entrance; //this shall not be here later on, there needs only be one
    public List<RoomEntrance> directions;
    public void Awake()
    {
        //Debug.Log("Instantiating entrances");
        //m_directions = new RoomEntrance[4] 
        //{
        //    Instantiate(m_entrance, transform),
        //    Instantiate(m_entrance, transform),
        //    Instantiate(m_entrance, transform),
        //    Instantiate(m_entrance, transform)
        //};
        for(int i = 0; i < 4; i++)
        {
            directions.Add(Instantiate(m_entrance, transform));
        }
        directions[0].SetDirectionModifier(new Vector2(0, 1)); directions[0].name = "North Entrance";
        directions[1].SetDirectionModifier(new Vector2(1, 0)); directions[1].name = "Right Entrance";
        directions[2].SetDirectionModifier(new Vector2(-1, 0)); directions[2].name = "Left Entrance";
        directions[3].SetDirectionModifier(new Vector2(0, -1)); directions[3].name = "South Entrance";
    }
    public void OpenAllEntrances()
    {
        foreach(RoomEntrance entrance in directions)
        {
            entrance.Open = true;
        }
    }
}
