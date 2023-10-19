using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class CustomPostProcessRenderFeature : ScriptableRendererFeature
{
   [SerializeField] 
   private Shader m_bloomShader;
   [SerializeField] 
   private Shader m_compositeShader;

   private Material m_bloomMaterial;
   private Material m_compositeMaterial;
   
   private CustomPostProcessPass m_customPass;
   
   public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
   {
      renderer.EnqueuePass(m_customPass);
   }

   public override void Create()
   {
      //动态创建材质
      m_bloomMaterial = CoreUtils.CreateEngineMaterial(m_bloomShader);
      m_compositeMaterial = CoreUtils.CreateEngineMaterial(m_compositeShader);
      
      m_customPass = new CustomPostProcessPass(m_bloomMaterial, m_compositeMaterial);
   }

   protected override void Dispose(bool disposing)
   {
      if (disposing)
      {
         CoreUtils.Destroy(m_bloomMaterial);
         CoreUtils.Destroy(m_compositeMaterial); 
      }
   }

   public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
   {
      if (renderingData.cameraData.cameraType == CameraType.Game)
      {
         m_customPass.ConfigureInput(ScriptableRenderPassInput.Depth);
         m_customPass.ConfigureInput(ScriptableRenderPassInput.Color);
         m_customPass.SetTarget(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
      }
   }
   
}
 