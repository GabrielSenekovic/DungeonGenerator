using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [System.Serializable]public struct SurnameData
    {
        public string name;
    }

    public List<NameData> names;
    public List<SurnameData> surnames;

    public void Sort()
    {
        names.Sort((x, y) => x.name.CompareTo(y.name));  
        surnames.Sort((x, y) => x.name.CompareTo(y.name));  
    }
    
    public void EliminateDuplicates()
    {
        for(int i = 0; i < names.Count; i++)
        {
            for(int j = 0; j < names.Count; j++)
            {
                if(names[i].name == names[j].name && i != j)
                {
                    Debug.LogError("<color=red>Error: Name Database eliminated:</color> " + names[j].name);
                    names.RemoveAt(j); j--;
                }
            }
        }
        for(int i = 0; i < surnames.Count; i++)
        {
            for(int j = 0; j < surnames.Count; j++)
            {
                if(surnames[i].name == surnames[j].name && i != j)
                {
                    Debug.LogError("<color=red>Error: Name Database eliminated:</color> " + surnames[j].name);
                    surnames.RemoveAt(j); j--;
                }
            }
        }
    }
    public string GetRandomName()
    {
        return names[Random.Range(0, names.Count)].name + " " + surnames[Random.Range(0, names.Count)].name;
    }
}
