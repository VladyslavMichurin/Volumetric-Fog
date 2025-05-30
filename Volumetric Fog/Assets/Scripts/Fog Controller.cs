using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FogController : MonoBehaviour
{
    [Header("Fog")]
    public Shader distanceFogShader;
    [Header("Variables")]
    public Color fogColor;
    public enum FogModes { Linear, Exponential, ExponentialSquared}
    public FogModes fogMode = FogModes.Linear;
    [Range(0.0f, 1.0f)]
    public float fogStart = 0;
    [Range(0.0f, 1.0f)]
    public float fogEnd = 1;

    private Material distanceFogMat;

    private void OnEnable()
    {
        if (distanceFogMat == null)
        {
            distanceFogMat = new Material(distanceFogShader);
            distanceFogMat.hideFlags = HideFlags.HideAndDontSave;
        }
    }
    private void OnRenderImage(RenderTexture _source, RenderTexture _destination)
    {
        distanceFogMat.SetFloat("_FogStart", fogStart);
        distanceFogMat.SetFloat("_FogEnd", fogEnd);
        distanceFogMat.SetVector("_FogColor", fogColor);

        Graphics.Blit(_source, _destination, distanceFogMat);
    }
}
