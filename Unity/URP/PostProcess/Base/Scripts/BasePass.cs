using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class BasePass : ScriptableRenderPass
{
    private RenderTargetIdentifier m_Source;
    private RenderTargetIdentifier m_Destination;
    
    private Material m_material;

    readonly int temporaryRTId = Shader.PropertyToID("_TempRT");


    public OutlinePass(Material OutlineMaterial)
    {
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        m_material = OutlineMaterial;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;
        
        m_Source = renderingData.cameraData.renderer.cameraColorTarget;
        
        cmd.GetTemporaryRT(temporaryRTId, descriptor, FilterMode.Bilinear);
        m_Destination = new RenderTargetIdentifier(temporaryRTId);
    }
    
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.isSceneViewCamera)
            return;

        CommandBuffer cmd = CommandBufferPool.Get("Outline");
        VolumeStack stack = VolumeManager.instance.stack;
        OutlineEffectComponent customEffect = stack.GetComponent<OutlineEffectComponent>();

        using (new ProfilingScope(cmd, new ProfilingSampler("Outline Post Process Effects")))
        {
            // m_material.SetFloat("_Intensity", customEffect.intensity.value);
            // m_material.SetColor("_OverlayColor", customEffect.overlayColor.value);
            
            // m_material.SetFloat("_Thickness", customEffect.thickness.value);
            // m_material.SetFloat("_MinDepth", customEffect.depthMin.value);
            // m_material.SetFloat("_MaxDepth", customEffect.depthMax.value);
            // m_material.SetColor("_EdgeColor", customEffect.edgeColor.value);
            
            
            Blit(cmd, m_Source, m_Destination, m_material, 0);
            Blit(cmd, m_Destination, m_Source);
        }
        
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }
    
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(temporaryRTId);
    }
    
}
