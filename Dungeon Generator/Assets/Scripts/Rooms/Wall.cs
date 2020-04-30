using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Wall : MonoBehaviour
{
    [SerializeField]List<SpriteRenderer> m_renderers;
    public void Awake()
    {
        //m_renderers.AddRange(GetComponentsInChildren<SpriteRenderer>());
    }
    public void ChangeColor(Color color, int i)
    {
        m_renderers[i].color = color;
    }
    public int GetAmountOfRenderers()
    {
        return m_renderers.Count;
    }
}
