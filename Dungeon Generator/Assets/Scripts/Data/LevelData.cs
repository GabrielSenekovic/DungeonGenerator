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

public class LevelData
{
    List<AudioClip> m_melody = new List<AudioClip>();
    List<AudioClip> m_baseLine = new List<AudioClip>();

    public Vector2 m_amountOfRoomsCap = new Vector2(50, 75);

    public Mood[] m_mood = new Mood[2];
    public Biome m_biome;

    public int temperatureLevel; //0 = tepid, 1 = warm, 2 = hot, -1 = cold, -2 freezing
    public uint waterLevel; //0 = no water, 1 = some few lakes, maybe a river, 2 = High chance for lakes, probably a river, 3 = Wetland, its like everything is a lake
    public uint magicLevel; //0 = normal, 1 = may find magical stones, some elementals, 2 = many elementals may spawn, many elemental ores may be found, 3 = magical mist so strong it enhances magic stats and decreases physical stats
    public uint dangerLevel;
    public int altitude; //0 is surface level. Higher levels imply mountainous, or sky. Lower levels imply underground caverns.

    public int normalRoomProbability = 20;
    public int treasureRoomProbability = 1;
    public int ambushRoomProbability = 5;
    public int restingRoomProbability = 5;

    public int openDoorProbability = 0;
    public Vector2 roomOpenness = Vector2.zero;

    public int foragingSpawnProbability;

    public bool cave = false;
    public bool dungeon = false;

    public bool mushroom = false;
    public bool crystal = false;

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
        return treasureRoomProbability + normalRoomProbability + restingRoomProbability + ambushRoomProbability;
    }

    public float GetRoomProbability(RoomType type)
    {
        switch(type)
        {
            case RoomType.NormalRoom: return normalRoomProbability;
            case RoomType.AmbushRoom: return ambushRoomProbability;
            case RoomType.TreasureRoom: return treasureRoomProbability;
            case RoomType.RestingRoom: return restingRoomProbability;
            default: return 0;
        }
    }
    public float GetRoomPercentage(RoomType type)
    {
        return (int)(GetRoomProbability(type)/GetFullRoomProbabilityPercentage()*100);
    }
}
