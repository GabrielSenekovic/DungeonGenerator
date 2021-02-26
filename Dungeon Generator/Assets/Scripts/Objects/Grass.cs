using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
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

    [SerializeField]float burningSpeed;

    public Vector3 grassRotation;
    public LayerMask layerMask;
    List<List<ObjectData>> batches = new List<List<ObjectData>>();

    public VisualEffectAsset VFX_Burning;

    [System.Serializable]public class VFXData
    {
        public GameObject gameObject;
        public bool playing;
    }

    public List<VFXData> VFX = new List<VFXData>();
    List<System.Tuple<List<ObjectData>, Material, VFXData>> burningBatches = new List<System.Tuple<List<ObjectData>, Material, VFXData>>();
    void Start()
    {
        int batchIndexNum = 0;
        List<ObjectData> currentBatch = new List<ObjectData>();
        for(int i = 0; i < area.x * area.y; i++)
        {
            int x = (int)(i % area.x) + (int)transform.position.x;
            int y = (int)(i / area.y) + (int)transform.position.y;
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
        UpdateFire();
    }

    private void FixedUpdate() 
    {
        CheckCollision();
        for(int i = 0; i < VFX.Count; i++)
        {
            if(VFX[i].gameObject.GetComponent<VisualEffect>().aliveParticleCount == 0 && !VFX[i].playing)
            {
                GameObject temp_2 = VFX[i].gameObject;
                VFX.RemoveAt(i);
                Destroy(temp_2);
                i--;
            }
        }
    }

    void UpdateFire()
    {
        for(int i = 0; i < burningBatches.Count(); i++)
        {
            float temp = burningBatches[i].Item2.GetFloat("_Fade");
            if(temp <= 0)
            {
                burningBatches[i].Item3.gameObject.GetComponent<VisualEffect>().Stop();
                burningBatches[i].Item3.playing = false;
                burningBatches.RemoveAt(i); i--;
                break;
            }
            burningBatches[i].Item2.SetFloat("_Fade", temp - burningSpeed);
        }
    }

    void RenderBatches()
    {
        foreach(var b in batches)
        {
            Graphics.DrawMeshInstanced(quad, 0, grassMaterial, b.Select((a) => a.matrix).ToList());
        }
        foreach(var b in burningBatches)
        {
            Graphics.DrawMeshInstanced(quad, 0, b.Item2, b.Item1.Select((a) => a.matrix).ToList());
        }
    }

    private void OnDrawGizmos() 
    {
        for(int i = 0; i < area.x * area.y; i++)
        {
            Vector3 center = new Vector3((int)(i%area.x) + 0.5f + transform.position.x, (int)(i/area.x) + 0.5f + transform.position.y, -0.5f);
            Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);
            Gizmos.DrawCube(center, size);
        }
    }
    void CheckCollision()
    {
        for(int i = 0; i < area.x * area.y; i++)
        {
            Vector3 center = new Vector3((int)(i%area.x) + 0.5f + transform.position.x, (int)(i/area.x) + 0.5f + transform.position.y, -0.5f);
            Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);
            Collider[] hitColliders = Physics.OverlapBox(center, size, Quaternion.identity, layerMask, QueryTriggerInteraction.UseGlobal);
            for(int j = 0; j < hitColliders.Length; j++)
            {
                if(hitColliders[j].gameObject.GetComponent<Fire>())
                {
                    int[] Index = GetBatchIndex(new Vector2((int)i%area.x + transform.position.x, (int)i/area.x + transform.position.y));
                    if(Index != null && Index.Length == 2)
                    {
                        burningBatches.Add(new System.Tuple<List<ObjectData>, Material, VFXData>(new List<ObjectData>(), new Material(grassMaterial), new VFXData()));
                        burningBatches[burningBatches.Count - 1].Item1.AddRange(batches[Index[0]].GetRange(Index[1], grassPerTile));
                        batches[Index[0]].RemoveRange(Index[1],grassPerTile);
                        VFXData temp = burningBatches[burningBatches.Count - 1].Item3;
                        temp.gameObject = new GameObject("VFX");
                        temp.gameObject.transform.parent = transform; 
                        temp.gameObject.transform.position = center;
                        temp.gameObject.AddComponent<VisualEffect>();
                        temp.gameObject.GetComponent<VisualEffect>().visualEffectAsset = VFX_Burning;
                        temp.playing = true;
                        VFX.Add(temp);
                    }
                }
            }
        }
    }

    int[] GetBatchIndex(Vector2 position)
    {
        for(int i = 0; i < batches.Count(); i++)
        {
            for(int j = 0; j < batches[i].Count; j+= grassPerTile)
            {
                if(batches[i][j].pos.x < 0 && Mathf.FloorToInt(batches[i][j].pos.x) == Mathf.FloorToInt(position.x) && Mathf.FloorToInt(batches[i][j].pos.y) == Mathf.FloorToInt(position.y))
                {
                    return new int[]{i,j};
                }
                else if((int)batches[i][j].pos.x >= 0 && (int)batches[i][j].pos.x == (int)position.x && (int)batches[i][j].pos.y == (int)position.y)
                {
                    return new int[]{i,j};
                }
            }
        } 
        return null;     
    }
}
