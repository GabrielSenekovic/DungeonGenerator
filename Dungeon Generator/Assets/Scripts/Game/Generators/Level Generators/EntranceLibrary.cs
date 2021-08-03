using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntranceLibrary : MonoBehaviour
{
    [SerializeField]List<Sprite> entranceSprites;

    public List<Sprite> floorSprites;

    public Sprite GetSprite(Entrance.EntranceType type)
    {
        switch (type)
        {
            case Entrance.EntranceType.AmbushDoor:
                return entranceSprites[0];
            case Entrance.EntranceType.BombableWall:
                return entranceSprites[1];
            case Entrance.EntranceType.LockedDoor:
                return entranceSprites[2];
            case Entrance.EntranceType.MultiLockedDoor:
                return entranceSprites[3];
            case Entrance.EntranceType.PuzzleDoor:
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
