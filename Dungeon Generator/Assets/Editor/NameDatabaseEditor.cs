using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(NameDatabase))]
public class NameDatabaseEditor:Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        NameDatabase database = (NameDatabase)target;
        if(GUILayout.Button("Sort"))
        {
            database.Sort();
        }
        if(GUILayout.Button("Eliminate Duplicates"))
        {
            database.EliminateDuplicates();
        }
    }
}
