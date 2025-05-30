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
        distanceFogMat.SetVector("_FogColor", fogColor);

        Graphics.Blit(_source, _destination, distanceFogMat);
    }
}
