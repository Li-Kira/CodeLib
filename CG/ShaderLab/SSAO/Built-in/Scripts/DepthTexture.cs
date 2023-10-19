using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthTexture : PostEffectBase
{
    public Shader DepthTextureShader = null;
    
    private Material m_DepthTextureMaterial = null;
    public Material material
    {
        get
        {
            m_DepthTextureMaterial = CheckShaderAndCreateMaterial(DepthTextureShader, m_DepthTextureMaterial);
            return m_DepthTextureMaterial;
        }
    }

    private Camera m_Camera; 
    public Camera camera
    {
        get {
            if (m_Camera == null) {
                m_Camera = GetComponent<Camera>();
            }
            return m_Camera;
        }
    }
    
    private void OnEnable()
    {
        camera.depthTextureMode = DepthTextureMode.Depth;
    }


    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        {
            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }



}
