using UnityEngine;
using System.Collections;

public enum DungeonType
{
    Cistern = 0,
    Sewers = 1,
    Windmill = 2,
    Fortress = 3,
    Castle = 4,
    Citadel = 5,
    Cathedral = 6,
    Tomb = 7,
    CrawlSpace = 8,
    Spire = 9,
    LightHouse = 10,
    Pyramid = 11
}

public class DungeonData : MonoBehaviour
{
    DungeonType m_dungeonType;
    public void Initialize()
    {
        m_dungeonType = DungeonType.Sewers;
    }
    public DungeonType GetDungeonType()
    {
        return m_dungeonType;
    }
}
