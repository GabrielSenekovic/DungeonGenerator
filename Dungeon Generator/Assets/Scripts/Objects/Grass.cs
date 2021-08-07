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
    Mesh mesh; //The grass mesh
    public Material grassMaterial; //The material used for grass
    public Vector2Int area;
    public int grassPerTile;

    public float burningSpeed;
    public Color fireColor;

    public Vector3 grassRotation;
    public LayerMask layerMask;
    List<List<ObjectData>> batches = new List<List<ObjectData>>();

    [System.Serializable]public class GrassTile
    {
        //This object has a reference to the grass straws that are on this tile
        public List<Vector3Int> batchIndices = new List<Vector3Int>(); //The x value is the batch list, and the y value is the start index in that batch list and the z value is how many steps forward
        public bool burning = false;
    }

    public List<GrassTile> tiles = new List<GrassTile>();

    public VisualEffectAsset VFX_Burning; //VFX used for fire

    [System.Serializable]public class VFXData
    {
        public GameObject gameObject;
        public bool playing;
    }
    public List<int> burningGrassIndices = new List<int>();

    public List<VFXData> VFX = new List<VFXData>();
    List<System.Tuple<List<ObjectData>, Material, VFXData>> burningBatches = new List<System.Tuple<List<ObjectData>, Material, VFXData>>();
    
    private void Awake() 
    {
        mesh = new Mesh();
        MeshMaker.CreateTuft(mesh);
    }
    void Start()
    {
    }
    public void PlantFlora(Room.RoomTemplate template)
    {
        int batchIndexNum = 0;
        List<ObjectData> currentBatch = new List<ObjectData>();
        for(int i = 0; i < area.x * area.y; i++) //Go through every tile of the area
        {
            tiles.Add(new GrassTile()); //Create new grass tile
            int x = (int)(i % (float)area.x) + (int)transform.position.x;
            int y = (int)(i / (float)area.y) + (int)transform.position.y;

            if(template.positions[i].identity == 0){continue;}

            for(int j = 0; j < grassPerTile; j++) //Make a set amount of grass for this one tile
            {
                AddObject(currentBatch, i, new Vector2(Random.Range(x, x + 1.0f), Random.Range(y, y+1.0f))); //Adds one singular blade of grass to "currentBatch" 
                batchIndexNum++;
                if(batchIndexNum >= 1000)
                {
                    batches.Add(currentBatch);
                    tiles[tiles.Count - 1].batchIndices.Add(new Vector3Int(batches.Count - 1, currentBatch.Count - 1 - j, j + 1)); //! The x value is the batch list, and the y value is the start index in that batch list and the z value is how many steps forward
                    currentBatch = BuildNewBatch();
                    batchIndexNum = 0;
                }
            }
            if(tiles.Count > 0 && tiles[tiles.Count - 1].batchIndices.Count > 0 && tiles[tiles.Count - 1].batchIndices[0].z < grassPerTile) //If you didn't fill up all the grass there
            {
                tiles[tiles.Count - 1].batchIndices.Add(new Vector3Int(batches.Count, 0, grassPerTile - tiles[tiles.Count - 1].batchIndices[0].z)); 
                //Just Count because it's going into the next batch made. Logically it also starts at 0
            }
            else if(tiles.Count > 0 && tiles[tiles.Count - 1].batchIndices.Count == 0)
            {
                //If you went through the for loop and didn't add blades to the tile
                tiles[tiles.Count - 1].batchIndices.Add(new Vector3Int(batches.Count, currentBatch.Count - grassPerTile, grassPerTile));
            }
        }
        if(batchIndexNum > 0 && batchIndexNum < 1000)
        {
            batches.Add(currentBatch); //Add last batch
        }
    }
    void AddObject(List<ObjectData> currentBatch, int i, Vector3 position)
    {
        Vector2 textureSize = new Vector2(grassMaterial.mainTexture.width, grassMaterial.mainTexture.height);
        position.Set(position.x, position.y, 0);
        currentBatch.Add(new ObjectData(position, new Vector3(1, 1, 1), Quaternion.identity));
    }

    List<ObjectData> BuildNewBatch()
    {
        return new List<ObjectData>();
    }

    private void Update() 
    {
       // RenderBatches();
        UpdateFire();
    }
    private void OnRenderObject() 
    {
        RenderBatches();
    }

    private void FixedUpdate() 
    {
        CheckCollision();
        SpreadFire();
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
            Graphics.DrawMeshInstanced(mesh, 0, grassMaterial, b.Select((a) => a.matrix).ToList());
        }
        foreach(var b in burningBatches)
        {
            Graphics.DrawMeshInstanced(mesh, 0, b.Item2, b.Item1.Select((a) => a.matrix).ToList());
        }
    }

    private void OnDrawGizmos() 
    {
        for(int i = 0; i < area.x * area.y; i++)
        {
            Vector3 center = new Vector3((int)(i%(float)area.x) + 0.5f + transform.position.x, (int)(i/(float)area.x) + 0.5f + transform.position.y, -0.5f);
            Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);
            Gizmos.DrawCube(center, size);
        }
    }
    void CheckCollision()
    {
        for(int i = 0; i < area.x * area.y; i++)
        {
            if(tiles[i].burning){continue;}
            Vector3 center = new Vector3((int)(i%(float)area.x) + 0.5f + transform.position.x, (int)(i/(float)area.x) + 0.5f + transform.position.y, -0.5f);
            Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);
            Collider[] hitColliders = Physics.OverlapBox(center, size, Quaternion.identity, layerMask, QueryTriggerInteraction.UseGlobal);
            
            for(int j = 0; j < hitColliders.Length; j++)
            {
                if(hitColliders[j].gameObject.GetComponent<Fire>()) //If something that is on fire has hit this collider
                {
                    SetTileOnFire(i, center);
                   // return;
                }
            }
        }
    }
    void SpreadFire()
    {
        for(int i = 0; i < burningGrassIndices.Count; i++)
        {
            if(tiles[burningGrassIndices[i]].burning)
            {
                int[] constraints = Math.GetValidConstraints(burningGrassIndices[i], 1, area);
                for(int x = constraints[0]; x < constraints[2]; x++)
                {
                    for(int y = constraints[1]; y < constraints[3]; y++)
                    {
                        if(x + area.x * y != burningGrassIndices[i]) //If were not in the middle
                        {
                            //TODO Use flammability of the material otherwise
                            if(Random.Range(0, 1000) < 2 && !tiles[x + area.x * y].burning)
                            {
                                //Set this tile on fire
                                Vector3 center = new Vector3((int)((x + area.x * y)%(float)area.x) + 0.5f + transform.position.x, (int)((x + area.x * y)/(float)area.x) + 0.5f + transform.position.y, -0.5f);
                                SetTileOnFire(x + area.x * y, center);
                            }
                        }
                    }
                }
            }
        }
    }

    void SetTileOnFire(int i, Vector3 center)
    {
        //hitColliders[j].gameObject.SetActive(false);
        List<Vector3Int> indices = tiles[i].batchIndices;
        tiles[i].burning = true;
        burningGrassIndices.Add(i);
        Debug.Log("Hit");
        
        //int[] Index = GetBatchIndex(new Vector2((int)i%area.x + transform.position.x, (int)i/area.x + transform.position.y)); //Find the index of the batch that just got hit 

        for(int k = 0; k < indices.Count; k++)
        {
            burningBatches.Add(new System.Tuple<List<ObjectData>, Material, VFXData>(new List<ObjectData>(), new Material(grassMaterial), new VFXData()));
            burningBatches[burningBatches.Count - 1].Item1.AddRange(batches[indices[k].x].GetRange(indices[k].y, indices[k].z)); //Add the batch you just hit to the list of burning batches
            
            batches[indices[k].x].RemoveRange(indices[k].y,indices[k].z); 
            //! if removing, then every single index has to be changed in all the tiles whose indices are in index[k].x
            //! I guess thats the fall I have to take since I can't figure out an alternative
            for(int l = i; l < tiles.Count; l++)
            {
                //Start from i, since it was the grass of tile i that was removed. Then work your way up and left shift all the indices by indices[k].z
                if(tiles[l].batchIndices[0].x > indices[k].x) //! if the first indices of this tile are from a batch higher up than the one with removed grass, then you can exit this for loop
                {
                    break;
                }

                for(int m = 0; m < tiles[l].batchIndices.Count; m++)
                {
                    if(tiles[l].batchIndices[m].x == indices[k].x) //! If the indices in this tile has indices on the same batch number as the one where indices were removed, move them further down
                    {
                        tiles[l].batchIndices[m] = new Vector3Int(tiles[l].batchIndices[m].x, tiles[l].batchIndices[m].y - indices[k].z, tiles[l].batchIndices[m].z);
                    }
                }
            }
        }
        
        VFXData temp = burningBatches[burningBatches.Count - 1].Item3;
        temp.gameObject = new GameObject("VFX"); //Create VFX for fire
        temp.gameObject.transform.parent = transform; 
        temp.gameObject.transform.position = center;
        temp.gameObject.AddComponent<VisualEffect>();
        temp.gameObject.GetComponent<VisualEffect>().visualEffectAsset = VFX_Burning;
        temp.playing = true;
        VFX.Add(temp);
    }
}
