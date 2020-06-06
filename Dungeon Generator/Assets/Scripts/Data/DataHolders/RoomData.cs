using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]public class RoomData
{
    public RoomType m_type = RoomType.NormalRoom;
    public RoomPosition m_roomPosition = RoomPosition.None;
    public RoomLayout m_layout = RoomLayout.NormalOutdoors;
    public bool m_Indoors = false;
    public int stepsAwayFromMainRoom = 0;
    public bool IsBuilt = false;
}
