using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloodShaderData : MonoBehaviour
{
    [SerializeField] private Shader floodShader;
    [SerializeField] private Material floodMaterial;
    [SerializeField] private GameObject waveBox;
    private Wave _waveBoxScript;

    private void Update()
    {
        if (!waveBox)
        {
            waveBox = GameObject.FindWithTag("Wave");
            _waveBoxScript = waveBox.GetComponent<Wave>();
        }
        else
        {
            floodMaterial.SetVector("_WaveBoxPosition", waveBox.transform.position);
            floodMaterial.SetVector("_WaveBoxSize", _waveBoxScript.size);
        }
    }
}