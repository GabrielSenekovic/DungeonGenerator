using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    static ItemGenerator instance;
    public static ItemGenerator GetInstance()
    {
        return instance;
    }
    public enum BlendMode
    {
        Multiply = 0,
        Overlay = 1
    }
    [System.Serializable]public struct TextureGroup
    {
        public enum ConnectorType
        {
            Straight = 0,
            Diagonal = 1,
            Stem = 2
        }
        [SerializeField] string name;
        public List<Texture2D> textures; //First texture is the base, second is the silhouette
        public Vector2 connector;
        public ConnectorType connectorType;
    }
    [System.Serializable]public struct TextureGroupList
    {
        [SerializeField] string name;
        public List<TextureGroup> groups;
    }
    [SerializeField] List<TextureGroupList> lists = new List<TextureGroupList>();
    [SerializeField] Sprite square;

    private void Start() 
    {
        instance = this;
    }
    public Sprite GenerateItemSprite()
    {
        int temp = Random.Range(0, lists[0].groups.Count); //gets a base for the fruit
        int temp2 = Random.Range(0, lists[1].groups.Count); //gets a base for the leaf
        Color fruitColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
        Color leafColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);

        return Sprite.Create(CreateTexture(temp, temp2, fruitColor, leafColor, 0.2f, Random.Range(0.5f, 1.0f)), new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 16);
    }
    public Texture2D CreateTexture(int fruit, int leaf, Color fruitColor, Color leafColor_in, float luminosity, float saturation)
    {
        Texture2D texture = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        List<Texture2D> fruitTextures = lists[0].groups[fruit].textures;
        AdjustColor(ref fruitColor);
        List<Texture2D> leafTextures = lists[1].groups[leaf].textures;
        Color leafColor = leafColor_in;
        Color[] pixels = new Color[32*32];

        for (int i = 0; i < texture.width * texture.height; i++)
        {
            int x = i % texture.width; int y = i / texture.width;
            texture.SetPixel(x, y, Color.clear);
            Color color = Color.clear;
            //Put square
            SetPixel(ref color, GetPixel(square.texture, x, y));
            //Add together the borders
            SetPixel(ref color, GetPixel(leafTextures[1], x - (int)lists[0].groups[fruit].connector.x, y - (int)lists[0].groups[fruit].connector.y));
            SetPixel(ref color, GetPixel(fruitTextures[1], x, y));
            //Add together the colored parts
            SetPixel(ref color, GetPixel(leafTextures[0], x - (int)lists[0].groups[fruit].connector.x, y - (int)lists[0].groups[fruit].connector.y, leafColor, BlendMode.Multiply, luminosity, saturation));
            SetPixel(ref color, GetPixel(fruitTextures[0],x, y, fruitColor, BlendMode.Overlay, luminosity, saturation));
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        return texture;
    }
    Color GetPixel(Texture2D texture, int x, int y)
    {
        if(x >= 0 && y >= 0 && x < texture.width && y < texture.height)
        {
            return texture.GetPixel(x, y);
        }
        return new Color(0,0,0,0);
    }
    Color GetPixel(Texture2D texture, int x, int y, Color color, BlendMode mode, float luminosity, float saturation)
    {
        if (x >= 0 && y >= 0 && x < texture.width && y < texture.height)
        {
            Color temp = texture.GetPixel(x, y);
            float h = 0, s = 0, v = 0;

            switch(mode)
            {
                case BlendMode.Multiply: temp *= color; break;
                case BlendMode.Overlay:
                    if (v > 0.5f)
                    {
                        temp.r = 1 - (1 - temp.r) * (1 - color.r);
                        temp.g = 1 - (1 - temp.g) * (1 - color.g);
                        temp.b = 1 - (1 - temp.b) * (1 - color.b);
                    }
                    else
                    {
                        temp *= color;
                    } break;
            }
            Color.RGBToHSV(temp, out h, out s, out v);
             if(temp.a != 0) { temp = Color.HSVToRGB(h, saturation, v + luminosity); }
            return temp;
        }
        return new Color(0, 0, 0, 0);
    }
    void SetPixel(ref Color newColor, Color color)
    {
        if(color.a != 0) { newColor = color;}
    }
    void AdjustColor(ref Color color)
    {
        //Because, for example, the fruit base cant be desaturated or it will look like crap
        //Meanwhile, leaves dont need this
        float h = 0, s = 0, v = 0;
        Color.RGBToHSV(color, out h, out s, out v);
        if(s < 0.6f) { s = 0.6f; }
        color = Color.HSVToRGB(h, s, v);
    }
}
