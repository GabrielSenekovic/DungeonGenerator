using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTooltip : MonoBehaviour
{
    static MenuTooltip instance;

    public static MenuTooltip Instance 
    {
        get { return instance; }
    }
    public static MenuTooltip GetInstance() 
    {
        return instance;
    }
    public SpriteText upperText;
    public SpriteText lowerText;
    public GraphemeDatabase graphemeDatabase;

    private void Start() 
    {
        instance = this;
        GetInstance().upperText.Initialize(graphemeDatabase.fonts[0], true);
        GetInstance().lowerText.Initialize(graphemeDatabase.fonts[0], true);
    }

    public static void UpdateUpperTooltip(string text_in)
    {
        GetInstance().upperText.Write(text_in);
    }
    static public void UpdateLowerTooltip(string text_in)
    {
        GetInstance().lowerText.Write(text_in);
    }
}
