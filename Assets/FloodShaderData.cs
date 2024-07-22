using System;
using UnityEngine;

public class FloodShaderData : MonoBehaviour
{
    [SerializeField] private Shader floodShader;
    [SerializeField] private Material floodMaterial;
    [SerializeField] private GameObject waveBox;
    private Wave _waveBoxScript;

    private MeshFilter _meshFilter;

    private MyVertices[] _myVerticesArray;

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        if (_meshFilter)
        {
            Mesh mesh = _meshFilter.mesh;
            if (mesh)
            {
                Vector3[] vertices = mesh.vertices;
                _myVerticesArray = new MyVertices[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    _myVerticesArray[i] = new MyVertices(i, vertices[i]);
                }
            }
        }
    }

    private void Update()
    {
        if (!waveBox)
        {
            waveBox = GameObject.FindWithTag("Wave");
            if (waveBox)
            {
                _waveBoxScript = waveBox.GetComponent<Wave>();
            }
        }
        else
        {
            floodMaterial.SetVector("_WaveBoxPosition", waveBox.transform.position);
            floodMaterial.SetVector("_WaveBoxSize", _waveBoxScript.size);
        }
    }
}