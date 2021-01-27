using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    public List<int> newTriangles = new List<int>();
    public List<Vector3> newVertices = new List<Vector3>();
    public List<Vector2> newUV = new List<Vector2>();
    public Material material;
    void Start()
    {
        //Build wall meshes all around the start area in a 30 x 30 square
        GameObject wallObject = new GameObject("Wall");
        wallObject.transform.parent = this.gameObject.transform;
        wallObject.AddComponent<MeshFilter>();
        CreateWall(wallObject.GetComponent<MeshFilter>().mesh, new Vector3(2, 1, 4), new Vector2(3,3), 0.05f);
        wallObject.AddComponent<MeshRenderer>();
        wallObject.GetComponent<MeshRenderer>().material = material;
    }
    public Mesh CreateWall(Mesh mesh, Vector3 dim, Vector2 divisions, float jaggedness)
    {
        //dim x = width, y = tilt, z = height
        //divisions = how many vertices per unit tile
        float centering = 0.5f - ((1 / divisions.x) * (divisions.x - 1)); //the mesh starts at 0 and then goes to the left, which essentially makes it start in the middle of a ground tile and stretch outside it. This float will fix that

        float x = transform.position.x + centering;
        float y = transform.position.y;
        float z = transform.position.z;

        int amount_of_faces = (int)(divisions.x * divisions.y);
        const int vertices_per_quad = 4;
        int vertices_per_tile = amount_of_faces * vertices_per_quad;

        for(int i = 0; i < dim.z; i++)
        {
            //Go through each square upwards
            for(int j = 0; j < divisions.x * divisions.y; j++)
            {
                //Go through each vertex of the square

                float v_x = (j % divisions.x) / divisions.x;
                float v_z = (j / (int)divisions.x) / divisions.y;
                int skip_left = (int)((v_x * divisions.x)-1 ) * vertices_per_quad;
                int skip_up = (int)((v_z * divisions.y)-1) * vertices_per_quad * (int)divisions.x;
                
                float quad_val_x = 1.0f / divisions.x;
                float quad_val_z = 1.0f / divisions.y;
                
                if(v_x * divisions.x > 0 && i > 0)
                {
                    //Connect tiles upwards to tiles downwards
                    newVertices.Add( newVertices[((3 + vertices_per_quad) + skip_up + skip_left) + i * vertices_per_tile]);
                    newVertices.Add( newVertices[(3 + skip_up + skip_left ) + i * vertices_per_tile]);
                    newVertices.Add( newVertices[(((4 * (int)divisions.x) + vertices_per_quad -1) + skip_up + skip_left) + i * vertices_per_tile]);
                    newVertices.Add(  new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness))                     , y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness)) -quad_val_z) - i));
                }
                else if(i > 0)
                {
                    //Connect first quad of tile upwards to the tile downwards
                    newVertices.Add( newVertices[((3 + skip_up) + i * vertices_per_tile)]);
                    newVertices.Add( newVertices[((2 + skip_up) + i * vertices_per_tile)]);
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness)) - quad_val_x, y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness)) -quad_val_z) - i));
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness))                     , y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness)) -quad_val_z) - i));
                }
                else if(v_z * divisions.y > 0 && v_x * divisions.x > 0)
                {
                    //Connect quad diagonally up to the left to surrounding quads
                    newVertices.Add( newVertices[((3 + vertices_per_quad) + skip_up + skip_left) + i * vertices_per_tile]);
                    newVertices.Add( newVertices[(3 + skip_up + skip_left ) + i * vertices_per_tile]);
                    newVertices.Add( newVertices[(((4 * (int)divisions.x) + vertices_per_quad -1) + skip_up + skip_left) + i * vertices_per_tile]);
                    newVertices.Add(  new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness))                     , y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness)) -quad_val_z) - i));
                }
                else if(v_z * divisions.y > 0 && v_x * divisions.x == 0)
                {
                    //Connect quad upwards to quad downwards
                    newVertices.Add( newVertices[(3 + skip_up) + i * vertices_per_tile]);
                    newVertices.Add( newVertices[(2 + skip_up) + i * vertices_per_tile]);
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness)) - quad_val_x, y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness)) -quad_val_z) - i));
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness))                     , y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness)) -quad_val_z) - i));
                }
                else if(v_x * divisions.x > 0)
                {
                    //Connect quad to the left to quad to the right
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness))                     , y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness))                 ) - i));
                    newVertices.Add( newVertices[(0 + skip_left) + i * vertices_per_tile]);
                    newVertices.Add( newVertices[(3 + skip_left) + i * vertices_per_tile]);
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness))                     , y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness)) -quad_val_z) - i));
                }
                else
                {
                    //Make lone quad
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness))                    , y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness))                 ) - i));
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness)) - quad_val_x         , y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness))                 ) - i));
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness)) - quad_val_x         , y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness)) -quad_val_z) - i));
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness))                    , y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness)) -quad_val_z) - i));
                }

                int jump_quad = 4 * j;
                int jump_tile = (int)(divisions.x * divisions.y) * i * 4;

                newTriangles.Add(0 + jump_quad + jump_tile);
                newTriangles.Add(1 + jump_quad + jump_tile);
                newTriangles.Add(3 + jump_quad + jump_tile);
                newTriangles.Add(1 + jump_quad + jump_tile);
                newTriangles.Add(2 + jump_quad + jump_tile);
                newTriangles.Add(3 + jump_quad + jump_tile);

                newUV.Add(new Vector2 (1, 0));
                newUV.Add(new Vector2 (0 , 0));
                newUV.Add(new Vector2 (0, 1));
                newUV.Add(new Vector2 (1, 1));
                /*if(i > 0 && v_z * divisions.y> 0)
                {
                    break;
                }*/
            }
        }

        mesh.Clear ();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = newUV.ToArray(); 
        mesh.Optimize ();
        mesh.RecalculateNormals ();

        return mesh;
    }
}
