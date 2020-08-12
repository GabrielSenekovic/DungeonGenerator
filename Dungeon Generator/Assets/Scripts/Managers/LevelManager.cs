using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Vector2 RoomSize = new Vector2(20,20);

    public LevelData data;
    public Room firstRoom;
    public Room lastRoom;

    public Room currentRoom;
    public Room previousRoom = null;

    public Party party;

    private void Awake() 
    {
        GameData.m_LevelConstructionSeed = Random.Range(0, int.MaxValue);
        GameData.m_LevelDataSeed = Random.Range(0, int.MaxValue);
        if(GameData.Instance != null)
        {
            GameData.SetPlayerPosition(new Vector2(GameData.GetPlayerPosition().x + RoomSize.x/2, GameData.GetPlayerPosition().y + RoomSize.y/2));
        }
        party = Party.instance;
    }
    private void Start() 
    {
        data = GameData.currentLevel;
        if(DebuggingTools.checkForBrokenSeeds)
        {
            try
            {
                GetComponent<LevelGenerator>().GenerateLevel(this, RoomSize);
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
        }
        currentRoom = firstRoom;
        CameraMovement.cameraConstraints = new Vector2(firstRoom.transform.position.x + RoomSize.x/2, firstRoom.transform.position.y + RoomSize.y/2);
        CameraMovement.movementMode = CameraMovement.CameraMovementMode.SingleRoom;
    }
    private void Update()
    {
        if(!GetComponent<LevelGenerator>().levelGenerated)
        {
            if(DebuggingTools.checkForBrokenSeeds)
            {
                try
                {
                    GetComponent<LevelGenerator>().BuildLevel(data, currentRoom);
                }
                catch
                {
                    Debug.LogError("<color=red>Error: Found broken seed when building!:</color> " + GameData.m_LevelConstructionSeed + " and: " + GameData.m_LevelDataSeed);
                    Debug.Break();
                }
            }
            else
            {
                GetComponent<LevelGenerator>().BuildLevel(data, currentRoom);
            }
        }
        if(party == null){return;}
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
        return false;
    }
}
