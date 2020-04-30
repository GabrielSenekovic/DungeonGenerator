using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LayerAdjuster : MonoBehaviour
{
    [SerializeField] int m_layer;
    List<SpriteRenderer> m_renderers = new List<SpriteRenderer> { };
    public bool m_isEntity;
    public void Awake()
    {
        m_renderers.AddRange(GetComponentsInChildren<SpriteRenderer>());
    }
    public void Update()
    {
        for(int i = 0; i < m_renderers.Count; i++)
        {
            int lowerLeftPosition = 0;
           // m_renderers[i].transform.position = transform.position;
            if(m_renderers[i].sprite)
            {
                lowerLeftPosition = 2 * (int)((transform.position.y - m_renderers[i].sprite.bounds.extents.y) * 10);
            }
            if (m_isEntity)
            {
                m_layer = lowerLeftPosition;
            }
            else
            {
                m_layer = lowerLeftPosition + 1;
            }
            m_renderers[i].sortingOrder = -m_layer;
        }
    }
}
