using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataGenerator : MonoBehaviour
{
    public LevelData Initialize(int LevelDataSeed)
    {
        GameData.m_LevelDataSeed = LevelDataSeed;
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
        int temp = Random.Range(0, 8);
        if(temp == 7)
        {
            data.dungeon = true;
        }
    }
    public void ChooseTemp(LevelData data)
    {
        data.temperatureLevel += Random.Range(-1, 2);
    }
    public void ChooseWaterLevel(LevelData data)
    {
        data.waterLevel = (uint)Random.Range(0, 4);
    }

    public void ChooseMagicLevel(LevelData data)
    {
        data.magicLevel = (uint)Random.Range(0, 2);
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
                    data.dangerLevel++;
                    break;
                case Mood.Calm:
                    data.restingRoomProbability += 2;
                    break;
                case Mood.Creepy:
                    data.restingRoomProbability--;
                    break;
                case Mood.Cursed:
                    data.restingRoomProbability -= 2;
                    break;
                case Mood.Dangerous:
                    data.dangerLevel += 2;
                    data.ambushRoomProbability += 2;
                    data.restingRoomProbability--;
                    break;
                case Mood.Decrepit:
                    data.treasureRoomProbability += 2;
                    break;
                case Mood.Fabulous:
                    data.magicLevel++;
                    break;
                case Mood.Mysterious:
                    data.magicLevel += 2;
                    break;
                case Mood.Plentiful:
                    data.treasureRoomProbability++;
                    data.foragingSpawnProbability += 3;
                    break;
            }
        }
    } 
    public void ChooseBiome(LevelData data)
    {
        data.m_biome = (Biome)Random.Range(0, 11);
        switch(data.m_biome)
        {
            case Biome.Alpine: 
                data.roomOpenness = new Vector2(10, 20);
                data.openDoorProbability = 2;
                break;
            case Biome.Boreal:
                data.temperatureLevel--;
                data.roomOpenness = new Vector2(2, 5);
                data.openDoorProbability = 2;
                break;
            case Biome.Desert:
                data.waterLevel -= 1;
                data.roomOpenness = new Vector2(10, 20);
                data.openDoorProbability = 5;
                break;
            case Biome.Xeric:
                data.waterLevel = 0;
                data.roomOpenness = new Vector2(10, 20);
                data.openDoorProbability = 5;
                break;
            case Biome.IceCap:
                data.temperatureLevel -= 3;
                data.roomOpenness = new Vector2(10, 20);
                data.openDoorProbability = 3;
                break;
            case Biome.Mediterranean:
                data.temperatureLevel++;
                data.roomOpenness = new Vector2(2, 10);
                data.openDoorProbability = 3;
                break;
            case Biome.Ocean: 
                data.openDoorProbability = 5;
                break;
            case Biome.Rainforest:
                data.temperatureLevel += 2;
                data.roomOpenness = new Vector2(0, 10);
                data.openDoorProbability = 1;
                break;
            case Biome.Savannah:
                data.temperatureLevel++;
                data.roomOpenness = new Vector2(5, 20);
                data.openDoorProbability = 2;
                break;
            case Biome.Tundra:
                data.temperatureLevel-=2;
                data.roomOpenness = new Vector2(5, 15);
                data.openDoorProbability = 2;
                break;
            case Biome.Continental: 
                data.roomOpenness = new Vector2(0, 20);
                data.openDoorProbability = 4;
                break;
        }
    }
    public void ChooseRoomProbabilities(LevelData data)
    {
        if (data.restingRoomProbability < 0)
        {
            data.restingRoomProbability = 0;
        }
        if(data.normalRoomProbability < 0)
        {
            data.normalRoomProbability = 0;
        }
        if(data.ambushRoomProbability < 0)
        {
            data.ambushRoomProbability = 0;
        }
        if(data.treasureRoomProbability < 0)
        {
            data.treasureRoomProbability = 0;
        }
    }
}
