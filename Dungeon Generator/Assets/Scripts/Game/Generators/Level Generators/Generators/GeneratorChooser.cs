using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorChooser : MonoBehaviour
{
    [SerializeField] List<GameObject> generators;
    
    private void Start() 
    {
        GameData.currentLevel = GetComponent<LevelDataGenerator>().Initialize(GameData.m_LevelDataSeed);
        GameData.currentLevel.dungeon = DebuggingTools.isDungeon;
        if(GameData.currentLevel.dungeon)
        {
            Instantiate(generators[1], transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        else
        {
            Instantiate(generators[0], transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
