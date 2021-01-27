using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject grass;
    public Vector2 area;
    public int grassPerTile;
    void Start()
    {
        for(int i = 0; i < area.x * area.y; i++)
        {
            int x = (int)(i % area.x);
            int y = (int)(i / area.y);
            //for each tile
            for(int j = 0; j < grassPerTile; j++)
            {
                Instantiate(grass, new Vector2(Random.Range(x, x + 1.0f), Random.Range(y, y+1.0f)), Quaternion.identity, gameObject.transform);
            }
        }
    }

}
