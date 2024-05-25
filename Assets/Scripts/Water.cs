using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Water : MonoBehaviour
{
    [SerializeField] private Wave wave;

    [SerializeField] private Mesh mesh;
    [SerializeField] private float meshWidth;
    [SerializeField] private float meshDepth;
    [SerializeField] private float maxMeshHeight;
    [SerializeField] private int cellCount;

    //[SerializeField] private float perlinStepSizeX = 0.1f;
    //[SerializeField] private float perlinStepSizeZ = 0.1f;

    private MyVertices[] myVertices;
    private Vector3[] verticesVectors;
    //private Color[] colours;
    private Vector2[] uvs;
    //private Vector3[] normals;
    private int[] triangles;

    private void Start()
    {
        CreateWaterMesh();
    }

    private void CreateWaterMesh()
    {
        if(mesh == null)
        {
            mesh = new Mesh();
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
        }

        int verticesRowCount = cellCount + 1;
        int verticesCount = verticesRowCount * (cellCount + 1);
        int trianglesCount = 6 * cellCount * cellCount;

        myVertices = new MyVertices[verticesCount];
        verticesVectors = new Vector3[verticesCount];
        uvs = new Vector2[verticesCount];
        //colours = new Color[verticesCount];
        //normals = new Vector3[verticesCount];
        triangles = new int[trianglesCount];

        // Set the vertices of the mesh
        int vertexIndex = 0;
        for (int z = 0; z <= cellCount; ++z)
        {
            float percentageZ = (float)z / (float)cellCount;
            float startZ = percentageZ * meshDepth;

            for (int x = 0; x <= cellCount; ++x)
            {
                float percentageX = (float)x / (float)cellCount;
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
        for (int z = 0; z < cellCount; ++z)
        {
            for (int x = 0; x < cellCount; ++x)
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

        mesh.Clear();
        mesh.vertices = verticesVectors;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        //mesh.colors = colours;
        //mesh.normals = normals;
        mesh.RecalculateNormals();
        mesh.UploadMeshData(false);
    }
}
