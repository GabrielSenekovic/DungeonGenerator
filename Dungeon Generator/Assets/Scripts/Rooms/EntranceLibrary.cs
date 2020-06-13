using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntranceLibrary : MonoBehaviour
{
    [SerializeField]List<Sprite> entranceSprites;

    public List<Sprite> floorSprites;

    public Sprite GetSprite(RoomEntrance.EntranceType type)
    {
        switch (type)
        {
            case RoomEntrance.EntranceType.AmbushDoor:
                return entranceSprites[0];
            case RoomEntrance.EntranceType.BombableWall:
                return entranceSprites[1];
            case RoomEntrance.EntranceType.LockedDoor:
                return entranceSprites[2];
            case RoomEntrance.EntranceType.MultiLockedDoor:
                return entranceSprites[3];
            case RoomEntrance.EntranceType.PuzzleDoor:
                return entranceSprites[4];
            default: return null;
        }
    }
    public Sprite GetFloorSprite(Biome biome)
    {
        switch(biome)
        {
            case Biome.Desert:
                return floorSprites[1];
            default:
                return floorSprites[0];
        }
    }
}
