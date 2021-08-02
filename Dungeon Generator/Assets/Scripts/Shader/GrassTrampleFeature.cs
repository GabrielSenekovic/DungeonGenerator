using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class GrassTrampleFeature : ScriptableRendererFeature
{
    class Pass:ScriptableRenderPass
    {
        Vector4[] tramplePositions;
        int numTramplePositions;

        public Pass(Vector4[] tramplePositions_in)
        {
            tramplePositions = tramplePositions_in;
        }

        public int NumTramplePositions{get => this.numTramplePositions; set => this.numTramplePositions = value;}
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("GrassTrampleFeature");
            cmd.SetGlobalVectorArray("_GrassTramplePositions", tramplePositions);
            cmd.SetGlobalInt("_NumGrassTramplePositions", numTramplePositions);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
    [SerializeField]int maxTrackedTransforms = 8;
    Pass pass;
    List<Transform> trackingTransforms;
    Vector4[] tramplePositions;

    public void AddTrackedTransform(Transform transform)
    {
        trackingTransforms.Add(transform);
    }
    public void RemoveTrackedTransform(Transform transform)
    {
        trackingTransforms.Remove(transform);
    }

    public override void Create()
    {
        trackingTransforms = new List<Transform>();
        trackingTransforms.AddRange(FindObjectsOfType<GrassTrampleObject>().Select((o) => o.transform));
        tramplePositions = new Vector4[maxTrackedTransforms];
        pass = new Pass(tramplePositions);
        pass.renderPassEvent = RenderPassEvent.BeforeRendering;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
#if UNITY_EDITOR
        trackingTransforms.RemoveAll((t) => t == null);
#endif
        for(int i = 0; i < tramplePositions.Length; i++)
        {
            tramplePositions[i] = Vector4.zero;
        }
        int count = (int)Mathf.Min(trackingTransforms.Count, tramplePositions.Length);
        for(int i = 0; i < count; i++)
        {
            Vector3 posn = trackingTransforms[i].position;
            tramplePositions[i] = new Vector4(posn.x, posn.y, posn.z, 1);
        }
        pass.NumTramplePositions = count;
        renderer.EnqueuePass(pass);
    }
}
