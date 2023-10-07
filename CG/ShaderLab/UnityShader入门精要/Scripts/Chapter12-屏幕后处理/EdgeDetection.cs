using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDetection : PostEffectsBase
{
    public Shader EdgeDetectionShader = null;
    private Material EdgeDetectionMaterial = null;
    
    public Material material
    {
        get
        {
            EdgeDetectionMaterial = CheckShaderAndCreateMaterial(EdgeDetectionShader, EdgeDetectionMaterial);
            return EdgeDetectionMaterial;
        }
    }

    [Range(0.0f, 1.0f)]
    public float edgeOnly = 0.0f;
    public Color edgeColor = Color.black;
    public Color backgroundColor = Color.white;
    
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        { 
            material.SetFloat("_EdgeOnly", edgeOnly);
            material.SetColor("_EdgeColor", edgeColor);
            material.SetColor("_BackgroundColor", backgroundColor);
            
            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
