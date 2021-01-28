using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectData
{
    public Vector3 pos;
    public Vector3 scale;
    public Quaternion rot;
    public Matrix4x4 matrix
    {
        get
        {
            return Matrix4x4.TRS(pos, rot, scale);
        }
    }

   public ObjectData(Vector3 pos_in, Vector3 scale_in, Quaternion rot_in)
   {
       pos = pos_in; scale = scale_in; rot = rot_in;
   }
}
public class Grass : MonoBehaviour
{
    // Start is called before the first frame update
    public int instances;
    public Mesh quad;
    public Material grassMaterial;
    public Vector2 area;
    public int grassPerTile;

    public Vector3 grassRotation;

    List<List<ObjectData>> batches = new List<List<ObjectData>>();
    void Start()
    {
        int batchIndexNum = 0;
        List<ObjectData> currentBatch = new List<ObjectData>();
        for(int i = 0; i < area.x * area.y; i++)
        {
            int x = (int)(i % area.x);
            int y = (int)(i / area.y);
            for(int j = 0; j < grassPerTile; j++)
            {
                AddObject(currentBatch, i, new Vector2(Random.Range(x, x + 1.0f), Random.Range(y, y+1.0f)));
                batchIndexNum++;
                if(batchIndexNum >= 1000)
                {
                    batches.Add(currentBatch);
                    currentBatch = BuildNewBatch();
                    batchIndexNum = 0;
                }
            }
        }
    }
    void AddObject(List<ObjectData> currentBatch, int i, Vector3 position)
    {
        Vector2 textureSize = new Vector2(grassMaterial.mainTexture.width, grassMaterial.mainTexture.height);
        Vector2 scale = new Vector2(1,20);
        scale = textureSize / 16 * scale;
        Quaternion rotation = Quaternion.Euler(grassRotation);
        position.Set(position.x, position.y, -(scale.y / 2));
        currentBatch.Add(new ObjectData(position, new Vector3(scale.x, scale.y, 1), rotation));
    }

    List<ObjectData> BuildNewBatch()
    {
        return new List<ObjectData>();
    }

    private void Update() 
    {
        RenderBatches();
    }

    void RenderBatches()
    {
        foreach(var b in batches)
        {
            Graphics.DrawMeshInstanced(quad, 0, grassMaterial, b.Select((a) => a.matrix).ToList());
        }

    }
}
