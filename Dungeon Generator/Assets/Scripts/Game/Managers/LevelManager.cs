using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
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

[System.Serializable]public class LevelData
{
    List<AudioClip> m_melody = new List<AudioClip>();
    List<AudioClip> m_baseLine = new List<AudioClip>();

    public Vector2Int m_amountOfRoomsCap = new Vector2Int(1, 2);

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

public class LevelDataGenerator : MonoBehaviour
{
    static public LevelData Initialize(int LevelDataSeed)
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
    static void ChooseLocation(LevelData data)
    {
        int temp = Random.Range(0, 8);
        if(temp == 7)
        {
            data.dungeon = true;
        }
    }
    static void ChooseTemp(LevelData data)
    {
        data.temperatureLevel += Random.Range(-1, 2);
    }
    static void ChooseWaterLevel(LevelData data)
    {
        data.waterLevel = (uint)Random.Range(0, 4);
    }

    static void ChooseMagicLevel(LevelData data)
    {
        data.magicLevel = (uint)Random.Range(0, 2);
    }
    static void ChooseMood(LevelData data)
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
    static void ChooseBiome(LevelData data)
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
    static void ChooseRoomProbabilities(LevelData data)
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

public class LevelManager : MonoBehaviour
{
    public Vector2 RoomSize = new Vector2(20,20);

    public LevelData l_data;
    public QuestData q_data;

    public Room firstRoom;
    public Room lastRoom;

    public Room currentRoom;
    public Room previousRoom = null;

    public Party party;

    LevelGenerator generator;

    private void Awake() 
    {
        //GameData.m_LevelConstructionSeed = Random.Range(0, int.MaxValue);
        //GameData.m_LevelDataSeed = Random.Range(0, int.MaxValue);
        if(GameData.Instance != null)
        {
            GameData.SetPlayerPosition(new Vector2(GameData.GetPlayerPosition().x + RoomSize.x/2, GameData.GetPlayerPosition().y + RoomSize.y/2));
        }
    }
    private void Start() 
    {
        party = Party.instance;
        l_data = GameData.GetCurrentLevelData();
        l_data.dungeon = true;
        q_data = GameData.GetCurrentQuestData();
        generator = GetComponent<LevelGenerator>();
        if(DebuggingTools.checkForBrokenSeeds)
        {
            try
            {
                generator.GenerateLevel(this, RoomSize, l_data.m_amountOfRoomsCap);
                generator.PutDownQuestObjects(this, q_data);
            }
            catch
            {
                Debug.LogError("<color=red>Error: Found broken seed when generating!:</color> " + GameData.m_LevelConstructionSeed + " and: " + GameData.m_LevelDataSeed);
                Debug.Break();
            }
        }
        else
        {
            generator.GenerateLevel(this, RoomSize, l_data.m_amountOfRoomsCap);
            generator.PutDownQuestObjects(this, q_data);
        }
        currentRoom = firstRoom;
        CameraMovement.SetCameraAnchor(new Vector2(firstRoom.transform.position.x, firstRoom.transform.position.y));
        CameraMovement.movementMode = CameraMovement.CameraMovementMode.SingleRoom;
    }
    private void Update()
    {
        if(!generator.levelGenerated){BuildLevel();}
        if(party == null){return;}
        if(UpdateQuest())
        {
            //Level is ended
            //Load HQ scene
            SceneManager.LoadSceneAsync("HQ");
        }
        if(CheckIfChangeRoom())
        {
            party.GetPartyLeader().GetPMM().SetCanMove(false);
            CameraMovement.SetMovingRoom(true);
        }
    }
    private void LateUpdate()
    {
        if (CameraMovement.GetMovingRoom())
        {
            if (CameraMovement.Instance.MoveCamera(new Vector3(currentRoom.transform.position.x, currentRoom.transform.position.y, CameraMovement.GetRotationObject().transform.position.z), previousRoom.transform.position))
            {
                CameraMovement.SetCameraAnchor(new Vector2(currentRoom.transform.position.x , currentRoom.transform.position.y));
                previousRoom.gameObject.SetActive(false);
            }
        }
    }
    void BuildLevel()
    {
        if (DebuggingTools.checkForBrokenSeeds)
        {
            try
            {
                generator.BuildLevel(l_data, currentRoom);
            }
            catch
            {
                Debug.LogError("<color=red>Error: Found broken seed when building!:</color> " + GameData.m_LevelConstructionSeed + " and: " + GameData.m_LevelDataSeed);
                Debug.Break();
            }
        }
        else
        {
            generator.BuildLevel(l_data, currentRoom);
        }
    }
    bool CheckIfChangeRoom()
    {
        if(party.GetPartyLeader().transform.position.x > currentRoom.transform.position.x + currentRoom.CameraBoundaries.x / 2)
        {
            previousRoom = currentRoom;
            currentRoom = generator.FindAdjacentRoom(currentRoom, new Vector2(1,0));
            currentRoom.gameObject.SetActive(true);
            return true;
        }
        else if(party.GetPartyLeader().transform.position.x < currentRoom.transform.position.x - currentRoom.CameraBoundaries.x / 2)
        {
            previousRoom = currentRoom;
            currentRoom = generator.FindAdjacentRoom(currentRoom, new Vector2(-1,0));
            currentRoom.gameObject.SetActive(true);
            return true;
        }
        else if (party.GetPartyLeader().transform.position.y > currentRoom.transform.position.y + currentRoom.CameraBoundaries.y / 2)
        {
            previousRoom = currentRoom;
            currentRoom = generator.FindAdjacentRoom(currentRoom, new Vector2(0, 1));
            currentRoom.gameObject.SetActive(true);
            return true;
        }
        else if (party.GetPartyLeader().transform.position.y < currentRoom.transform.position.y - currentRoom.CameraBoundaries.y / 2)
        {
            previousRoom = currentRoom;
            currentRoom = generator.FindAdjacentRoom(currentRoom, new Vector2(0, -1));
            currentRoom.gameObject.SetActive(true);
            return true;
        }
        return false;
    }
    bool UpdateQuest()
    {
        switch(q_data.missionType)
        {
            case QuestData.MissionType.Recovery:
                return generator.spawnedEndOfLevel.isInteractedWith;
            case QuestData.MissionType.Backup:
                if(q_data.GetStatus())
                {
                    return true;
                }
                //If false, update timers about the status of the NPCs youre supposed to help
                return false;
            case QuestData.MissionType.Delivery:
            case QuestData.MissionType.Escort:
            case QuestData.MissionType.Hunt:
            case QuestData.MissionType.Inquiry:
            case QuestData.MissionType.Investigation:
                return q_data.GetStatus();
            default: return false;
        }
    }
}
