using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[DisallowMultipleRendererFeature]
[Tooltip("The Scene Normals pass enables rendering to the CameraNormalsTexture if no other pass does it already.")]
internal class DepthNormals : ScriptableRendererFeature
{
    private SceneNormalsPass m_SceneNormalsPass = null;

    public override void Create()
    {
        if (m_SceneNormalsPass == null)
        {
            m_SceneNormalsPass = new SceneNormalsPass();
        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_SceneNormalsPass.Setup();
        renderer.EnqueuePass(m_SceneNormalsPass);
    }


    // The Scene Normals Pass
    private class SceneNormalsPass : ScriptableRenderPass
    {
        public void Setup()
        {
            ConfigureInput(ScriptableRenderPassInput.Normal); // all of this to just call this one line
            return;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) { }
    }
}
