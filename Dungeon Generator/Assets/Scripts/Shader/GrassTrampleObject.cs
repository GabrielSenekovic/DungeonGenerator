using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class GrassTrampleObject : MonoBehaviour
{
    [SerializeField]ForwardRendererData rendererSettings = null;

    bool TryGetFeature(out GrassTrampleFeature feature)
    {
        feature = rendererSettings.rendererFeatures.OfType<GrassTrampleFeature>().FirstOrDefault();
        return feature != null;
    }

    private void OnEnable() 
    {
        if(TryGetFeature(out var feature))
        {
            feature.AddTrackedTransform(transform);
        }
    }

    private void OnDisable() 
    {
        if(TryGetFeature(out var feature))
        {
            feature.RemoveTrackedTransform(transform);
        }
    }
}
