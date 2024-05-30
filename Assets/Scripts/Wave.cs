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
    public MyVertices[] vertexData;

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

        vertexData = new MyVertices[_waterMeshVertices.Length];
        for (int i = 0; i < _waterMeshVertices.Length; i++)
        {
            vertexData[i] = new MyVertices(i , _waterMeshVertices[i]);
        }
        
        //_vertexBuffer = new ComputeBuffer(vertexData2.Length, sizeof(uint) + (sizeof(float) * 3) + (sizeof(float) * 3) + (sizeof(float) * 3));
        _vertexBuffer = new ComputeBuffer(vertexData.Length, sizeof(uint) + (sizeof(float) * 3));
    }

    private void Update()
    {
        FindLocalVertices();
        UseComputeShader();
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

        for (int i = 0; i < vertexData.Length; i++)
        {
            Vector3 worldVertex = _waterMeshFilter.transform.TransformPoint(vertexData[i].GetVector());
            if (IsWithinBounds(worldVertex, minBounds, maxBounds))
            {
                _localVertices.Add(vertexData[i]);
            }
        }
    }

    private bool IsWithinBounds(Vector3 point, Vector3 minBounds, Vector3 maxBounds)
    {
        return (point.x >= minBounds.x && point.x <= maxBounds.x &&
                point.y >= minBounds.y && point.y <= maxBounds.y &&
                point.z >= minBounds.z && point.z <= maxBounds.z);
    }
    
    private void UseComputeShader()
    {
        VertexData[] arrayBuffer = new VertexData[_localVertices.Count];
        for (int i = 0; i < _localVertices.Count; i++)
        {
            arrayBuffer[i].vertexID = (uint)_localVertices[i].vertexIndex;
            arrayBuffer[i].vertexPosition = _localVertices[i].vertexVector;
            if (i == 0)
            {
                Debug.Log("POS BEFORE COMPUTE: " + arrayBuffer[i].vertexPosition);
            }
            //arrayBuffer[i].vertexStartPosition = _localVertices[i].vertexVector;
            //arrayBuffer[i].vertexWorldPosition = _waterMeshFilter.transform.TransformPoint(_localVertices[i].GetVector());
        }
        _vertexBuffer.SetData(arrayBuffer);
        computeShader.SetBuffer(_kernelHandle, "localVerticesBuffer", _vertexBuffer);
        int threadGroups = Mathf.CeilToInt(vertexData.Length / 8.0f);
        computeShader.Dispatch(_kernelHandle, threadGroups, threadGroups, 1);
        
        //
        _vertexBuffer.GetData(arrayBuffer);
        Vector3[] vertices = new Vector3[_waterMeshVertices.Length];

        // for (int i = 0; i < _waterMeshVertices.Length; i++)
        // {
        //     vertices[i] = vertexData[i].GetVector();
        // }
        for (int i = 0; i < arrayBuffer.Length; i++)
        {
            if (i == 0)
            {
                Debug.Log("POS AFTER COMPUTE: " + arrayBuffer[i].vertexPosition);
            }
        
            vertices[i] = arrayBuffer[i].vertexPosition;
        }

        _waterMesh.vertices = vertices;
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
