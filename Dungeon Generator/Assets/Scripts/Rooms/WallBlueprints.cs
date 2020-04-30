using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBlueprints : MonoBehaviour
{
    //This script will hold all walls of one variant
    [SerializeField] List<Wall> walls;

    public GameObject wallBlock;

    public Wall GetWall(WallVariant variant)
    {
        return walls[(int)variant - 1];
    }
}
