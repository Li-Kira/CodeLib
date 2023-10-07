using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloom : PostEffectsBase
{
    public Shader BloomShader = null;
    private Material BloomMaterial = null;
    
    public Material material
    {
        get
        {
            BloomMaterial = CheckShaderAndCreateMaterial(BloomShader, BloomMaterial);
            return BloomMaterial;
        }
    }
    
    [Range(0f, 4f)]
    public int iterations = 3;
    [Range(0.2f, 3.0f)]
    public float blurSpread = 0.6f;
    [Range(1f, 8f)]
    public int downSample = 2;
    [Range(0.0f, 4.0f)]
    public float luminanceThreshold = 0.6f;
    
    void OnRenderImage (RenderTexture src, RenderTexture dest) {
        if (material != null) {
            material.SetFloat("_LuminanceThreshold", luminanceThreshold);

            int rtW = src.width/downSample;
            int rtH = src.height/downSample;
			
            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;
			
            Graphics.Blit(src, buffer0, material, 0);
			
            for (int i = 0; i < iterations; i++) {
                material.SetFloat("_BlurSize", 1.0f + i * blurSpread);
				
                RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
				
                // Render the vertical pass
                Graphics.Blit(buffer0, buffer1, material, 1);
				
                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
                buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
				
                // Render the horizontal pass
                Graphics.Blit(buffer0, buffer1, material, 2);
				
                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
            }

            material.SetTexture ("_Bloom", buffer0);  
            Graphics.Blit (src, dest, material, 3);  

            RenderTexture.ReleaseTemporary(buffer0);
        } else {
            Graphics.Blit(src, dest);
        }
    }
    
}
