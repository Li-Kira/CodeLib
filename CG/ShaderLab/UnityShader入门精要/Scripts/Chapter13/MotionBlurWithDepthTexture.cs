using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBlurWithDepthTexture : PostEffectsBase
{
    public Shader MotionBlurWithDepthTextureShader = null;
    private Material MotionBlurWithDepthTextureMaterial = null;
    
    public Material material
    {
        get
        {
            MotionBlurWithDepthTextureMaterial = CheckShaderAndCreateMaterial(MotionBlurWithDepthTextureShader, MotionBlurWithDepthTextureMaterial);
            return MotionBlurWithDepthTextureMaterial;
        }
    }
    
    [Range(0.0f, 9.0f)] public float blurSize = 0.5f;

    private Camera myCamera;

    public Camera camera
    {
        get
        {
            if (myCamera == null)
            {
                myCamera = GetComponent<Camera>();
            }
            return myCamera;
        }
    }

    //上一帧摄像机的视角 * 投影矩阵
    private Matrix4x4 previousViewProjectionMatrix;

    //获取相机的深度纹理
    private void OnEnable()
    {
        camera.depthTextureMode |= DepthTextureMode.Depth;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        {
            material.SetMatrix("_PreviousViewProjectionMatrix", previousViewProjectionMatrix);
            Matrix4x4 currentViewProjectionMatrix = camera.projectionMatrix * camera.worldToCameraMatrix;
            Matrix4x4 currentViewProjectionInverseMatrix = currentViewProjectionMatrix.inverse;
            material.SetMatrix("_CurrentViewProjectionInverseMatrix", currentViewProjectionInverseMatrix);
            previousViewProjectionMatrix = currentViewProjectionMatrix;
            
            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
