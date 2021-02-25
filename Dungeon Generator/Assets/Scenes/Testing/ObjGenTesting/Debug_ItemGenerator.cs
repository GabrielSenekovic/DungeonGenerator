using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Debug_ItemGenerator : MonoBehaviour
{
    [SerializeField] ItemGenerator itemGenerator;
    [SerializeField] int fruit;
    [SerializeField] Color fruitColor;
    [SerializeField] int leaf;
    [SerializeField] Color leafColor;
    [Range(0.0f, 0.2f)] [SerializeField] float luminosity;

    [Range(0.0f, 1.0f)] [SerializeField] float saturation;

    SpriteRenderer myRenderer;

    private void Start()
    {
        myRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void OnGenerate()
    {
        Texture2D texture = itemGenerator.CreateTexture(fruit, leaf, fruitColor, leafColor, luminosity, saturation);
        Rect rect = new Rect(0, 0, 32, 32);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        myRenderer.sprite = Sprite.Create(texture, rect, pivot, 16);
    }
    public void OnGenerateRandom()
    {
        myRenderer.sprite = itemGenerator.GenerateItemSprite();
    }
}
