﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMaker : MonoBehaviour
{
    public struct WallData
    {
        public int length; //How many walls will I make in this direction
        public int tilt; //How inclined the wall is into the tile
        public int height; //How tall are the walls
        public int rotation; //Determines what direction the walls are drawn in. Sides, up, down, diagonal etc

        public Vector3 position; //The start position to draw the wall from

        public WallData(Vector3 position_in, int rotation_in, int length_in, int height_in, int tilt_in)
        {
            position = position_in;
            rotation = rotation_in;
            length = length_in;
            height = height_in;
            tilt = tilt_in;
        }
    }
    public static void CreateChest(Mesh mesh)
    {

    }
    public static void CreateVase(Mesh mesh)
    {
        
    }
    public static void CreateWall(GameObject wall, Mesh mesh, Transform transform /*, List<List<int>> instructions*/, Vector3 dim, Vector2 divisions, float jaggedness)
    {
        //dim x = width, y = tilt, z = height
        //divisions = how many vertices per unit tile
        //instructions tells us how many steps to go before turning, and what direction to turn
        float centering = 0.5f - ((1 / divisions.x) * (divisions.x - 1)); //the mesh starts at 0 and then goes to the left, which essentially makes it start in the middle of a ground tile and stretch outside it. This float will fix that

        List<int> newTriangles = new List<int>();
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector2> newUV = new List<Vector2>();

        float x = transform.position.x + centering;
        float y = transform.position.y;
        float z = transform.position.z;

        int amount_of_faces = (int)(divisions.x * divisions.y);
        const int vertices_per_quad = 4;
        int vertices_per_tile = amount_of_faces * vertices_per_quad;
        int vertices_per_column = vertices_per_tile * (int)dim.z;

        float tilt_increment = dim.y / (dim.z * divisions.y);

        for(int i = 0; i < dim.x; i++)
        {
            Debug.Log("New Column!");
            string debug_info = "";
            //Go through each column of wall
            for(int j = 0; j < dim.z; j++)
            {
                //Go through each square upwards
                for(int k = 0; k < divisions.x * divisions.y; k++)
                {
                    //Go through each vertex of the square

                    float v_x = ((k % divisions.x) / divisions.x) + i;
                    float v_z = (k / (int)divisions.x) / divisions.y;
                    int skip_left = (int)(Mathf.RoundToInt(((v_x - i) * divisions.x)-1 )) * vertices_per_quad;
                    //I have to RoundToInt here because for some reason, on the second wall when doing D, it gave me 2 - 1 = 0.9999999
                    //Epsilons suck
                    int skip_up = (int)(Mathf.RoundToInt((v_z * divisions.y)-1)) * vertices_per_quad * (int)divisions.x;

                    int steps_up = j * (int)divisions.y + (int)(v_z * divisions.y); //counts the total row you are on
                    float current_tilt_increment = tilt_increment + tilt_increment * steps_up;
                    
                    float quad_val_x = 1.0f / divisions.x;
                    float quad_val_z = 1.0f / divisions.y;

                    //The 4 is because there's 4 vertices per quad
                    int jump_quad_up = vertices_per_quad * k;
                    int jump_tile = amount_of_faces * j * vertices_per_quad;
                    int jump_quad_side = amount_of_faces * (int)dim.z * i * vertices_per_quad;
                
                    if(v_x * divisions.x > i * divisions.x && j > 0) //F
                    {
                        debug_info += "F";
                        //Connect tiles upwards to tiles downwards
                        newVertices.Add( newVertices[((3 + vertices_per_quad) + skip_up + skip_left) + j * vertices_per_tile + i * vertices_per_column]);
                        newVertices.Add( newVertices[(3 + skip_up + skip_left ) + j * vertices_per_tile + i * vertices_per_column]);
                        newVertices.Add( newVertices[(((4 * (int)divisions.x) + vertices_per_quad -1) + skip_up + skip_left) + j * vertices_per_tile + i * vertices_per_column]);
                        newVertices.Add(  new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - j));
                    }
                    else if((v_z > 0 || j > 0) && v_x * divisions.x == i * divisions.x && i > 0) //H
                    {
                        debug_info += "H";
                        //Connect the first quad of each row of right columns to the last quad of each row of left columns
                        newVertices.Add( newVertices[((3 + vertices_per_quad) + skip_up + skip_left) + j * vertices_per_tile + i * vertices_per_column]);
                        newVertices.Add( newVertices[(3 + skip_up + vertices_per_quad * ((int)divisions.x - 1)) + j * vertices_per_tile + (i-1) * vertices_per_column]);
                        newVertices.Add( newVertices[(3 + skip_up + vertices_per_quad * ((int)divisions.x * 2 - 1)) + j * vertices_per_tile + (i-1) * vertices_per_column]);
                        newVertices.Add(  new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - j));
                    }
                    else if(j > 0) //E
                    {
                        debug_info += "E";
                        //Connect first quad of tile upwards to the tile downwards
                        newVertices.Add( newVertices[((3 + skip_up) + j * vertices_per_tile) + i * vertices_per_column]);
                        newVertices.Add( newVertices[((2 + skip_up) + j * vertices_per_tile) + i * vertices_per_column]);
                        newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness)) - quad_val_x        , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - j));
                        newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - j));
                    }
                    else if(v_z * divisions.y > 0 && v_x * divisions.x > i * divisions.x) //D
                    {
                        debug_info += "D";
                        //Connect quad diagonally up to the left to surrounding quads
                        newVertices.Add( newVertices[((3 + vertices_per_quad) + skip_up + skip_left) + j * vertices_per_tile + i * vertices_per_column]);
                        newVertices.Add( newVertices[(3 + skip_up + skip_left ) + j * vertices_per_tile + i * vertices_per_column]);
                        newVertices.Add( newVertices[(((4 * (int)divisions.x) + vertices_per_quad -1) + skip_up + skip_left) + j * vertices_per_tile + i * vertices_per_column]);
                        newVertices.Add(  new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - j));
                    }
                    else if(v_z * divisions.y > 0 && v_x * divisions.x == i * divisions.x) //C
                    {
                        debug_info += "C";
                        //Connect quad upwards to quad downwards
                        newVertices.Add( newVertices[(3 + skip_up) + j * vertices_per_tile + i * vertices_per_column]);
                        newVertices.Add( newVertices[(2 + skip_up) + j * vertices_per_tile + i * vertices_per_column]);
                        newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness)) - quad_val_x        , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - j));
                        newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - j));
                    }
                    else if(v_x * divisions.x > i * divisions.x) //B
                    {
                        debug_info += "B";
                        //Connect quad to the left to quad to the right
                        newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness))                 ) - j));
                        newVertices.Add( newVertices[(0 + skip_left) + j * vertices_per_tile + i * vertices_per_column]);
                        newVertices.Add( newVertices[(3 + skip_left) + j * vertices_per_tile + i * vertices_per_column]);
                        newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - j));
                    }
                    else if(i > 0) //G
                    {
                        debug_info += "G";
                        //Connect first quad of column to the right to the last quad of the first row of the first column
                        newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                    , y + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness))                 ) - j));
                        newVertices.Add( newVertices[(0 + vertices_per_quad * ((int)divisions.x - 1)) + j * vertices_per_tile + (i-1) * vertices_per_column]);
                        newVertices.Add( newVertices[(3 + vertices_per_quad * ((int)divisions.x - 1)) + j * vertices_per_tile + (i-1) * vertices_per_column]);
                        newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                    , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - j));
                    }
                    else //A
                    {
                        debug_info += "A";
                        //Make lone quad
                        newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                    , y + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness))                 ) - j));
                        newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness)) - quad_val_x         , y + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness))                 ) - j));
                        newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness)) - quad_val_x         , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - j));
                        newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                    , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - j));
                    }

                    newTriangles.Add(0 + jump_quad_up + jump_tile + jump_quad_side);
                    newTriangles.Add(1 + jump_quad_up + jump_tile + jump_quad_side);
                    newTriangles.Add(3 + jump_quad_up + jump_tile + jump_quad_side);
                    newTriangles.Add(1 + jump_quad_up + jump_tile + jump_quad_side);
                    newTriangles.Add(2 + jump_quad_up + jump_tile + jump_quad_side);
                    newTriangles.Add(3 + jump_quad_up + jump_tile + jump_quad_side);

                    newUV.Add(new Vector2 (1, 0));
                    newUV.Add(new Vector2 (0 , 0));
                    newUV.Add(new Vector2 (0, 1));
                    newUV.Add(new Vector2 (1, 1));
                }
                debug_info += "_";
            }
            Debug.Log(debug_info);
        }
        BoxCollider box = wall.AddComponent<BoxCollider>();
        box.size = new Vector3( dim.x,1,dim.z);
        box.center = new Vector3(box.size.x / 2 - 0.5f,0.5f,-box.size.z / 2);

        mesh.Clear ();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = newUV.ToArray(); 
        mesh.Optimize();
        mesh.RecalculateNormals();
    }
    public static void CreateWall(GameObject wall, Mesh mesh, List<WallData> instructions, Vector2 divisions, float jaggedness)
    {
        //dim x = width, y = tilt, z = height
        //divisions = how many vertices per unit tile
        //instructions tells us how many steps to go before turning, and what direction to turn
        float centering = 0.5f - ((1 / divisions.x) * (divisions.x - 1)); //the mesh starts at 0 and then goes to the left, which essentially makes it start in the middle of a ground tile and stretch outside it. This float will fix that

        List<int> newTriangles = new List<int>();
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector2> newUV = new List<Vector2>();

        int jump_wall = 0; //it needs to accumulate all the walls vertices

        for(int i = 0; i < instructions.Count; i++)
        {
            float x = instructions[i].position.x + centering;
            float y = instructions[i].position.y;
            float z = instructions[i].position.z;

            int amount_of_faces = (int)(divisions.x * divisions.y);
            const int vertices_per_quad = 4;
            int vertices_per_tile = amount_of_faces * vertices_per_quad;
            int vertices_per_column = vertices_per_tile * instructions[i].height;

            jump_wall = i > 0? jump_wall + (instructions[i-1].length * vertices_per_column) :0;

            float tilt_increment = instructions[i].tilt / (instructions[i].height * divisions.y);

            for(int j = 0; j < instructions[i].length; j++)
            {
                Debug.Log("New Column!");
                string debug_info = j+ ": ";
                //Go through each column of wall
                for(int k = 0; k < instructions[i].height; k++)
                {
                    //Go through each square upwards
                    for(int l = 0; l < divisions.x * divisions.y; l++)
                    {
                        //Go through each vertex of the square

                        float v_x = ((l % divisions.x) / divisions.x) + j;
                        float v_z = (l / (int)divisions.x) / divisions.y;
                        int skip_left = (int)(Mathf.RoundToInt(((v_x - j) * divisions.x)-1 )) * vertices_per_quad;
                        //I have to RoundToInt here because for some reason, on the second wall when doing D, it gave me 2 - 1 = 0.9999999
                        //Epsilons suck
                        int skip_up = (int)(Mathf.RoundToInt((v_z * divisions.y)-1)) * vertices_per_quad * (int)divisions.x;

                        int steps_up = k * (int)divisions.y + (int)(v_z * divisions.y); //counts the total row you are on
                        float current_tilt_increment = tilt_increment + tilt_increment * steps_up;
                        
                        float quad_val_x = 1.0f / divisions.x;
                        float quad_val_z = 1.0f / divisions.y;

                        //The 4 is because there's 4 vertices per quad
                        int jump_quad_up = vertices_per_quad * l;
                        int jump_tile = amount_of_faces * k * vertices_per_quad;
                        int jump_quad_side = amount_of_faces * (int)instructions[i].height * j * vertices_per_quad;
                    
                        if(v_x * divisions.x > j * divisions.x && k > 0) //F
                        {
                            debug_info += "F";
                            //Connect tiles upwards to tiles downwards
                            newVertices.Add( newVertices[((3 + vertices_per_quad) + skip_up + skip_left)+ jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                            newVertices.Add( newVertices[(3 + skip_up + skip_left )+ jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                            newVertices.Add( newVertices[(((4 * (int)divisions.x) + vertices_per_quad -1) + skip_up + skip_left)+ jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                            newVertices.Add(  new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - k));
                            CreateWall_Rotate(newVertices, new List<int>{3}, instructions[i].position, instructions[i].rotation);
                        }
                        else if((v_z > 0 || k > 0) && v_x * divisions.x == j * divisions.x && j > 0) //H
                        {
                            debug_info += "H";
                            //Connect the first quad of each row of right columns to the last quad of each row of left columns
                            newVertices.Add( newVertices[((3 + vertices_per_quad) + skip_up + skip_left) + jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                            newVertices.Add( newVertices[(3 + skip_up + vertices_per_quad * ((int)divisions.x - 1)) + jump_wall + k * vertices_per_tile + (j-1) * vertices_per_column]);
                            newVertices.Add( newVertices[(3 + skip_up + vertices_per_quad * ((int)divisions.x * 2 - 1)) + jump_wall + k * vertices_per_tile + (j-1) * vertices_per_column]);
                            newVertices.Add(  new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - k));
                            CreateWall_Rotate(newVertices, new List<int>{3}, instructions[i].position, instructions[i].rotation);
                        }
                        else if((v_z > 0 || k > 0) && v_x * divisions.x == j * divisions.x && i > 0) //H
                        {
                            debug_info += "J";
                            int jump_column = j > 0 ? (j-1) * vertices_per_column: 0;
                            //Connect the first quad of each row of right columns to the last quad of each row of previous wall
                            newVertices.Add( newVertices[((3 + jump_wall + skip_up + vertices_per_quad)+ skip_left) + k * vertices_per_tile+ j * vertices_per_column + jump_column]);
                            newVertices.Add( newVertices[(3 + (jump_wall- vertices_per_column) + skip_up + vertices_per_quad * ((int)divisions.x - 1)) + k * vertices_per_tile + jump_column]);
                            newVertices.Add( newVertices[(3 + (jump_wall - vertices_per_column) + skip_up + vertices_per_quad * ((int)divisions.x * 2 - 1)) + k * vertices_per_tile + jump_column]);
                            newVertices.Add(  new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - k));
                            CreateWall_Rotate(newVertices, new List<int>{3}, instructions[i].position, instructions[i].rotation);
                        }
                        else if(k > 0) //E
                        {
                            debug_info += "E";
                            //Connect first quad of tile upwards to the tile downwards
                            newVertices.Add( newVertices[((3 + skip_up) + k * vertices_per_tile) + j * vertices_per_column]);
                            newVertices.Add( newVertices[((2 + skip_up) + k * vertices_per_tile) + j * vertices_per_column]);
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness)) - quad_val_x        , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - k));
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - k));
                            CreateWall_Rotate(newVertices, new List<int>{}, instructions[i].position, instructions[i].rotation);
                        }
                        else if(v_z * divisions.y > 0 && v_x * divisions.x > j * divisions.x) //D
                        {
                            debug_info += "D";
                            //Connect quad diagonally up to the left to surrounding quads
                            newVertices.Add( newVertices[((3 + vertices_per_quad) + skip_up + skip_left) + jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                            newVertices.Add( newVertices[(3 + skip_up + skip_left )  + jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                            newVertices.Add( newVertices[(((4 * (int)divisions.x) + vertices_per_quad -1) + skip_up + skip_left) + jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                            newVertices.Add(  new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - k));
                            CreateWall_Rotate(newVertices, new List<int>{3}, instructions[i].position, instructions[i].rotation);
                        }
                        else if(v_z * divisions.y > 0 && v_x * divisions.x == j * divisions.x) //C
                        {
                            debug_info += "C";
                            //Connect quad upwards to quad downwards
                            newVertices.Add( newVertices[(3 + skip_up) + k * vertices_per_tile + j * vertices_per_column]);
                            newVertices.Add( newVertices[(2 + skip_up) + k * vertices_per_tile + j * vertices_per_column]);
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness)) - quad_val_x        , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - k));
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - k));
                            CreateWall_Rotate(newVertices, new List<int>{}, instructions[i].position, instructions[i].rotation);
                        }
                        else if(v_x * divisions.x > j * divisions.x) //B
                        {
                            debug_info += "B";
                            //Connect quad to the left to quad to the right
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness))                 ) - k));
                            newVertices.Add( newVertices[(0 + skip_left) + jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                            newVertices.Add( newVertices[(3 + skip_left) + jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - k));
                            CreateWall_Rotate(newVertices, new List<int>{}, instructions[i].position, instructions[i].rotation);
                        }
                        else if(i > 0 && j == 0) //I
                        {
                            debug_info += "I";
                            int jump_column = j > 0 ? (j-1) * vertices_per_column: 0;
                            //Connect the first quad of the second wall to the last quad of the first row of the first wall
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                    , y + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness))                 ) - k));
                            newVertices.Add( newVertices[(0 + (jump_wall - vertices_per_column) + vertices_per_quad * ((int)divisions.x - 1)) + k * vertices_per_tile]);
                            newVertices.Add( newVertices[(3 + (jump_wall - vertices_per_column) + vertices_per_quad * ((int)divisions.x - 1)) + k * vertices_per_tile]);
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                    , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - k));
                            Debug.Log("I");
                            CreateWall_Rotate(newVertices, new List<int>{}, instructions[i].position, instructions[i].rotation);
                        }
                        else if(j > 0) //G
                        {
                            debug_info += "G";
                            int jump_column = j > 0 ? (j-1) * vertices_per_column: 0;
                            //Connect first quad of column to the right to the last quad of the first row of the first column
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                    , y + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness))                 ) - k));
                            newVertices.Add( newVertices[(0 + vertices_per_quad * ((int)divisions.x - 1)) + jump_wall + k * vertices_per_tile + jump_column]);
                            newVertices.Add( newVertices[(3 + vertices_per_quad * ((int)divisions.x - 1)) + jump_wall + k * vertices_per_tile + jump_column]);
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                    , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - k));
                            CreateWall_Rotate(newVertices, new List<int>{}, instructions[i].position, instructions[i].rotation);
                        }
                        else //A
                        {
                            debug_info += "A";
                            //Make lone quad
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                    , y + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness))                 ) - k));
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness)) - quad_val_x         , y + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness))                 ) - k));
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness)) - quad_val_x         , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - k));
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                    , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - k));
                            CreateWall_Rotate(newVertices, new List<int>{}, instructions[i].position, instructions[i].rotation);
                        }

                        newTriangles.Add(0 + jump_quad_up + jump_tile + jump_quad_side + jump_wall);
                        newTriangles.Add(1 + jump_quad_up + jump_tile + jump_quad_side + jump_wall);
                        newTriangles.Add(3 + jump_quad_up + jump_tile + jump_quad_side + jump_wall);
                        newTriangles.Add(1 + jump_quad_up + jump_tile + jump_quad_side + jump_wall);
                        newTriangles.Add(2 + jump_quad_up + jump_tile + jump_quad_side + jump_wall);
                        newTriangles.Add(3 + jump_quad_up + jump_tile + jump_quad_side + jump_wall);

                        newUV.Add(new Vector2 (1, 0));
                        newUV.Add(new Vector2 (0 , 0));
                        newUV.Add(new Vector2 (0, 1));
                        newUV.Add(new Vector2 (1, 1));
                    }
                    debug_info += "_";
                }
                Debug.Log(debug_info);
            }
            BoxCollider box = wall.AddComponent<BoxCollider>();
            box.size = new Vector3( instructions[i].length,1, instructions[i].height);
            box.center = new Vector3(box.size.x / 2 - 0.5f,0.5f,-box.size.z / 2);
        }

        mesh.Clear ();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = newUV.ToArray(); 
        mesh.Optimize();
        mesh.RecalculateNormals();
    }
    static public void CreateWall_Rotate(List<Vector3> vertices, List<int> indices, Vector3 origin, int rotation)
    {
        //Rotates the indices and vertices just made
        //Without this function, the wall would only be able to span infinitely in the direction they were first made
        //Thanks to this function, you can have corners, and also close a wall into a room
        if(rotation % 360 == 0){return;}
        if(indices.Count <= 0)
        {
            indices = new List<int>{0,3};
        }
        int j = vertices.Count - 4;
        for(int i = 0; i < indices.Count; i++)
        {
            Vector3 dir = vertices[j + indices[i]] - origin;
            dir = Quaternion.Euler(0,0,rotation) * dir;
            vertices[j + indices[i]] = dir + origin;
            Debug.Log(vertices[j + indices[i]]);
        }
    }
}