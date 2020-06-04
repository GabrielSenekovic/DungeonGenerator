using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    [SerializeField] LevelData m_LevelData;
    Text m_DebugText;

    public void Awake()
    {
        m_DebugText = GetComponentInChildren<Text>();
        m_DebugText.text += "Level Construction Seed: " + GameData.GetConstructionSeed() + "\n";
        m_DebugText.text += "Level Data Seed: " + GameData.GetDataSeed() + "\n";
        m_DebugText.text += "Level Type: " + m_LevelData.m_location + "\n";
        m_DebugText.text += "Biome: " + m_LevelData.m_biome + "\n";
        m_DebugText.text += "Mood 1: " + m_LevelData.GetMood(0) + "\n";
        m_DebugText.text += "Mood 2: " + m_LevelData.GetMood(1) + "\n";
        m_DebugText.text += "Danger Levels: " + m_LevelData.m_dangerLevel + "\n";
        m_DebugText.text += "Temperature: " + m_LevelData.m_temperatureLevel + "\n";
        m_DebugText.text += "Water Level: " + m_LevelData.m_waterLevel + "\n";
        m_DebugText.text += "Magic Level: " + m_LevelData.m_magicLevel + "\n";
        m_DebugText.text += "Altitude: " + m_LevelData.m_altitude + "\n";
        m_DebugText.text += "Probability for Normal Room: " + m_LevelData.GetNormalRoomPercentage() + "%" + "\n";
        m_DebugText.text += "Probability for Treasure Room: " + m_LevelData.GetTreasureRoomPercentage() + "%" + "\n";
        m_DebugText.text += "Probability for Ambush Room: " + m_LevelData.GetAmbushRoomPercentage() + "%" + "\n";
        m_DebugText.text += "Probability for Safe Room: " + m_LevelData.GetSafeRoomPercentage() + "%" + "\n";
    }
}
