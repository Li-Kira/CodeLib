using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[DisallowMultipleRendererFeature]
public class SSAOPostProcessRenderFeature : ScriptableRendererFeature
{
    private SSAOPass m_SSAOPass;
    private Material m_Material;
    
    [Serializable]
    public class SSAORenderSettings
    {
        public Shader m_Shader;
    }
    
    public SSAORenderSettings settings = new SSAORenderSettings();
    
    public override void Create()
    {
        m_Material = CoreUtils.CreateEngineMaterial(settings.m_Shader);
        m_SSAOPass = new SSAOPass(m_Material);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_SSAOPass);
    }
    
    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(m_Material);
    }


}
