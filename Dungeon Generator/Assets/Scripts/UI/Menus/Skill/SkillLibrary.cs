using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkillLibrary : MonoBehaviour
{
    public List<AttackIdentifier> attacks;

    private void Update()
    {
        Sort();
        EliminateDuplicates();
    }
    private void Sort()
    {
        attacks.Sort((x, y) => x.gameObject.name.CompareTo(y.gameObject.name));  
    }
    void EliminateDuplicates()
    {
        for(int i = 0; i < attacks.Count; i++)
        {
            for(int j = 0; j < attacks.Count; j++)
            {
                if(attacks[i].name == attacks[j].name && i != j)
                {
                    Debug.LogError("<color=red>Error: Skill Library already contains:</color> " + attacks[j].name);
                    attacks.RemoveAt(j); j--;
                }
            }
        }
    }
}
