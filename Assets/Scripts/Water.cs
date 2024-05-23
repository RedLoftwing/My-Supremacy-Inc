using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Water : MonoBehaviour
{
    [SerializeField] private Wave wave;

    [SerializeField] private Mesh mesh;
    [SerializeField] private float meshWidth;
    [SerializeField] private float meshDepth;
    [SerializeField] private float maxMeshHeight;
    [SerializeField] private int cellCount;
    private int cellCountX, cellCountZ;

    //[SerializeField] private float perlinStepSizeX = 0.1f;
    //[SerializeField] private float perlinStepSizeZ = 0.1f;

    //[SerializeField] private Vector3[] vertices;
    [SerializeField] private MyVertices[] myVertices;
    [SerializeField] private Vector3[] verticesVectors;
    //private Color[] colours;
    public Vector2[] uvs;
    //private Vector3[] normals;
    public int[] triangles;

    // Start is called before the first frame update
    void Start()
    {
        CreateWaterMesh();
    }

    private void CreateWaterMesh()
    {
        if(mesh == null)
        {
            mesh = new Mesh();
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
        }

        cellCountX = cellCount;
        cellCountZ = cellCount;

        int verticesRowCount = cellCountX + 1;
        int verticesCount = verticesRowCount * (cellCountZ + 1);
        int trianglesCount = 6 * cellCountX * cellCountZ;

        //vertices = new Vector3[verticesCount];
        myVertices = new MyVertices[verticesCount];
        verticesVectors = new Vector3[verticesCount];
        uvs = new Vector2[verticesCount];
        //colours = new Color[verticesCount];
        //normals = new Vector3[verticesCount];
        triangles = new int[trianglesCount];



        // Set the vertices of the mesh
        int vertexIndex = 0;
        for (int z = 0; z <= cellCountZ; ++z)
        {
            float percentageZ = (float)z / (float)cellCountZ;
            float startZ = percentageZ * meshDepth;

            for (int x = 0; x <= cellCountX; ++x)
            {
                float percentageX = (float)x / (float)cellCountX;
                float startX = percentageX * meshWidth;
                float height = maxMeshHeight;

                myVertices[vertexIndex] = new MyVertices(vertexIndex, new Vector3(startX, height, startZ));
                verticesVectors[vertexIndex] = new Vector3(startX, height, startZ);
                uvs[vertexIndex] = new Vector2(percentageX, percentageZ);
                //normals[vertexIndex] = Vector3.up;
                ++vertexIndex;
            }
        }

        // Setup the indexes so they are in the correct order and will render correctly
        vertexIndex = 0;
        int trianglesIndex = 0;
        for (int z = 0; z < cellCountZ; ++z)
        {
            for (int x = 0; x < cellCountX; ++x)
            {
                vertexIndex = x + (verticesRowCount * z);

                triangles[trianglesIndex++] = vertexIndex;
                triangles[trianglesIndex++] = vertexIndex + verticesRowCount;
                triangles[trianglesIndex++] = (vertexIndex + 1) + verticesRowCount;
                triangles[trianglesIndex++] = (vertexIndex + 1) + verticesRowCount;
                triangles[trianglesIndex++] = vertexIndex + 1;
                triangles[trianglesIndex++] = vertexIndex;
            }
        }

        //// Assign all of the data that has been created to the mesh and update it
        //Vector3[] returnedVertices = myVertices;

        mesh.vertices = verticesVectors;
        Debug.Log("mesh uvs: " + mesh.uv.Length + ". \nuvs: " + uvs + ". \nmesh triangles: " + mesh.triangles.Length + ". \ntriangles: " + triangles);
        mesh.uv = uvs;
        mesh.triangles = triangles;
        //mesh.colors = colours;
        //mesh.normals = normals;
        mesh.RecalculateNormals();
        mesh.UploadMeshData(false);
    }
}
