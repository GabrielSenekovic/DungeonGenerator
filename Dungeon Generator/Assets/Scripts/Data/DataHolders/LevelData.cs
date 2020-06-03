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

    Vector2 m_amountOfRoomsCap = new Vector2(50, 75);

    Mood[] m_mood = new Mood[2];
    Biome m_biome;
    LevelLocation m_location;

    int m_temperatureLevel; //0 = tepid, 1 = warm, 2 = hot, -1 = cold, -2 freezing
    uint m_waterLevel; //0 = no water, 1 = some few lakes, maybe a river, 2 = High chance for lakes, probably a river, 3 = Wetland, its like everything is a lake
    uint m_magicLevel; //0 = normal, 1 = may find magical stones, some elementals, 2 = many elementals may spawn, many elemental ores may be found, 3 = magical mist so strong it enhances magic stats and decreases physical stats
    uint m_dangerLevel;
    int m_altitude; //0 is surface level. Higher levels imply mountainous, or sky. Lower levels imply underground caverns.

    int m_normalRoomProbability = 10;
    int m_treasureRoomProbability = 1;
    int m_ambushRoomProbability = 1;
    int m_safeRoomProbability = 1;

    int m_foragingSpawnProbability;

    //List<Flora> m_flora = new List<Flora>{}
    //List<Enemy> m_enemies = new List<Enemy>{}
    //List<Entity> m_fauna = new List<Entity>{}

    public void Initialize(int LevelDataSeed)
    {
        Random.InitState(LevelDataSeed);
        ChooseLocation();
        ChooseTemp();
        ChooseWaterLevel();
        ChooseMagicLevel();
        ChooseMood();
        ChooseBiome();
        ChooseRoomProbabilities();
    }

    public void ChooseLocation()
    {
        m_location = (LevelLocation)Random.Range(0, 3);
    }

    public void ChooseMood()
    {
        m_mood[0] = (Mood)Random.Range(0, 9);
        m_mood[1] = (Mood)Random.Range(0, 9);
        foreach(Mood mood in m_mood)
        {
            switch(mood)
            {
                case Mood.Adventurous:
                    m_dangerLevel++;
                    break;
                case Mood.Calm:
                    m_safeRoomProbability += 2;
                    break;
                case Mood.Creepy:
                    m_safeRoomProbability--;
                    break;
                case Mood.Cursed:
                    m_safeRoomProbability -= 2;
                    break;
                case Mood.Dangerous:
                    m_dangerLevel += 2;
                    m_ambushRoomProbability += 2;
                    m_safeRoomProbability--;
                    break;
                case Mood.Decrepit:
                    m_treasureRoomProbability += 2;
                    break;
                case Mood.Fabulous:
                    m_magicLevel++;
                    break;
                case Mood.Mysterious:
                    m_magicLevel += 2;
                    break;
                case Mood.Plentiful:
                    m_treasureRoomProbability++;
                    m_foragingSpawnProbability += 3;
                    break;
            }
        }
    } 

    public void ChooseBiome()
    {
        m_biome = (Biome)Random.Range(0, 11);
        switch(m_biome)
        {
            case Biome.Alpine: break;
            case Biome.Boreal:
                m_temperatureLevel--;
                break;
            case Biome.Desert:
                m_waterLevel -= 1;
                break;
            case Biome.Xeric:
                m_waterLevel = 0;
                break;
            case Biome.IceCap:
                m_temperatureLevel -= 3;
                Debug.Log(m_temperatureLevel);
                break;
            case Biome.Mediterranean:
                m_temperatureLevel++;
                break;
            case Biome.Ocean: break;
            case Biome.Rainforest:
                m_temperatureLevel += 2;
                break;
            case Biome.Savannah:
                m_temperatureLevel++;
                break;
            case Biome.Tundra:
                m_temperatureLevel-=2;
                break;
            case Biome.Continental: break;
        }
    }

    public void ChooseTemp()
    {
        m_temperatureLevel += Random.Range(-1, 2);
    }

    public void ChooseWaterLevel()
    {
        m_waterLevel = (uint)Random.Range(0, 4);
    }

    public void ChooseMagicLevel()
    {
        m_magicLevel = (uint)Random.Range(0, 2);
    }

    public void ChooseRoomProbabilities()
    {
        if (m_safeRoomProbability < 0)
        {
            m_safeRoomProbability = 0;
        }
        if(m_normalRoomProbability < 0)
        {
            m_normalRoomProbability = 0;
        }
        if(m_ambushRoomProbability < 0)
        {
            m_ambushRoomProbability = 0;
        }
        if(m_treasureRoomProbability < 0)
        {
            m_treasureRoomProbability = 0;
        }
    }

    public LevelLocation GetLocation()
    {
        return m_location;
    }

    public Biome GetBiome()
    {
        return m_biome;
    }

    public Mood GetMood(int i)
    {
        return m_mood[i];
    }

    public int GetTemperature()
    {
        return m_temperatureLevel;
    }

    public uint GetWaterLevel()
    {
        return m_waterLevel;
    }

    public uint GetMagicLevel()
    {
        return m_magicLevel;
    }

    public int GetAltitude()
    {
        return m_altitude;
    }

    public uint GetDangerLevel()
    {
        return m_dangerLevel;
    }
    
    public Vector2 GetRoomAmountCap()
    {
        return m_amountOfRoomsCap;
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
