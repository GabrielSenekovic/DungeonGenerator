using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class GraphemeDatabase : MonoBehaviour
{
    [System.Serializable]public struct Grapheme
    {
        public Sprite sprite;
        public char value;
    }
    [System.Serializable] public struct Font
    {
        public List<Grapheme> letters;
        public List<Grapheme> numbers;
        public List<Grapheme> orthography;

        public Grapheme Find(char c)
        {
            c = c.ToString().ToUpper()[0];
            for(int i = 0; i < letters.Count;i++)
            {
                if(letters[i].value == c)
                {
                    return letters[i];
                }
            }
            for(int i = 0; i < numbers.Count;i++)
            {
                if(numbers[i].value == c)
                {
                    return numbers[i];
                }
            }
            for(int i = 0; i < orthography.Count;i++)
            {
                if(orthography[i].value == c)
                {
                    return orthography[i];
                }
            }
            
            Debug.Log((int)c);
            Debug.Log(c);
            throw new System.Exception();
        }
    }
    public List<Font> fonts;

    private void Update()
    {
        Sort();
    }
    public void Sort()
    {
        fonts[0].letters.Sort((x, y) => x.value.CompareTo(y.value));  
        fonts[0].numbers.Sort((x, y) => x.value.CompareTo(y.value));  
    }
}
