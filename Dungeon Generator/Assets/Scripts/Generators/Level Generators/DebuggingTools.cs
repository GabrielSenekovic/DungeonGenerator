using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuggingTools : MonoBehaviour
{
    public bool checkForBrokenSeeds_in;
    public static bool checkForBrokenSeeds;

    public bool spawnOnlyBasicRooms_in;
    public static bool spawnOnlyBasicRooms;

    public bool isDungeon_in;
    public static bool isDungeon;

    public bool displayFuseRoomDebugLogs_in;
    public static bool displayFuseRoomDebugLogs;
    public bool displayRoomConstructionDebugLogs_in;
    public static bool displayRoomConstructionDebugLogs;
    public bool displayRoomEntranceSprites_in;
    public static bool displayRoomEntranceSprites;

    private void Awake() 
    {
        checkForBrokenSeeds = checkForBrokenSeeds_in;
        isDungeon = isDungeon_in;
        displayFuseRoomDebugLogs = displayFuseRoomDebugLogs_in;
        displayRoomConstructionDebugLogs = displayRoomConstructionDebugLogs_in;
        spawnOnlyBasicRooms = spawnOnlyBasicRooms_in;
        displayRoomEntranceSprites = displayRoomEntranceSprites_in;
    }
}
