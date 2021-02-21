using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(GraphemeDatabase))]
public class GraphemeDatabaseEditor:Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GraphemeDatabase database = (GraphemeDatabase)target;
        if(GUILayout.Button("Sort"))
        {
            database.Sort();
        }
    }
}
