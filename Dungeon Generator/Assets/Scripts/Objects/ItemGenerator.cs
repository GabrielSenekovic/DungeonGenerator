using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [System.Serializable]public struct TextureGroup
    {
        public enum ConnectorType
        {
            Straight = 0,
            Diagonal = 1
        }
        [SerializeField] string name;
        public List<Texture2D> textures;
        public Vector2 connector;
        public ConnectorType connectorType;
    }
    [System.Serializable]public struct TextureGroupList
    {
        [SerializeField] string name;
        public List<TextureGroup> groups;
    }
    [SerializeField] List<TextureGroupList> lists = new List<TextureGroupList>();
    public static Sprite GenerateItemSprite()
    {
        Sprite sprite = null;

        return sprite;
    }
}
