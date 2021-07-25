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

        public Vector2Int divisions;

        public Vector3 position; //The start position to draw the wall from

        public WallData(Vector3 position_in, int rotation_in, int length_in, int height_in, int tilt_in, Vector2Int divisions_in)
        {
            position = position_in;
            rotation = rotation_in;
            length = length_in;
            height = height_in;
            tilt = tilt_in;
            divisions = divisions_in;
        }
        public WallData(Vector3 position_in, int rotation_in, int height_in, int tilt_in) //If this wall has the same length as the previous one, you don't have to define length
        {
            position = position_in;
            rotation = rotation_in;
            length = 0;
            height = height_in;
            tilt = tilt_in;
            divisions = new Vector2Int(1,1);
        }
    }
    public static void CreateChest(Mesh mesh, int length)
    {
        //Specify how long the chest is. Length determines how long it is from left to right, if the opening side is in front
        //Start off by creating just a cube chest
        //Like in Minecraft
        //Start from the back then forward. Save the points on the top where the lid will rotate around
    }
    public static void CreateCylinder(Mesh mesh, float height)
    {
        //Create a vase from a sinewave controlling a radius
        List<Vector3> positions = new List<Vector3>();
        float angleIncrement = 360.0f / 10.0f;
        float currentHeight = 0;
        float heightIncrement = height / 10.0f;

        for(int i = 0; i < 10; i++)
        {
            //Go through all levels upwards
            float angle = 0;
            float currentRadius = 0.5f;

            for(int j = 0; j < 10; j++)
            {
                //Go around the circle
                positions.Add(new Vector3(currentRadius * Mathf.Sin(angle * Mathf.Deg2Rad), currentRadius * Mathf.Cos(angle* Mathf.Deg2Rad), -currentHeight));
                angle += angleIncrement;
            }
            currentHeight += heightIncrement;
        }
        Debug.Log("Amount of positions: " + positions.Count);

        List<int> newTriangles = new List<int>();
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector2> newUV = new List<Vector2>();

        //Now fill up the vertices
        for(int i = 0; i < positions.Count - 10; i++)
        {
            newVertices.Add(positions[0 + i]);
            newVertices.Add(positions[(1 + i)%10 + 10 * (int)((float)i/10.0f)]);
            newVertices.Add(positions[(11 + i)%10 + 10 * (int)((float)(i+10)/10.0f)]);
            newVertices.Add(positions[10 + i]);

            newUV.Add(new Vector2 (1, 0));                      //1,0
            newUV.Add(new Vector2 (0, 0));                      //0,0
            newUV.Add(new Vector2 (0, 1)); //0,1
            newUV.Add(new Vector2 (1, 1)); //1,1

            int[] indexValue = new int[]{0,1,3,1,2,3};
            for(int index = 0; index < indexValue.Length; index++)
            {
                newTriangles.Add(indexValue[index] + i * 4);
            }
        }

        mesh.Clear ();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = newUV.ToArray(); 
        mesh.Optimize();
        mesh.RecalculateNormals();
    }
    public static void CreateVase(Mesh mesh, float height, AnimationCurve curve)
    {
        //Create a vase from a sinewave controlling a radius
        List<Vector3> positions = new List<Vector3>();
        float angleIncrement = 360.0f / 10.0f;
        float currentHeight = 0;
        float heightIncrement = height / 10.0f;

        for(int j = 0; j < 10; j++)
        {
            //Go through all levels upwards
            float angle = 0;
            //0.5f radius is base radius, cuz it fills up one tile
            float currentRadius = curve.Evaluate((float)j/10.0f);

            for(int k = 0; k < 10; k++)
            {
                //Go around the circle
                positions.Add(new Vector3(currentRadius * Mathf.Sin(angle * Mathf.Deg2Rad), currentRadius * Mathf.Cos(angle* Mathf.Deg2Rad), -currentHeight));
                angle += angleIncrement;
            }
            currentHeight += heightIncrement;
        }

        currentHeight-=heightIncrement;

        for(int j = 9; j >= 0; j--)
        {
            //Go through all levels downwards
            float angle = 0;
            float currentRadius = curve.Evaluate((float)j/10.0f);

            for(int k = 9; k >= 0; k--)
            {
                //Go around the circle
                positions.Add(new Vector3(currentRadius * Mathf.Sin(angle * Mathf.Deg2Rad), currentRadius * Mathf.Cos(angle* Mathf.Deg2Rad), -currentHeight));
                angle += angleIncrement;
            }
            currentHeight -= heightIncrement;
        }
       // Debug.Log(positions.Count);

        List<int> newTriangles = new List<int>();
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector2> newUV = new List<Vector2>();

        for(int j = 0; j < positions.Count -10; j++)
        {
            newVertices.Add(positions[0 + j]);
            newVertices.Add(positions[(1 + j)%10 + 10 * (int)((float)j/10.0f)]);
            newVertices.Add(positions[(11 + j)%10 + 10 * (int)((float)(j+10)/10.0f)]);
            newVertices.Add(positions[10 + j]);

            newUV.Add(new Vector2 (1, 0));                      //1,0
            newUV.Add(new Vector2 (0, 0));                      //0,0
            newUV.Add(new Vector2 (0, 1)); //0,1
            newUV.Add(new Vector2 (1, 1)); //1,1

            int[] indexValue = new int[]{0,1,3,1,2,3};

            for(int index = 0; index < indexValue.Length; index++)
            {
                Debug.Log(indexValue[index] + j * 4);
                newTriangles.Add(indexValue[index] + j * 4);
            }
        }

        //Now fill up the vertices
       /* for(int i = 0; i < 2; i++)
        {
            /*int[] indexValue = new int[]{};
            if(i == 0){indexValue = new int[]{0,1,3,1,2,3};};
            if(i == 1){indexValue = new int[]{3,1,0,3,2,1};};

            int limit = (positions.Count * (i+1) / 2) - 10;
            int start = positions.Count / 2 * (i + 1) - positions.Count / 2;
            //Gives 90 if i is 0, 190 if i is 1
            Debug.Log("Start " + start);
            Debug.Log("Limit " + limit);

            for(int j = start; j < limit; j++)
            {
                newVertices.Add(positions[0 + j]);
                newVertices.Add(positions[(1 + j)%10 + 10 * (int)((float)j/10.0f)]);
                newVertices.Add(positions[(11 + j)%10 + 10 * (int)((float)(j+10)/10.0f)]);
                newVertices.Add(positions[10 + j]);

                newUV.Add(new Vector2 (1, 0));                      //1,0
                newUV.Add(new Vector2 (0, 0));                      //0,0
                newUV.Add(new Vector2 (0, 1)); //0,1
                newUV.Add(new Vector2 (1, 1)); //1,1

                int[] indexValue = new int[]{0,1,3,1,2,3};

                for(int index = 0; index < indexValue.Length; index++)
                {
                    Debug.Log(indexValue[index] + j * 4);
                    newTriangles.Add(indexValue[index] + j * 4);
                }
            }
        }*/

        mesh.Clear ();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = newUV.ToArray(); 
        mesh.Optimize();
        mesh.RecalculateNormals();
    }
    public static void CreateWall(GameObject wall, Mesh mesh, List<WallData> instructions, bool wrap)
    {
        float jaggedness = 0.05f;
        Vector2 divisions = new Vector2(instructions[0].divisions.x +1, instructions[0].divisions.y+1);
        jaggedness = 0;
       // if(divisions.x == 1){jaggedness = 0;}
        //dim x = width, y = tilt, z = height
        //divisions = how many vertices per unit tile
        //instructions tells us how many steps to go before turning, and what direction to turn
        float centering = 0.5f - ((1 / divisions.x) * (divisions.x - 1)); //the mesh starts at 0 and then goes to the left, which essentially makes it start in the middle of a ground tile and stretch outside it. This float will fix that

        List<int> newTriangles = new List<int>();
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector2> newUV = new List<Vector2>();

        int jump_wall = 0; //it needs to accumulate all the walls vertices

        int savedLengthOfWall = instructions[0].length; //If one of the walls have length zero, use this value instead

        for(int i = 0; i < instructions.Count; i++)
        {
            //Go through each wall
            float x = instructions[i].position.x - centering;
            float y = instructions[i].position.y;
            float z = instructions[i].position.z;

            int amount_of_faces = (int)(divisions.x * divisions.y);
            const int vertices_per_quad = 4;
            int vertices_per_tile = amount_of_faces * vertices_per_quad;
            int vertices_per_column = vertices_per_tile * instructions[i].height;

            int lengthOfWall = instructions[i].length > 0 ? instructions[i].length : savedLengthOfWall;
            int previousLengthOfWall = instructions[i].length > 0 && i > 0 ? instructions[i-1].length : savedLengthOfWall; //Get this so you can jump over the previous wall
            savedLengthOfWall = lengthOfWall;

            jump_wall = i > 0? jump_wall + (previousLengthOfWall * vertices_per_column) :0; //This value always grows. It doesnt reset in each loop

            float tilt_increment = instructions[i].tilt / (instructions[i].height * divisions.y);

            for(int j = 0; j < savedLengthOfWall; j++)
            {
                //Debug.Log("New Column! Going this many steps: " + savedLengthOfWall);
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

                        //******************************************************************************************||
                        //*Shorthand for what things mean                                                           ||
                        //* v_x = Vertex x position in decimal                                                      ||
                        //* v_y = Vertex y position in decimal                                                      ||
                        //*                                                                                         ||
                        //* v_x * division.x = Vertex x position not in decimal                                     ||
                        //* j * divisions.x = Start vertex position of each column                                  ||
                        //* v_x * divisions.x == j * divisions.x = Is this vertex at the start of the column?       ||
                        //* v_x * divisions.x > j * divisions.x = Is this vertex not at the start of the column?    ||
                        //* j * divisions.x + division.x - 1 = The last vertex position of each column              ||
                        //*                                                                                         ||
                        //******************************************************************************************||

                        float upperJaggedness = jaggedness;
                        if(l > divisions.x * divisions.y - divisions.x)
                        {
                            upperJaggedness = 0;
                        }
                    
                        if(wrap && i == instructions.Count - 1 && j == savedLengthOfWall - 1 && v_z * divisions.y + k > 0 && Mathf.Round(v_x * divisions.x) == Mathf.Round(j * divisions.x + divisions.x - 1)) //L
                        {
                            //If on the last wall
                            //Then connect to the first wall
                            debug_info += "L";
                            //Connect tiles upwards to tiles downwards
                            newVertices.Add( newVertices[((3 + vertices_per_quad) + skip_up + skip_left)+ jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                            if(divisions.x > 1)
                            {
                                newVertices.Add( newVertices[(3 + skip_up + skip_left )+ jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                                newVertices.Add( newVertices[(((4 * (int)divisions.x) + vertices_per_quad -1) + skip_up + skip_left)+ jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                            }
                            else
                            {
                                newVertices.Add( newVertices[(3 + skip_up + vertices_per_quad * ((int)divisions.x - 1)) + jump_wall + k * vertices_per_tile + (j-1) * vertices_per_column]);
                                newVertices.Add( newVertices[(3 + skip_up + vertices_per_quad * ((int)divisions.x * 2 - 1)) + jump_wall + k * vertices_per_tile + (j-1) * vertices_per_column]);
                            }
                            newVertices.Add( newVertices[2 + skip_up + k * vertices_per_tile + vertices_per_quad * (int)divisions.x]);
                        }
                        else if(wrap && i == instructions.Count - 1 && j == savedLengthOfWall - 1 && Mathf.Round(v_x * divisions.x) == Mathf.Round(j * divisions.x + divisions.x - 1)) //K
                        {
                            //If on the last wall
                            //Then connect to the first wall
                            debug_info += "K";
                            newVertices.Add( newVertices[1]);
                            if(divisions.x > 1)
                            {
                                newVertices.Add( newVertices[(0 + skip_left) + jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                                newVertices.Add( newVertices[(3 + skip_left) + jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                            }
                            else //When divisions.x is 1, then G isnt there to connect to the column before. Instead K has to do it, and it isnt normally equipped to do so.
                            {
                                int jump_column = j > 0 ? (j-1) * vertices_per_column: 0;
                                newVertices.Add( newVertices[(0 + vertices_per_quad * ((int)divisions.x - 1)) + jump_wall + k * vertices_per_tile + jump_column]);
                                newVertices.Add( newVertices[(3 + vertices_per_quad * ((int)divisions.x - 1)) + jump_wall + k * vertices_per_tile + jump_column]);
                            }
                            newVertices.Add( newVertices[2]);
                        }
                        else if(v_x * divisions.x > j * divisions.x && k > 0) //F
                        {
                            debug_info += "F";
                            //Connect tiles upwards to tiles downwards
                            newVertices.Add( newVertices[((3 + vertices_per_quad) + skip_up + skip_left)+ jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                            newVertices.Add( newVertices[(3 + skip_up + skip_left )+ jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                            newVertices.Add( newVertices[(((4 * (int)divisions.x) + vertices_per_quad -1) + skip_up + skip_left)+ jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                            newVertices.Add(  new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, upperJaggedness) , ((z - v_z - UnityEngine.Random.Range(-upperJaggedness, upperJaggedness)) -quad_val_z) - k));
                            CreateWall_Rotate(newVertices, new List<int>{3}, instructions[i].position, instructions[i].rotation);
                        }
                        else if((v_z > 0 || k > 0) && v_x * divisions.x == j * divisions.x && j > 0) //H
                        {
                            debug_info += "H";
                            //Connect the first quad of each row of right columns to the last quad of each row of left columns
                            newVertices.Add( newVertices[((3 + vertices_per_quad) + skip_up + skip_left) + jump_wall + k * vertices_per_tile + j * vertices_per_column]);
                            newVertices.Add( newVertices[(3 + skip_up + vertices_per_quad * ((int)divisions.x - 1)) + jump_wall + k * vertices_per_tile + (j-1) * vertices_per_column]);
                            newVertices.Add( newVertices[(3 + skip_up + vertices_per_quad * ((int)divisions.x * 2 - 1)) + jump_wall + k * vertices_per_tile + (j-1) * vertices_per_column]);
                            newVertices.Add(  new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, upperJaggedness) , ((z - v_z - UnityEngine.Random.Range(-upperJaggedness, upperJaggedness)) -quad_val_z) - k));
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
                            newVertices.Add(  new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, upperJaggedness) , ((z - v_z - UnityEngine.Random.Range(-upperJaggedness, upperJaggedness)) -quad_val_z) - k));
                            CreateWall_Rotate(newVertices, new List<int>{3}, instructions[i].position, instructions[i].rotation);
                        }
                        else if(k > 0) //E
                        {
                            debug_info += "E";
                            //Connect first quad of tile upwards to the tile downwards
                            newVertices.Add( newVertices[((3 + skip_up) + k * vertices_per_tile) + j * vertices_per_column]);
                            newVertices.Add( newVertices[((2 + skip_up) + k * vertices_per_tile) + j * vertices_per_column]);
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness)) - quad_val_x        , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, upperJaggedness) , ((z - v_z - UnityEngine.Random.Range(-upperJaggedness, upperJaggedness)) -quad_val_z) - k));
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, upperJaggedness) , ((z - v_z - UnityEngine.Random.Range(-upperJaggedness, upperJaggedness)) -quad_val_z) - k));
                            CreateWall_Rotate(newVertices, new List<int>{2}, new Vector3(x,y, z), instructions[i].rotation);
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
                            CreateWall_Rotate(newVertices, new List<int>{2}, instructions[i].position, instructions[i].rotation);
                        }
                        else if(v_x * divisions.x > j * divisions.x) //B
                        {
                            debug_info += "B";
                            //Connect quad to the left to quad to the right
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                     , y + UnityEngine.Random.Range(-jaggedness, 0) , ((z - v_z )) - k));
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
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                    , y + UnityEngine.Random.Range(-jaggedness, 0) , ((z - v_z ) ) - k));
                            newVertices.Add( newVertices[(0 + (jump_wall - vertices_per_column) + vertices_per_quad * ((int)divisions.x - 1)) + k * vertices_per_tile]);
                            newVertices.Add( newVertices[(3 + (jump_wall - vertices_per_column) + vertices_per_quad * ((int)divisions.x - 1)) + k * vertices_per_tile]);
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                    , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - k));
                            CreateWall_Rotate(newVertices, new List<int>{}, instructions[i].position, instructions[i].rotation);
                        }
                        else if(j > 0) //G
                        {
                            debug_info += "G";
                            int jump_column = j > 0 ? (j-1) * vertices_per_column: 0;
                            //Connect first quad of column to the right to the last quad of the first row of the first column
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                    , y + UnityEngine.Random.Range(-jaggedness, 0) , ((z - v_z ) ) - k));
                            newVertices.Add( newVertices[(0 + vertices_per_quad * ((int)divisions.x - 1)) + jump_wall + k * vertices_per_tile + jump_column]);
                            newVertices.Add( newVertices[(3 + vertices_per_quad * ((int)divisions.x - 1)) + jump_wall + k * vertices_per_tile + jump_column]);
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                    , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - k));
                            CreateWall_Rotate(newVertices, new List<int>{}, instructions[i].position, instructions[i].rotation);
                        }
                        else //A
                        {
                            debug_info += "A";
                            //Make lone quad
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                    , y + UnityEngine.Random.Range(-jaggedness, 0) , (z - v_z ) - k));
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness)) - quad_val_x         , y  + UnityEngine.Random.Range(-jaggedness, 0), (z - v_z ) - k));
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness)) - quad_val_x         , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - k));
                            newVertices.Add( new Vector3 ((x + v_x + UnityEngine.Random.Range(-jaggedness, jaggedness))                    , y + current_tilt_increment + UnityEngine.Random.Range(-jaggedness, jaggedness) , ((z - v_z - UnityEngine.Random.Range(-jaggedness, jaggedness)) -quad_val_z) - k));
                            CreateWall_Rotate(newVertices, new List<int>{0,1,2,3}, new Vector3(x,y,z), instructions[i].rotation);
                        }
                        
                        int[] indexValue = new int[]{0,1,3,1,2,3};
                        for(int index = 0; index < indexValue.Length; index++){newTriangles.Add(indexValue[index] + jump_quad_up + jump_tile + jump_quad_side + jump_wall);}

                        newUV.Add(new Vector2 (v_x - j + 1.0f / divisions.x, v_z));                      //1,0
                        newUV.Add(new Vector2 (v_x - j                     , v_z));                      //0,0
                        newUV.Add(new Vector2 (v_x - j                     , v_z + 1.0f / divisions.y)); //0,1
                        newUV.Add(new Vector2 (v_x - j + 1.0f / divisions.x, v_z + 1.0f / divisions.y)); //1,1
                        //goto end;
                    }
                    debug_info += "_";
                    //Debug.Log("So far: " + debug_info);
                }
                //Debug.Log(debug_info);
            }
           // BoxCollider box = wall.AddComponent<BoxCollider>();
           // box.size = new Vector3( instructions[i].length,1, instructions[i].height);
            //box.center = new Vector3(box.size.x / 2 - 0.5f,0.5f,-box.size.z / 2);
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
        if(Math.Mod(rotation,360) == 0){return;}
        if(indices.Count <= 0)
        {
            indices = new List<int>{0,3};
        }
        int j = vertices.Count - 4;
       // Debug.Log("<color=magenta>Origin: " + origin + " and rotation: " + rotation + "</color>");
        for(int i = 0; i < indices.Count; i++)
        {
            Vector3 dir = vertices[j + indices[i]] - origin;
            dir = Quaternion.Euler(0,0,rotation) * dir;
           // Debug.Log("Dir: " + dir + " and i: " + i);
            vertices[j + indices[i]] = dir + origin;
        }
    }
    static public void CreateSurface(List<Vector3Int> positions, Mesh mesh)
    {
        //Positions literally mean the position of every single quad on the grid
        List<int> newTriangles = new List<int>();
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector2> newUV = new List<Vector2>();

        for(int i = 0; i < positions.Count; i++)
        {
            newVertices.Add(new Vector3(positions[i].x,     positions[i].y,     -positions[i].z));
            newVertices.Add(new Vector3(positions[i].x + 1, positions[i].y,     -positions[i].z));
            newVertices.Add(new Vector3(positions[i].x + 1, positions[i].y + 1, -positions[i].z));
            newVertices.Add(new Vector3(positions[i].x,     positions[i].y + 1, -positions[i].z));

            newTriangles.Add(3 + 4 * i); //0, 1, 3, 1, 2, 3
            newTriangles.Add(1 + 4 * i);
            newTriangles.Add(0 + 4 * i);
            newTriangles.Add(3 + 4 * i);
            newTriangles.Add(2 + 4 * i);
            newTriangles.Add(1 + 4 * i);

            newUV.Add(new Vector2 (1,0));                      //1,0
            newUV.Add(new Vector2 (0,0));                      //0,0
            newUV.Add(new Vector2 (0,1)); //0,1
            newUV.Add(new Vector2 (1,1)); //1,1
        }

        mesh.Clear ();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = newUV.ToArray(); 
        mesh.Optimize();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
    static public void CreateSurface(Mesh mesh, float height)
    {
        //When you are making a completely flat room without walls
        List<int> newTriangles = new List<int>();
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector2> newUV = new List<Vector2>();

        for(int i = 0; i < 20 * 20; i++) //20 is the standard room size
        {
            newVertices.Add(new Vector3(i % 20, -i / 20 -1, -height));
            newVertices.Add(new Vector3(i % 20 + 1, -i / 20 -1, -height));
            newVertices.Add(new Vector3(i % 20 + 1, -i / 20, -height));
            newVertices.Add(new Vector3(i % 20, -i / 20, -height));

            newTriangles.Add(3 + 4 * i); //0, 1, 3, 1, 2, 3
            newTriangles.Add(1 + 4 * i);
            newTriangles.Add(0 + 4 * i);
            newTriangles.Add(3 + 4 * i);
            newTriangles.Add(2 + 4 * i);
            newTriangles.Add(1 + 4 * i);

            newUV.Add(new Vector2 (1,0));                      //1,0
            newUV.Add(new Vector2 (0,0));                      //0,0
            newUV.Add(new Vector2 (0,1)); //0,1
            newUV.Add(new Vector2 (1,1)); //1,1
        }

        mesh.Clear ();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = newUV.ToArray(); 
        mesh.Optimize();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}