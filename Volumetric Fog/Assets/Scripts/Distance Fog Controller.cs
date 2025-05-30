using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class DistanceFogController : MonoBehaviour
{
    [Header("Fog")]
    public Shader distanceFogShader;
    [Header("Variables")]
    public Color fogColor;
    public enum FogModes { Linear, Exponential, ExponentialSquared}
    public FogModes fogMode = FogModes.Linear;

    public float fogStart = 0f;
    public float fogEnd = 15f;
    [Range(0.0f, 1.0f)]
    public float fogDensity = 0.1f;

    private Material distanceFogMat;

    private void OnEnable()
    {
        if (distanceFogMat == null && distanceFogShader != null)
        {
            distanceFogMat = new Material(distanceFogShader);
            distanceFogMat.hideFlags = HideFlags.HideAndDontSave;
        }
    }
    private void OnDisable()
    {
        distanceFogMat = null;
    }
    private void OnRenderImage(RenderTexture _source, RenderTexture _destination)
    {
        if(distanceFogMat != null)
        {
            distanceFogMat.SetFloat("_FogStart", fogStart);
            distanceFogMat.SetFloat("_FogEnd", fogEnd);
            distanceFogMat.SetFloat("_FogDensity", fogDensity);
            distanceFogMat.SetVector("_FogColor", fogColor);
            SetKeyword(distanceFogMat, "EXPONENTIAL", fogMode == FogModes.Exponential);
            SetKeyword(distanceFogMat, "EXPONENTIAL_SQRD", fogMode == FogModes.ExponentialSquared);

            Graphics.Blit(_source, _destination, distanceFogMat);
        }
        else
        {
            Graphics.Blit(_source, _destination);
            Debug.LogError("distanceFogMat is null");
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
