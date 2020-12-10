using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

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

    private void Awake() 
    {
        //GameData.m_LevelConstructionSeed = Random.Range(0, int.MaxValue);
        //GameData.m_LevelDataSeed = Random.Range(0, int.MaxValue);
        if(GameData.Instance != null)
        {
            GameData.SetPlayerPosition(new Vector2(GameData.GetPlayerPosition().x + RoomSize.x/2, GameData.GetPlayerPosition().y + RoomSize.y/2));
        }
        party = Party.instance;
    }
    private void Start() 
    {
        l_data = GameData.currentLevel;
        q_data = GameData.currentQuest;
        if(DebuggingTools.checkForBrokenSeeds)
        {
            try
            {
                GetComponent<LevelGenerator>().GenerateLevel(this, RoomSize);
                GetComponent<LevelGenerator>().PutDownQuestObjects(this, q_data);
            }
            catch
            {
                Debug.LogError("<color=red>Error: Found broken seed when generating!:</color> " + GameData.m_LevelConstructionSeed + " and: " + GameData.m_LevelDataSeed);
                Debug.Break();
            }
        }
        else
        {
            GetComponent<LevelGenerator>().GenerateLevel(this, RoomSize);
            GetComponent<LevelGenerator>().PutDownQuestObjects(this, q_data);
        }
        currentRoom = firstRoom;
        CameraMovement.cameraConstraints = new Vector2(firstRoom.transform.position.x + RoomSize.x/2, firstRoom.transform.position.y + RoomSize.y/2);
        CameraMovement.movementMode = CameraMovement.CameraMovementMode.SingleRoom;
    }
    private void Update()
    {
        if(!GetComponent<LevelGenerator>().levelGenerated){BuildLevel();}
        if(party == null){return;}
        if(UpdateQuest())
        {
            //Level is ended
            //Load HQ scene
            SceneManager.LoadSceneAsync("HQ");
        }
        if(CheckIfChangeRoom())
        {
            party.GetPartyLeader().GetPMM().canMove = false;
            party.movingRoom = true;
        }
        if(party.movingRoom)
        {
            if(party.LerpCamera(new Vector3(currentRoom.transform.position.x + RoomSize.x/2 - 0.5f, currentRoom.transform.position.y + RoomSize.y/2 - 0.5f,party.cameraRotationObject.transform.position.z)))
            {
                CameraMovement.cameraConstraints = new Vector2(currentRoom.transform.position.x + RoomSize.x/2, currentRoom.transform.position.y + RoomSize.y/2);
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
                GetComponent<LevelGenerator>().BuildLevel(l_data, currentRoom);
            }
            catch
            {
                Debug.LogError("<color=red>Error: Found broken seed when building!:</color> " + GameData.m_LevelConstructionSeed + " and: " + GameData.m_LevelDataSeed);
                Debug.Break();
            }
        }
        else
        {
            GetComponent<LevelGenerator>().BuildLevel(l_data, currentRoom);
        }
    }
    bool CheckIfChangeRoom()
    {
        if(party.GetPartyLeader().transform.position.x > currentRoom.transform.position.x + currentRoom.CameraBoundaries.x)
        {
            previousRoom = currentRoom;
            currentRoom = GetComponent<LevelGenerator>().FindAdjacentRoom(currentRoom, new Vector2(1,0));
            currentRoom.gameObject.SetActive(true);
            return true;
        }
        else if(party.GetPartyLeader().transform.position.x < currentRoom.transform.position.x)
        {
            previousRoom = currentRoom;
            currentRoom = GetComponent<LevelGenerator>().FindAdjacentRoom(currentRoom, new Vector2(-1,0));
            currentRoom.gameObject.SetActive(true);
            return true;
        }
        else if (party.GetPartyLeader().transform.position.y > currentRoom.transform.position.y + currentRoom.CameraBoundaries.y)
        {
            previousRoom = currentRoom;
            currentRoom = GetComponent<LevelGenerator>().FindAdjacentRoom(currentRoom, new Vector2(0, 1));
            currentRoom.gameObject.SetActive(true);
            return true;
        }
        else if (party.GetPartyLeader().transform.position.y < currentRoom.transform.position.y)
        {
            previousRoom = currentRoom;
            currentRoom = GetComponent<LevelGenerator>().FindAdjacentRoom(currentRoom, new Vector2(0, -1));
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
                return GetComponent<LevelGenerator>().spawnedEndOfLevel.isInteractedWith;
            case QuestData.MissionType.Backup:
                if(q_data.GetStatus())
                {
                    return true;
                }
                //If false, update timers about the status of the NPCs youre supposed to help
                return false;
            case QuestData.MissionType.Delivery:
                return false;
            case QuestData.MissionType.Escort:
                return false;
            case QuestData.MissionType.Hunt:
                return false;
            case QuestData.MissionType.Inquiry:
                return q_data.GetStatus();
            case QuestData.MissionType.Investigation:
                return false;
            default: return false;
        }
    }
}
