using System.Collections.Generic;
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
    public List<VertexData> _localVertices1 = new List<VertexData>();
    private SplineAnimate _splineAnimate;

    public ComputeShader computeShader;
    private ComputeBuffer _vertexBuffer;
    private int _kernelHandle;
    private VertexData[] vertexData;
    public MyVertices[] vertexData2;

    public struct VertexData
    {
        public uint vertexID;
        public Vector3 vertexStartPosition;
        public Vector3 vertexPosition;
        public Vector3 vertexWorldPosition;
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
        
        
        //Convert the vertices of the mesh to the VertexData struct
        //vertexData = new VertexData[_waterMeshVertices.Length];
        for (int i = 0; i < _waterMeshVertices.Length; i++)
        {
            // vertexData[i] = new VertexData
            // {
            //     vertexID = (uint)i,
            //     vertexStartPosition = _waterMeshVertices[i],
            //     vertexPosition = _waterMeshVertices[i],
            //     vertexWorldPosition = _waterMeshFilter.transform.TransformPoint(_waterMeshVertices[i]) // Convert to world space
            // };
        }

        vertexData2 = new MyVertices[_waterMeshVertices.Length];
        for (int i = 0; i < _waterMeshVertices.Length; i++)
        {
            vertexData2[i] = new MyVertices(i , _waterMeshVertices[i]);
        }
        
        _vertexBuffer = new ComputeBuffer(vertexData2.Length, sizeof(uint) + (sizeof(float) * 3) + (sizeof(float)));
    }

    private void Update()
    {
        FindLocalVertices();
        UseComputeShader2();
    }

    private void FindLocalVertices()
    {
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

        for (int i = 0; i < vertexData2.Length; i++)
        {
            //Vector3 worldVertex = _waterMeshFilter.transform.TransformPoint(allVertices[i]); // Convert to world space
            // if (IsWithinBounds(vertexData[i].vertexWorldPosition, minBounds, maxBounds))
            // {
            //     _localVertices.Add(new MyVertices(i, vertexData[i].vertexWorldPosition));
            //     //_localVertices1.Add();
            //     //localVertexIndices.Add(i);
            // }

            Vector3 worldVertex = _waterMeshFilter.transform.TransformPoint(vertexData2[i].GetVector());
            if (IsWithinBounds(worldVertex, minBounds, maxBounds))
            {
                //Debug.Log("The vertex to add: " + vertexData2[i].GetIndex());
                _localVertices.Add(vertexData2[i]);
            }
        }
    }

    private bool IsWithinBounds(Vector3 point, Vector3 minBounds, Vector3 maxBounds)
    {
        return (point.x >= minBounds.x && point.x <= maxBounds.x &&
                point.y >= minBounds.y && point.y <= maxBounds.y &&
                point.z >= minBounds.z && point.z <= maxBounds.z);
    }

    private void UseComputeShader0()
    {
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

    private void UseComputeShader1()
    {
        VertexData[] localVertexArray = new VertexData[_localVertices.Count];
        for (int i = 0; i < _localVertices.Count; i++)
        {
            localVertexArray[i] = vertexData[i];
        }
        _vertexBuffer.SetData(localVertexArray);
    }
    
    private void UseComputeShader2()
    {
        VertexData[] arrayBuffer = new VertexData[_localVertices.Count];
        for (int i = 0; i < _localVertices.Count; i++)
        {
            arrayBuffer[i].vertexID = (uint)_localVertices[i].vertexIndex;
            arrayBuffer[i].vertexPosition = _localVertices[i].vertexVector;
            arrayBuffer[i].vertexStartPosition = _localVertices[i].vertexVector;
            arrayBuffer[i].vertexWorldPosition = _waterMeshFilter.transform.TransformPoint(_localVertices[i].GetVector());
        }
        _vertexBuffer.SetData(arrayBuffer);
        computeShader.SetBuffer(_kernelHandle, "localVerticesBuffer", _vertexBuffer);
        int threadGroups = Mathf.CeilToInt(vertexData2.Length / 8.0f);
        computeShader.Dispatch(_kernelHandle, threadGroups, threadGroups, 1);
        
        //
        _vertexBuffer.GetData(arrayBuffer);
        Vector3[] vertices = new Vector3[_waterMeshVertices.Length];
        for (int i = 0; i < _localVertices.Count; i++)
        {
            vertices[i] = arrayBuffer[i].vertexPosition;
        }

        _waterMeshVertices = vertices;
        _waterMesh.RecalculateNormals();
        _waterMeshFilter.mesh = _waterMesh;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < _localVertices.Count; i++)
        {
            var test = _waterMeshFilter.transform.TransformPoint(_localVertices[i].GetVector());
            Gizmos.DrawSphere(test, 1f);
        }
    }
}
