using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NameDatabase : MonoBehaviour
{
    public enum Gender
    {
        MASC,
        FEM,
        UNI
    }
    [System.Serializable]public struct NameData
    {
        public string name;
        public Gender gender;
    }

    public List<NameData> names;

    private void Update()
    {
        Sort();
        EliminateDuplicates();
    }
    private void Sort()
    {
        names.Sort((x, y) => x.name.CompareTo(y.name));  
    }
    
    void EliminateDuplicates()
    {
        for(int i = 0; i < names.Count; i++)
        {
            for(int j = 0; j < names.Count; j++)
            {
                if(names[i].name == names[j].name && i != j)
                {
                    Debug.LogError("<color=red>Error: Name Database already contains:</color> " + names[j].name);
                    names.RemoveAt(j); j--;
                }
            }
        }
    }
}
