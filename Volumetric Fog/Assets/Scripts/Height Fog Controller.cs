using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]
public class HeightFogController : MonoBehaviour
{
    [Header("Fog")]
    public Shader heightFogShader;
    [Header("Variables")]
    public Color fogColor = new Color(0.64f, 0.64f, 0.64f);
    public enum FogModes { Linear, Exponential, ExponentialSquared }
    public FogModes fogMode = FogModes.Linear;

    public float fogStart = 0f;
    public float fogEnd = 15f;
    [Range(0.0f, 1.0f)]
    public float fogDensity = 0.1f;

    private Material heightFogMat;
    private Camera cam;

    private void OnEnable()
    {
        if (heightFogMat == null && heightFogShader != null)
        {
            heightFogMat = new Material(heightFogShader);
            heightFogMat.hideFlags = HideFlags.HideAndDontSave;
        }
        
        cam = GetComponent<Camera>();
    }
    private void OnDisable()
    {
        heightFogMat = null;
        cam = null;
    }
    private void OnRenderImage(RenderTexture _source, RenderTexture _destination)
    {
        if (heightFogMat != null)
        {
            heightFogMat.SetFloat("_FogStart", fogStart);
            heightFogMat.SetFloat("_FogEnd", fogEnd);
            heightFogMat.SetFloat("_FogDensity", fogDensity);
            heightFogMat.SetVector("_FogColor", fogColor);
            float halfFOVTan = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            heightFogMat.SetFloat("_Half_FOV_Tan", halfFOVTan);
            SetKeyword(heightFogMat, "EXPONENTIAL", fogMode == FogModes.Exponential);
            SetKeyword(heightFogMat, "EXPONENTIAL_SQRD", fogMode == FogModes.ExponentialSquared);

            Graphics.Blit(_source, _destination, heightFogMat);
        }
        else
        {
            Graphics.Blit(_source, _destination);
            Debug.LogError("heightFogMat is null");
        }
    }

    void SetKeyword(Material _mat, string _keyword, bool _state)
    {
        if (_state)
        {
            _mat.EnableKeyword(_keyword);
        }
        else
        {
            _mat.DisableKeyword(_keyword);
        }
    }
}
