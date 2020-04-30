using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntranceLibrary : MonoBehaviour
{
    [SerializeField]List<Sprite> m_sprites;

    public Sprite GetSprite(EntranceType type)
    {
        switch (type)
        {
            case EntranceType.AmbushDoor:
                return m_sprites[0];
            case EntranceType.BombableWall:
                return m_sprites[1];
            case EntranceType.LockedDoor:
                return m_sprites[2];
            case EntranceType.MultiLockedDoor:
                return m_sprites[3];
            case EntranceType.PuzzleDoor:
                return m_sprites[4];
            default: return null;
        }
    }
}
