using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MotionBlur : PostEffectsBase
{
    public Shader MotionBlurShader = null;
    private Material MotionBlurMaterial = null;
    
    public Material material
    {
        get
        {
            MotionBlurMaterial = CheckShaderAndCreateMaterial(MotionBlurShader, MotionBlurMaterial);
            return MotionBlurMaterial;
        }
    }

    [Range(0.0f, 9.0f)] public float blurAmount = 0.5f;

    private RenderTexture accumulationTexture;

    private void OnDisable()
    {
        DestroyImmediate(accumulationTexture);
    }

    void OnRenderImage (RenderTexture src, RenderTexture dest) {
        if (material != null) {
            // Create the accumulation texture
            if (accumulationTexture == null || accumulationTexture.width != src.width || accumulationTexture.height != src.height) {
                DestroyImmediate(accumulationTexture);
                accumulationTexture = new RenderTexture(src.width, src.height, 0);
                accumulationTexture.hideFlags = HideFlags.HideAndDontSave;
                Graphics.Blit(src, accumulationTexture);
            }

            // We are accumulating motion over frames without clear/discard
            // by design, so silence any performance warnings from Unity
            accumulationTexture.MarkRestoreExpected();

            material.SetFloat("_BlurAmount", 1.0f - blurAmount);

            Graphics.Blit (src, accumulationTexture, material);
            Graphics.Blit (accumulationTexture, dest);
        } else {
            Graphics.Blit(src, dest);
        }
    }
        
    
}
