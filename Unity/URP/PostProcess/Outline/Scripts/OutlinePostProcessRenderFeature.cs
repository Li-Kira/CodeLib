using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class OutlinePostProcessRenderFeature : ScriptableRendererFeature
{
    [SerializeField]
    private Shader m_OutlineShader;
    private OutlinePass m_OutlinePass;
    private NoOutlinePass m_NoOutlinePass;
    
    [SerializeField]
    private LayerMask outlinesLayerMask;
    [SerializeField]
    private LayerMask outlinesOccluderLayerMask;
    
    public override void Create()
    {
        var OutlineMaterial = CoreUtils.CreateEngineMaterial(m_OutlineShader);
        
        //m_NoOutlinePass = new NoOutlinePass(outlinesLayerMask, outlinesOccluderLayerMask);
        m_OutlinePass = new OutlinePass(OutlineMaterial);
        
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //renderer.EnqueuePass(m_NoOutlinePass);
        renderer.EnqueuePass(m_OutlinePass);
    }
}

