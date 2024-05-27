using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Splines;

public class Wave : MonoBehaviour
{
    public Vector3 size;
    [SerializeField] private float waterLowerSpeed;
    [SerializeField] private float waterRiseSpeed;
    [SerializeField] private GameObject water;
    private Mesh _waterMesh;
    private MeshFilter _waterMeshFilter;
    private Vector3[] _waterMeshVertices;
    public List<MyVertices> _localVertices = new List<MyVertices>();
    private SplineAnimate _splineAnimate;

    public ComputeShader computeShader;
    private ComputeBuffer _vertexBuffer;
    private int _kernelHandle;

    public struct VertexData
    {
        public uint vertexID;
        public Vector3 vertexPosition;
    }
    
    void Awake()
    {
        water = FindAnyObjectByType<Water>().gameObject;
        _waterMeshFilter = water.GetComponent<MeshFilter>();
        _waterMesh = _waterMeshFilter.mesh;
        _waterMeshVertices = _waterMesh.vertices;
        _splineAnimate = this.GetComponent<SplineAnimate>();
        _splineAnimate.Container = GameObject.FindGameObjectWithTag("Spline3").GetComponent<SplineContainer>();
        
        //
        _kernelHandle = computeShader.FindKernel("CSMain");
    }

    private void Update()
    {
        FindLocalVertices();

        // // Debugging: Log the count of _localVertices
        if (_localVertices == null || _localVertices.Count == 0)
        {
            Debug.LogWarning("No vertices found. Skipping ComputeBuffer creation.");
        }
        
        
        ////IF the distance between this gameobject and the first main node in the array is less than 1...destroy this gameobject.
        //if (Vector3.Distance(transform.position, nodeArray[1].position) < 1)
        //{
        //    Destroy(this.gameObject);
        //}
        
        //Add vertices to the data.
        VertexData[] vertexData = new VertexData[_localVertices.Count];
        for (int i = 0; i < _localVertices.Count; i++)
        {
            vertexData[i] = new VertexData
            {
                vertexID = (uint)_localVertices[i].vertexIndex,
                vertexPosition = _localVertices[i].vertexVector
            };
        
            // if (vertexData[i].vertexID != _localVertices[i].vertexIndex)
            // {
            //     Debug.LogError("vertexIndex: " + _localVertices[i].vertexIndex + ". vertexPosition: " + _localVertices[i].vertexVector);
            // }
        }
        
        //
        _vertexBuffer = new ComputeBuffer(vertexData.Length, sizeof(uint) + sizeof(float) * 3);
        _vertexBuffer.SetData(vertexData);
        computeShader.SetBuffer(_kernelHandle, "localVerticesBuffer", _vertexBuffer);
        int threadGroups = Mathf.CeilToInt(vertexData.Length / 8.0f);
        computeShader.Dispatch(_kernelHandle, threadGroups, threadGroups, 1);
        _vertexBuffer.GetData(vertexData);
        
        Debug.Log(vertexData.Length + " " + _waterMeshVertices.Length);
        for (int i = 0; i < vertexData.Length; i++)
        {
            _waterMeshVertices[vertexData[i].vertexID] = vertexData[i].vertexPosition;
        }
        
        //
        _vertexBuffer.Release();
        
        _waterMesh.vertices = _waterMeshVertices;
        _waterMesh.RecalculateBounds();
    }

    private void FindLocalVertices()
    {
        Debug.Log("FindLocalVertices called.");

        if (_localVertices == null)
        {
            _localVertices = new List<MyVertices>();
        }
        else
        {
            _localVertices.Clear(); // Clear the list before repopulating
        }
        Vector3 halfSize = size * 0.5f;
        Vector3 minBounds = transform.position - halfSize;
        Vector3 maxBounds = transform.position + halfSize;

        Vector3[] allVertices = _waterMesh.vertices;

        for (int i = 0; i < allVertices.Length; i++)
        {
            Vector3 worldVertex = _waterMeshFilter.transform.TransformPoint(allVertices[i]); // Convert to world space
            if (IsWithinBounds(worldVertex, minBounds, maxBounds))
            {
                _localVertices.Add(new MyVertices(i, worldVertex));
                //localVertexIndices.Add(i);
            }
        }
    }

    private bool IsWithinBounds(Vector3 point, Vector3 minBounds, Vector3 maxBounds)
    {
        return (point.x >= minBounds.x && point.x <= maxBounds.x &&
                point.y >= minBounds.y && point.y <= maxBounds.y &&
                point.z >= minBounds.z && point.z <= maxBounds.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < _localVertices.Count; i++)
        {
            Gizmos.DrawSphere(_localVertices[i].vertexVector, 1f);
        }
    }
}
