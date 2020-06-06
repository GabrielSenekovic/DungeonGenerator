using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Biome
{
    Continental = 0,
    Xeric = 1,
    Desert = 2,
    Savannah = 3,
    Alpine = 4,
    IceCap = 5,
    Tundra = 6,
    Boreal = 7,
    Mediterranean = 8,
    Rainforest = 9,
    Ocean = 10
}

public enum Mood
{
    Creepy = 0, //Grayish, ghostly, bones, discordant, misty
    Mysterious = 1, //Saturated, Sparkly, Misty, Slow, Invisible, Magical
    Calm = 2, //Not many enemies, calm music, desaturated, more safe rooms, maybe villages
    Decrepit = 3, //Ruins, adventurous, full of treasures, curious
    Adventurous = 4, //Higher chance for altitudal elements, higher pace, more enemies
    Dangerous = 5,  //High spawn rate, scary music, no chill, very hazardous, many traps
    Cursed = 6, //Rooms may be cursed, almost no safe zones, ghosts spawn
    Fabulous = 7, //Many flowers, sparkly water, elementals, saturated, crystals
    Plentiful = 8 //Increases rate of forage materials, thick shrubbery, more treasure rooms
}

public enum LevelLocation
{
    Overworld = 0,
    Cave = 1,
    Dungeon = 2
}

public class LevelData
{
    List<AudioClip> m_melody = new List<AudioClip>();
    List<AudioClip> m_baseLine = new List<AudioClip>();

    public Vector2 m_amountOfRoomsCap = new Vector2(50, 75);

    public Mood[] m_mood = new Mood[2];
    public Biome m_biome;
    public LevelLocation m_location;

    public int m_temperatureLevel; //0 = tepid, 1 = warm, 2 = hot, -1 = cold, -2 freezing
    public uint m_waterLevel; //0 = no water, 1 = some few lakes, maybe a river, 2 = High chance for lakes, probably a river, 3 = Wetland, its like everything is a lake
    public uint m_magicLevel; //0 = normal, 1 = may find magical stones, some elementals, 2 = many elementals may spawn, many elemental ores may be found, 3 = magical mist so strong it enhances magic stats and decreases physical stats
    public uint m_dangerLevel;
    public int m_altitude; //0 is surface level. Higher levels imply mountainous, or sky. Lower levels imply underground caverns.

    public int m_normalRoomProbability = 10;
    public int m_treasureRoomProbability = 1;
    public int m_ambushRoomProbability = 1;
    public int m_safeRoomProbability = 1;

    public int openDoorProbability = 0;
    public Vector2 roomOpenness = Vector2.zero;

    public int m_foragingSpawnProbability;

    //List<Flora> m_flora = new List<Flora>{}
    //List<Enemy> m_enemies = new List<Enemy>{}
    //List<Entity> m_fauna = new List<Entity>{}

    public Mood GetMood(int i)
    {
        return m_mood[i];
    }

    public float GetFullRoomProbabilityPercentage()
    {
        //Debug.Log("Full probability: " + (m_treasureRoomProbability + m_normalRoomProbability + m_safeRoomProbability + m_ambushRoomProbability));
        return m_treasureRoomProbability + m_normalRoomProbability + m_safeRoomProbability + m_ambushRoomProbability;
    }

    public float GetTreasureRoomPercentage()
    {
        return (int)(m_treasureRoomProbability/GetFullRoomProbabilityPercentage() * 100);
    }
    public int GetTreasureRoomProbability()
    {
        return m_treasureRoomProbability;
    }
    public float GetAmbushRoomPercentage()
    {
        return (int)(m_ambushRoomProbability / GetFullRoomProbabilityPercentage() * 100);
    }
    public int GetAmbushRoomProbability()
    {
        return m_ambushRoomProbability;
    }
    public float GetNormalRoomPercentage()
    {
        return (int)(m_normalRoomProbability / GetFullRoomProbabilityPercentage() * 100);
    }
    public int GetNormalRoomProbability()
    {
        return m_normalRoomProbability;
    }
    public float GetSafeRoomPercentage()
    {
        return (int)(m_safeRoomProbability / GetFullRoomProbabilityPercentage() * 100);
    }
    public int GetSafeRoomProbability()
    {
        return m_safeRoomProbability;
    }
}
