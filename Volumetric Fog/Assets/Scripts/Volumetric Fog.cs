using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class VolumetricFog : MonoBehaviour
{
    [Header("Fog")]
    public Shader volumetricFogShader;
    public Light dirLight;
    [Header("Variables")]
    public Color fogColor = new Color(0.64f, 0.64f, 0.64f);
    [Range(0.0f, 1.0f)]
    public float fogDensity = 0.1f;
    [Range(1, 64)]
    public int raymarchingSteps = 1;

    private Material volumetricFogMat;
    private Camera cam;
    private CommandBuffer m_afterShadowPass = null;

    private void OnEnable()
    {
        if (volumetricFogMat == null && volumetricFogShader != null)
        {
            volumetricFogMat = new Material(volumetricFogShader);
            volumetricFogMat.hideFlags = HideFlags.HideAndDontSave;
        }
        cam = GetComponent<Camera>();

        AddCommandBuffer();
    }
    private void OnDisable()
    {
        volumetricFogMat = null;
        cam = null;
        RemoveCommandBuffer();
    }
    private void OnRenderImage(RenderTexture _source, RenderTexture _destination)
    {
        if (volumetricFogMat != null)
        {
            volumetricFogMat.SetVector("_FogColor", fogColor);
            volumetricFogMat.SetFloat("_FogDensity", fogDensity);
            float half_FOV_Tan = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            volumetricFogMat.SetFloat("_Half_FOV_Tan", half_FOV_Tan);
            volumetricFogMat.SetInt("_RaymarchingSteps", raymarchingSteps);

            Graphics.Blit(_source, _destination, volumetricFogMat);
        }
        else
        {
            Graphics.Blit(_source, _destination);
            Debug.LogError("volumetricFogMat is null");
        }
    }

    void AddCommandBuffer()
    {
        if (m_afterShadowPass == null && dirLight != null)
        {
            m_afterShadowPass = new CommandBuffer();
            m_afterShadowPass.name = "Shadowmap Copy";

            m_afterShadowPass.Blit(BuiltinRenderTextureType.CurrentActive, BuiltinRenderTextureType.CurrentActive);
            m_afterShadowPass.SetGlobalTexture("_MyShadowMap",
                new RenderTargetIdentifier(BuiltinRenderTextureType.CurrentActive));

            dirLight.AddCommandBuffer(LightEvent.AfterShadowMap, m_afterShadowPass);
        }
    }

    void RemoveCommandBuffer()
    {
        if (m_afterShadowPass != null && dirLight != null)
        {
            dirLight.RemoveCommandBuffer(LightEvent.AfterShadowMap, m_afterShadowPass);
            m_afterShadowPass = null;
        }
    }
    

}


