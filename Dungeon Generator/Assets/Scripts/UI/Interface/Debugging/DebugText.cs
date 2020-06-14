using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    Text m_DebugText;

    public void Display(LevelData levelData)
    {
        m_DebugText = GetComponentInChildren<Text>();
        m_DebugText.text += "Level Construction Seed: " + GameData.m_LevelConstructionSeed + "\n";
        m_DebugText.text += "Level Data Seed: " + GameData.m_LevelDataSeed + "\n";
        m_DebugText.text += "Is Dungeon: " + levelData.dungeon + "\n";
        m_DebugText.text += "Biome: " + levelData.m_biome + "\n";
        m_DebugText.text += "Mood 1: " + levelData.GetMood(0) + "\n";
        m_DebugText.text += "Mood 2: " + levelData.GetMood(1) + "\n";
        m_DebugText.text += "Danger Levels: " + levelData.dangerLevel + "\n";
        m_DebugText.text += "Temperature: " + levelData.temperatureLevel + "\n";
        m_DebugText.text += "Water Level: " + levelData.waterLevel + "\n";
        m_DebugText.text += "Magic Level: " + levelData.magicLevel + "\n";
        m_DebugText.text += "Altitude: " + levelData.altitude + "\n";
        m_DebugText.text += "Probability for Normal Room: " + levelData.GetNormalRoomPercentage() + "%" + "\n";
        m_DebugText.text += "Probability for Treasure Room: " + levelData.GetTreasureRoomPercentage() + "%" + "\n";
        m_DebugText.text += "Probability for Ambush Room: " + levelData.GetAmbushRoomPercentage() + "%" + "\n";
        m_DebugText.text += "Probability for Safe Room: " + levelData.GetSafeRoomPercentage() + "%" + "\n";
    }
}
