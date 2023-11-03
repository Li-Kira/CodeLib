using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SSAOPass : ScriptableRenderPass
{
    private RenderTargetIdentifier m_Source;
    private RenderTargetIdentifier m_Destination;
    private RenderTargetIdentifier m_BlurBuffer1;
    private RenderTargetIdentifier m_BlurBuffer2;
    
    private Material m_Material;
    private SSAOEffectComponent m_Effect;
    private readonly int temporaryRTId_0 = Shader.PropertyToID("_TempRT");
    private readonly int temporaryRTId_1 = Shader.PropertyToID("_BlurRT1");
    private readonly int temporaryRTId_2 = Shader.PropertyToID("_BlurRT2");

    public SSAOPass(Material material)
    {
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        m_Material = material;
    }
    
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;
        
        m_Source = renderingData.cameraData.renderer.cameraColorTarget;
        
        cmd.GetTemporaryRT(temporaryRTId_0, descriptor, FilterMode.Bilinear);
        cmd.GetTemporaryRT(temporaryRTId_1, descriptor, FilterMode.Bilinear);
        cmd.GetTemporaryRT(temporaryRTId_2, descriptor, FilterMode.Bilinear);
        
        m_Destination = new RenderTargetIdentifier(temporaryRTId_0);
        m_BlurBuffer1 = new RenderTargetIdentifier(temporaryRTId_1);
        m_BlurBuffer2 = new RenderTargetIdentifier(temporaryRTId_2);
    }
    
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.isSceneViewCamera)
            return;
        
        CommandBuffer cmd = CommandBufferPool.Get();
        VolumeStack stack = VolumeManager.instance.stack;
        m_Effect = stack.GetComponent<SSAOEffectComponent>();
        
        using (new ProfilingScope(cmd, new ProfilingSampler("SSAO Effects")))
        {
            Render(cmd, ref renderingData);
        }
        
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }
    
    public void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        m_Material.SetColor("_aoColor", m_Effect.color.value);

        Matrix4x4 vp_Matrix = renderingData.cameraData.camera.projectionMatrix * renderingData.cameraData.camera.worldToCameraMatrix;
        m_Material.SetMatrix("_VPMatrix_invers", vp_Matrix.inverse);
        Matrix4x4 v_Matrix = renderingData.cameraData.camera.worldToCameraMatrix;
        m_Material.SetMatrix("_VMatrix", v_Matrix);
        Matrix4x4 p_Matrix = renderingData.cameraData.camera.projectionMatrix;
        m_Material.SetMatrix("_PMatrix", p_Matrix);
        
        m_Material.SetFloat("_SampleCount", m_Effect.sampleCount.value);
        m_Material.SetFloat("_Radius", m_Effect.radius.value);
        m_Material.SetFloat("_RangeCheck", m_Effect.rangeCheck.value);
        m_Material.SetFloat("_AOInt", m_Effect.aoInt.value);
        
        
        // m_Material.SetFloat("_BlurRadius", m_Effect.blurRadius.value);
        // m_Material.SetFloat("_BilaterFilterFactor", m_Effect.bilaterFilterFactor.value);
        m_Material.SetFloat("_BlurSize", m_Effect.blurSize.value);

            
        Blit(cmd, m_Source, m_Destination);
        
        // Test SSAO
        //Blit(cmd, m_Destination, m_Source, m_Material, 0);
        
        // Test Blur
        // Blit(cmd, m_Destination, m_BlurBuffer1, m_Material, 0);
        // cmd.SetGlobalTexture("_AOTex", m_BlurBuffer1);  
        // Blit(cmd, m_BlurBuffer1, m_BlurBuffer2, m_Material, 1);
        // cmd.SetGlobalTexture("_AOTex", m_BlurBuffer2);  
        // Blit(cmd, m_Destination, m_Source, m_Material, 2);
        
        // Final
        Blit(cmd, m_Destination, m_BlurBuffer1, m_Material, 0);
        cmd.SetGlobalTexture("_AOTex", m_BlurBuffer1);  
        Blit(cmd, m_BlurBuffer1, m_BlurBuffer2, m_Material, 1);
        cmd.SetGlobalTexture("_AOTex", m_BlurBuffer2);  
        Blit(cmd, m_BlurBuffer2, m_BlurBuffer1, m_Material, 2);
        cmd.SetGlobalTexture("_AOTex", m_BlurBuffer1);  
        Blit(cmd, m_Destination, m_Source, m_Material, 3);
        
    }
    
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(temporaryRTId_0);
        cmd.ReleaseTemporaryRT(temporaryRTId_1);
        cmd.ReleaseTemporaryRT(temporaryRTId_2);
    }
    
}
