using UnityEngine;

public class VertexAdjuster : MonoBehaviour
{
    public static VertexAdjuster Instance { get; private set; }
    
    public ComputeShader computeShader;
    public GameObject intersectingObject;
    private const float IntersectionRadius = 12.0f;
    private const float AscendSpeed = 3.0f;
    private const float DescendSpeed = 0.5f;
    private const float MaxVertHeight = 4.9f;

    private Mesh _mesh;
    private Vector3[] _originalVertices;
    private ComputeBuffer _vertexBuffer;
    private ComputeBuffer _originalVertexBuffer;
    private ComputeBuffer _vertexYOffsetBuffer;
    private ComputeBuffer _outputVertexBuffer;

    private Vector3 _thisIntersectingObjectPosition;
    private Vector3 _thisObjectSize;

    private Vector3[] _outputVertices;
    
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    
    void Start()
    {
        // Set some compute shader values that do not require constant updates.
        computeShader.SetFloat("_ascendSpeed", AscendSpeed);
        computeShader.SetFloat("_descendSpeed", DescendSpeed);
        _thisObjectSize = new Vector3(IntersectionRadius, 200.0f, IntersectionRadius); // Can remove if not using gizmos.
        computeShader.SetFloats("_objectHalfSize", IntersectionRadius / 2, 200.0f / 2, IntersectionRadius / 2);
        computeShader.SetFloat("_maxVertHeight", MaxVertHeight);
        
        SetVerticesArray();
        CreateBuffers();
        SetBufferData();
    }
    
    void Update()
    {
        if (!intersectingObject)
        {
            intersectingObject = GameObject.FindWithTag("Wave");
        }

        if (!intersectingObject) return;
        
        // -- Update the compute shader with necessary data --
        computeShader.SetFloat("_deltaTime", Time.deltaTime);
        _thisIntersectingObjectPosition = intersectingObject.transform.position; // Can remove if not using gizmos.
        computeShader.SetFloats("_intersectingObjectPosition", 
 intersectingObject.transform.position.x, 
            intersectingObject.transform.position.y, 
            intersectingObject.transform.position.z);
        
        // Set each buffer.
        computeShader.SetBuffer(0, "vertices", _vertexBuffer);
        computeShader.SetBuffer(0, "originalVertexPos", _originalVertexBuffer);
        computeShader.SetBuffer(0, "vertexYOffset", _vertexYOffsetBuffer);
        computeShader.SetBuffer(0, "outputVertices", _outputVertexBuffer);

        // Dispatch the compute shader
        int threadGroups = Mathf.CeilToInt(_originalVertices.Length / 1.0f);
        computeShader.Dispatch(0, threadGroups, 1, 1);

        // Get the output data from the outputVertices buffer.
        _outputVertexBuffer.GetData(_outputVertices);

        // Update the mesh with the new vertex positions
        _mesh.vertices = _outputVertices;
        _mesh.RecalculateNormals();
    }

    private void SetVerticesArray()
    {
        // Get the mesh from the MeshFilter component
        _mesh = GetComponent<MeshFilter>().mesh;
        _originalVertices = _mesh.vertices;
        
        // 
        _outputVertices = new Vector3[_originalVertices.Length];
    }
    
    private void CreateBuffers()
    {
        _vertexBuffer = new ComputeBuffer(_originalVertices.Length, sizeof(float) * 3);
        _originalVertexBuffer = new ComputeBuffer(_originalVertices.Length, sizeof(float) * 3);
        _vertexYOffsetBuffer = new ComputeBuffer(_originalVertices.Length, sizeof(float));
        _outputVertexBuffer = new ComputeBuffer(_originalVertices.Length, sizeof(float) * 3);
    }

    private void SetBufferData()
    {
        _vertexBuffer.SetData(_originalVertices);
        _originalVertexBuffer.SetData(_originalVertices);
        _vertexYOffsetBuffer.SetData(new float[_originalVertices.Length]);
        _outputVertexBuffer.SetData(_originalVertices);
    }

    void OnDestroy()
    {
        // Release buffers
        _vertexBuffer.Release();
        _originalVertexBuffer.Release();
        _vertexYOffsetBuffer.Release();
        _outputVertexBuffer.Release();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 1, 1f, 0.75f);
        Gizmos.DrawCube(_thisIntersectingObjectPosition, _thisObjectSize);

        // for (int i = 0; i < originalVertices.Length; i++)
        // {
        //     if (i % 16 == 0)
        //     {
        //         Gizmos.color = Color.red;
        //         Gizmos.DrawCube(originalVertices[i], new Vector3(1, 1000, 1));
        //     }
        // }
    }
}