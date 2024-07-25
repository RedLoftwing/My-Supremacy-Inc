using UnityEngine;

public class VertexAdjuster : MonoBehaviour
{
    public ComputeShader computeShader;
    public GameObject intersectingObject;
    public float intersectionRadius;
    public float ascendSpeed = 0.1f;
    public float descendSpeed = 0.1f;

    private Mesh mesh;
    private Vector3[] originalVertices;
    private ComputeBuffer vertexBuffer;
    private ComputeBuffer originalVertexBuffer;
    private ComputeBuffer vertexYOffsetBuffer;
    private ComputeBuffer outputVertexBuffer;

    private Vector3 thisIntersectingObjectPosition;
    private Vector3 thisObjectSize;
    public float thisMaxVertHeight;
    
    void Start()
    {
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

        if (intersectingObject)
        {
            // Update the compute shader with necessary data
            computeShader.SetFloat("deltaTime", Time.deltaTime);
            
            thisIntersectingObjectPosition = intersectingObject.transform.position;
            computeShader.SetFloats("intersectingObjectPosition", thisIntersectingObjectPosition.x, thisIntersectingObjectPosition.y, thisIntersectingObjectPosition.z);
            
            computeShader.SetFloat("ascendSpeed", ascendSpeed);
            computeShader.SetFloat("descendSpeed", descendSpeed);
            
            thisObjectSize = new Vector3(intersectionRadius, 200.0f, intersectionRadius);
            computeShader.SetFloats("objectSize", intersectionRadius, 200.0f, intersectionRadius); // Assuming a height of 10 for the intersecting object
            computeShader.SetFloat("maxVertHeight", thisMaxVertHeight);
            
            // Set buffers
            computeShader.SetBuffer(0, "vertices", vertexBuffer);
            computeShader.SetBuffer(0, "originalVertexPos", originalVertexBuffer);
            computeShader.SetBuffer(0, "vertexYOffset", vertexYOffsetBuffer);
            computeShader.SetBuffer(0, "outputVertices", outputVertexBuffer);

            // Dispatch the compute shader
            int threadGroups = Mathf.CeilToInt(originalVertices.Length / 1.0f);
            computeShader.Dispatch(0, threadGroups, 1, 1);

            // Get the output data
            Vector3[] outputVertices = new Vector3[originalVertices.Length];
            outputVertexBuffer.GetData(outputVertices);

            // Update the mesh with the new vertex positions
            mesh.vertices = outputVertices;
            mesh.RecalculateNormals();
        }
    }

    private void SetVerticesArray()
    {
        // Get the mesh from the MeshFilter component
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;
    }
    
    private void CreateBuffers()
    {
        vertexBuffer = new ComputeBuffer(originalVertices.Length, sizeof(float) * 3);
        originalVertexBuffer = new ComputeBuffer(originalVertices.Length, sizeof(float) * 3);
        vertexYOffsetBuffer = new ComputeBuffer(originalVertices.Length, sizeof(float));
        outputVertexBuffer = new ComputeBuffer(originalVertices.Length, sizeof(float) * 3);
    }

    private void SetBufferData()
    {
        vertexBuffer.SetData(originalVertices);
        originalVertexBuffer.SetData(originalVertices);
        vertexYOffsetBuffer.SetData(new float[originalVertices.Length]);
        outputVertexBuffer.SetData(originalVertices);
    }

    void OnDestroy()
    {
        // Release buffers
        vertexBuffer.Release();
        originalVertexBuffer.Release();
        vertexYOffsetBuffer.Release();
        outputVertexBuffer.Release();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 1, 1f, 0.75f);
        Gizmos.DrawCube(thisIntersectingObjectPosition, thisObjectSize);

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
