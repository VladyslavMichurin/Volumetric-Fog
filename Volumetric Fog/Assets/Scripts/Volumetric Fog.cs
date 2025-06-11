using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class VolumetricFog : MonoBehaviour
{
    [Header("Fog")]
    public Shader volumetricFogShader;
    [Header("Variables")]
    public Color fogColor = new Color(0.64f, 0.64f, 0.64f);
    [Range(0.0f, 1.0f)]
    public float fogDensity = 0.1f;
    [Range(1.0f, 1000.0f)]
    public float maxDistance = 100.0f;
    [Range(0.01f, 100.0f)]
    public float stepSize = 1;

    private Material volumetricFogMat;
    private Camera cam;

    private void OnEnable()
    {
        if (volumetricFogMat == null && volumetricFogShader != null)
        {
            volumetricFogMat = new Material(volumetricFogShader);
            volumetricFogMat.hideFlags = HideFlags.HideAndDontSave;
        }
        cam = GetComponent<Camera>();
    }
    private void OnDisable()
    {
        volumetricFogMat = null;
        cam = null;
    }
    private void OnRenderImage(RenderTexture _source, RenderTexture _destination)
    {
        if (volumetricFogMat != null)
        {
            volumetricFogMat.SetVector("_FogColor", fogColor);
            volumetricFogMat.SetFloat("_FogDensity", fogDensity);
            volumetricFogMat.SetFloat("_MaxDistance", maxDistance);
            float half_FOV_Tan = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            volumetricFogMat.SetFloat("_Half_FOV_Tan", half_FOV_Tan);
            volumetricFogMat.SetFloat("_StepSize", stepSize);

            Graphics.Blit(_source, _destination, volumetricFogMat);
        }
        else
        {
            Graphics.Blit(_source, _destination);
            Debug.LogError("volumetricFogMat is null");
        }
    }
}
