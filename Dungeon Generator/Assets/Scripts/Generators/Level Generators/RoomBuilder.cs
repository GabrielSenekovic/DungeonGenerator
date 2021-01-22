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
        CreateWall(wallObject.GetComponent<MeshFilter>().mesh, new Vector3(1, 1, 1), new Vector2(3,3), 0.05f);
        wallObject.AddComponent<MeshRenderer>();
        wallObject.GetComponent<MeshRenderer>().material = material;
    }
    public Mesh CreateWall(Mesh mesh, Vector3 dim, Vector2 divisions, float jaggedness)
    {
        //dim x = width, y = tilt, z = height
        //divisions = how many vertices per unit tile
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        for(int i = 0; i < dim.z; i++)
        {
            //Go through each square upwards
            for(int j = 0; j < divisions.x * divisions.y; j++)
            {
                //Go through each vertex of the square
                float v_x = (j % divisions.x) / divisions.x;
                float v_z = (j / (int)divisions.x) / divisions.y;
                
                if(v_z * divisions.y > 0 && v_x * divisions.x == 0)
                {
                    newVertices.Add( newVertices[3]);
                    newVertices.Add( newVertices[2]);
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness)) - 1.0f / divisions.x, y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness)) -1 / divisions.y) - i));
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness))                     , y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness)) -1 / divisions.y) - i));
                }
                else if(v_x * divisions.x > 0)
                {
                    
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness))                     , y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness))                 ) - i));
                    newVertices.Add( newVertices[0 + (int)((v_x * divisions.x)-1 ) * 4]);
                    newVertices.Add( newVertices[3 + (int)((v_x * divisions.x)-1 ) * 4]);
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness))                     , y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness)) -1 / divisions.y) - i));

                }
                else
                {
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness))                     , y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness))                 ) - i));
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness)) - 1.0f / divisions.x, y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness))                 ) - i));
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness)) - 1.0f / divisions.x, y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness)) -1 / divisions.y) - i));
                    newVertices.Add( new Vector3 ((x + v_x + Random.Range(-jaggedness, jaggedness))                     , y + Random.Range(-jaggedness, jaggedness) , ((z - v_z - Random.Range(-jaggedness, jaggedness)) -1 / divisions.y) - i));
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
                if(v_z * divisions.y > 1)
                {
                    break;
                }
            }
        }

        mesh.Clear ();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = newUV.ToArray(); // add this line to the code here
        mesh.Optimize ();
        mesh.RecalculateNormals ();

        return mesh;
    }
}
