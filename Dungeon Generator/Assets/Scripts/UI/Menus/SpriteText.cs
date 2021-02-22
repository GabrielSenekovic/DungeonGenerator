using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteText : MonoBehaviour
{
    GraphemeDatabase.Font font;
    public string text;

    List<GameObject> letters = new List<GameObject>();

    public int row = 0;
    int width = 0;

    public Vector2 offset;
    public int spaceSize;

    public int rowSeparation;
    public int maxWidth;

    bool multipleRows;

    public void Initialize(GraphemeDatabase.Font font_in, bool value)
    {
        font = font_in;
        multipleRows = value;
    }

    public void Write(string text_in)
    {
        text = text_in;
        Write();
    }

    public void Write()
    {
        Reset();
        int i = 0;
        if(text.Length == 0){return;}
        foreach(char c in text)
        {
            if(c != ' ' && (int)c != 10) //If current letter isnt space and isnt new line
            {
                if(c != text[i])
                {
                    Debug.Log(c);
                    Debug.Log(text[i]);
                    throw new System.Exception();
                }
                GameObject temp = new GameObject();
                letters.Add(temp);
                temp.transform.parent = transform;
                temp.AddComponent<Image>();
                Sprite sprite = font.Find(c).sprite;
                temp.GetComponent<Image>().sprite = sprite;
                temp.GetComponent<Image>().SetNativeSize();
                temp.transform.localScale = new Vector3(1,1,1);
                temp.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
                temp.GetComponent<RectTransform>().anchorMin = new Vector2(0,1);
                temp.transform.localPosition = new Vector2(offset.x + width, -row * font.letters[0].sprite.texture.height + offset.y - row * rowSeparation);
                width += (int)sprite.rect.width;
                i++;
                if(c == '.' || c == ',' || c == ':' || c == ';')
                {
                    width += spaceSize;
                }
            }
            else if(c != 10)
            {
                width += spaceSize;
                i++;
                if(i < text.Length && multipleRows && IsNextWordTooLong(ref i))
                {
                    width = 0;
                    row++;
                }
            }
            else
            {
                i++;
                width = 0;
                row++;
            }
        }
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, (row + 1) * font.letters[0].sprite.texture.height - offset.y * 2 + row * rowSeparation);
    }
    public bool IsNextWordTooLong(ref int i)
    {
        char currentLetter = text[i];
        if((int)currentLetter == 10)
        {
            //i++;
            return true;
        }
        int j = 1; //was 1 before
        int wordWidth = 0;
        while(currentLetter != ' ' && (int)currentLetter != 10 && i+j < text.Length)
        {
            wordWidth += font.Find(currentLetter).sprite.texture.width;
            Debug.Log(i + j);
            currentLetter = text[i + j];
            j++;
        }
        return wordWidth + width >= maxWidth;
    }

    void Reset()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(100,100);
        for(int j = letters.Count - 1; j >= 0 ; j-- )
        {
            Destroy(letters[j]);
        }
        letters.Clear();
        width = 0;
        row = 0;
    }
}
