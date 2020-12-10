using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    int amountOfEnemiesInRoom;
    bool allEnemiesOfRoomDefeated()
    {
        return amountOfEnemiesInRoom == 0;
    }
}
