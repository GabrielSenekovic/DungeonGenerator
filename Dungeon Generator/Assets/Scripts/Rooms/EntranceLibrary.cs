using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntranceLibrary : MonoBehaviour
{
    [SerializeField]List<Sprite> m_sprites;

    public Sprite GetSprite(RoomEntrance.EntranceType type)
    {
        switch (type)
        {
            case RoomEntrance.EntranceType.AmbushDoor:
                return m_sprites[0];
            case RoomEntrance.EntranceType.BombableWall:
                return m_sprites[1];
            case RoomEntrance.EntranceType.LockedDoor:
                return m_sprites[2];
            case RoomEntrance.EntranceType.MultiLockedDoor:
                return m_sprites[3];
            case RoomEntrance.EntranceType.PuzzleDoor:
                return m_sprites[4];
            default: return null;
        }
    }
}
