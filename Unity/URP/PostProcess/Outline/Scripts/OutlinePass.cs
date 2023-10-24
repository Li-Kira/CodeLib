using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class OutlinePass : ScriptableRenderPass
{
    //Blit
    private RenderTargetIdentifier m_Source;
    private RenderTargetIdentifier m_Destination;
    
    private Material m_Material;
    private OutlineEffectComponent m_OutlineEffect;

    private readonly int temporaryRTId = Shader.PropertyToID("_TempRT");
    
    public OutlinePass(Material OutlineMaterial, RenderQueueType renderQueueType, int layerMask)
    {
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        m_Material = OutlineMaterial;
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
        
        CommandBuffer cmd = CommandBufferPool.Get();
        VolumeStack stack = VolumeManager.instance.stack;
        m_OutlineEffect = stack.GetComponent<OutlineEffectComponent>();

        using (new ProfilingScope(cmd, new ProfilingSampler("Outline Effects")))
        {
            Render(cmd, renderingData);
        }
        
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    public void Render(CommandBuffer cmd, RenderingData renderingData, int pass = 0)
    {
        m_Material.SetFloat("_Scale", m_OutlineEffect.scale.value);
        m_Material.SetFloat("_DepthThreshold", m_OutlineEffect.depthThreshold.value);
        m_Material.SetFloat("_NormalThreshold", m_OutlineEffect.normalThreshold.value);

        Matrix4x4 clipToView = GL.GetGPUProjectionMatrix(renderingData.cameraData.GetProjectionMatrix(), true).inverse;
        m_Material.SetMatrix("_ClipToView", clipToView);
        
        m_Material.SetFloat("_DepthNormalThreshold", m_OutlineEffect.depthNormalThreshold.value);
        m_Material.SetFloat("_DepthNormalThresholdScale", m_OutlineEffect.depthNormalThresholdScale.value);
        
        m_Material.SetColor("_EdgeColor", m_OutlineEffect.edgeColor.value);

        Blit(cmd, m_Source, m_Destination, m_Material, pass);
        Blit(cmd, m_Destination, m_Source);
    }
    
    
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(temporaryRTId);
    }
    
}
