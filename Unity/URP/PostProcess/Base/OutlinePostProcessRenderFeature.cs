using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class OutlinePostProcessRenderFeature : ScriptableRendererFeature
{
    [SerializeField]
    private Shader m_outlineShader;
    private OutlinePass m_outlinePass;
    
    public override void Create()
    {
        var OutlineMaterial = CoreUtils.CreateEngineMaterial(m_outlineShader);
        m_outlinePass = new OutlinePass(OutlineMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_outlinePass);
    }
}

