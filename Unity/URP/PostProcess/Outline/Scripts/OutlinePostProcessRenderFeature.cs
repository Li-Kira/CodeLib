using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


[System.Serializable]
public class OutlinePostProcessRenderFeature : ScriptableRendererFeature
{
    private OutlinePass m_OutlinePass;
    
    [System.Serializable]
    public class OutlineObjectSettings
    {
        public Shader m_OutlineShader;
        public FilterSettings filterSettings = new FilterSettings();
    }
    
    [System.Serializable]
    public class FilterSettings
    {
        public RenderQueueType RenderQueueType;
        public LayerMask LayerMask;
        //public LayerMask occluderLayerMask;
        public FilterSettings()
        {
            RenderQueueType = RenderQueueType.Opaque;
            LayerMask = 0;
            //occluderLayerMask = 0;
        }
    }
    
    
    public OutlineObjectSettings settings = new OutlineObjectSettings();
    
    // public NoOutlinePass.ViewSpaceNormalsTextureSettings viewSpaceNormalsTextureSettings = new NoOutlinePass.ViewSpaceNormalsTextureSettings();
    // private NoOutlinePass m_NoOutlinePass;
    
    public override void Create()
    {
        FilterSettings filter = settings.filterSettings;
        var OutlineMaterial = CoreUtils.CreateEngineMaterial(settings.m_OutlineShader);
        
        m_OutlinePass = new OutlinePass(OutlineMaterial,filter.RenderQueueType, filter.LayerMask);
        //m_NoOutlinePass = new NoOutlinePass(RenderPassEvent.BeforeRenderingPrePasses, filter.LayerMask,filter.LayerMask ,viewSpaceNormalsTextureSettings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //renderer.EnqueuePass(m_NoOutlinePass);
        renderer.EnqueuePass(m_OutlinePass);
    }
}

