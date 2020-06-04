using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataGenerator : MonoBehaviour
{
    public LevelData Initialize(int LevelDataSeed)
    {
        LevelData data = new LevelData();
        Random.InitState(LevelDataSeed);
        ChooseLocation(data);
        ChooseTemp(data);
        ChooseWaterLevel(data);
        ChooseMagicLevel(data);
        ChooseMood(data);
        ChooseBiome(data);
        ChooseRoomProbabilities(data);
        return data;
    }
    public void ChooseLocation(LevelData data)
    {
        data.m_location = (LevelLocation)Random.Range(0, 3);
    }
    public void ChooseTemp(LevelData data)
    {
        data.m_temperatureLevel += Random.Range(-1, 2);
    }
    public void ChooseWaterLevel(LevelData data)
    {
        data.m_waterLevel = (uint)Random.Range(0, 4);
    }

    public void ChooseMagicLevel(LevelData data)
    {
        data.m_magicLevel = (uint)Random.Range(0, 2);
    }
     public void ChooseMood(LevelData data)
    {
        data.m_mood[0] = (Mood)Random.Range(0, 9);
        data.m_mood[1] = (Mood)Random.Range(0, 9);
        foreach(Mood mood in data.m_mood)
        {
            switch(mood)
            {
                case Mood.Adventurous:
                    data.m_dangerLevel++;
                    break;
                case Mood.Calm:
                    data.m_safeRoomProbability += 2;
                    break;
                case Mood.Creepy:
                    data.m_safeRoomProbability--;
                    break;
                case Mood.Cursed:
                    data.m_safeRoomProbability -= 2;
                    break;
                case Mood.Dangerous:
                    data.m_dangerLevel += 2;
                    data.m_ambushRoomProbability += 2;
                    data.m_safeRoomProbability--;
                    break;
                case Mood.Decrepit:
                    data.m_treasureRoomProbability += 2;
                    break;
                case Mood.Fabulous:
                    data.m_magicLevel++;
                    break;
                case Mood.Mysterious:
                    data.m_magicLevel += 2;
                    break;
                case Mood.Plentiful:
                    data.m_treasureRoomProbability++;
                    data.m_foragingSpawnProbability += 3;
                    break;
            }
        }
    } 
    public void ChooseBiome(LevelData data)
    {
        data.m_biome = (Biome)Random.Range(0, 11);
        switch(data.m_biome)
        {
            case Biome.Alpine: break;
            case Biome.Boreal:
                data.m_temperatureLevel--;
                break;
            case Biome.Desert:
                data.m_waterLevel -= 1;
                break;
            case Biome.Xeric:
                data.m_waterLevel = 0;
                break;
            case Biome.IceCap:
                data.m_temperatureLevel -= 3;
                Debug.Log(data.m_temperatureLevel);
                break;
            case Biome.Mediterranean:
                data.m_temperatureLevel++;
                break;
            case Biome.Ocean: break;
            case Biome.Rainforest:
                data.m_temperatureLevel += 2;
                break;
            case Biome.Savannah:
                data.m_temperatureLevel++;
                break;
            case Biome.Tundra:
                data.m_temperatureLevel-=2;
                break;
            case Biome.Continental: break;
        }
    }
    public void ChooseRoomProbabilities(LevelData data)
    {
        if (data.m_safeRoomProbability < 0)
        {
            data.m_safeRoomProbability = 0;
        }
        if(data.m_normalRoomProbability < 0)
        {
            data.m_normalRoomProbability = 0;
        }
        if(data.m_ambushRoomProbability < 0)
        {
            data.m_ambushRoomProbability = 0;
        }
        if(data.m_treasureRoomProbability < 0)
        {
            data.m_treasureRoomProbability = 0;
        }
    }
}
