using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

[RequireComponent(typeof(MeshRenderer))]
public class Water : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private float meshWidth;
    [SerializeField] private float meshDepth;
    [SerializeField] private float maxMeshHeight;
    [SerializeField] private int cellCountX;
    [SerializeField] private int cellCountZ;

    //[SerializeField] private float perlinStepSizeX = 0.1f;
    //[SerializeField] private float perlinStepSizeZ = 0.1f;

    [SerializeField] private Vector3[] vertices;
    //private Color[] colours;
    private Vector2[] uvs;
    //private Vector3[] normals;
    private int[] triangles;

    // Start is called before the first frame update
    void OnValidate()
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

        int verticesRowCount = cellCountX + 1;
        int verticesCount = verticesRowCount * (cellCountZ + 1);
        int trianglesCount = 6 * cellCountX * cellCountZ;

        vertices = new Vector3[verticesCount];
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

                // CHANGE ME! This height variable controls the height of each vertex in the generated grid.
                // If you want to see different heights per vertex you will need to change this in each iteration of these loops
                //Generate perlin noise...
                //float perlinNoise = Mathf.PerlinNoise(perlinStepSizeX * x, perlinStepSizeZ * z) * maxMeshHeight;
                float height = maxMeshHeight;



                vertices[vertexIndex] = new Vector3(startX, height, startZ);
                uvs[vertexIndex] = new Vector2(percentageX, percentageZ);       // No texturing so just set to zero
                //normals[vertexIndex] = Vector3.up;      // These should be set based on heights of terrain but we can use Recalulated normals on mesh to calculate for us
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

        // Assign all of the data that has been created to the mesh and update it
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        //mesh.colors = colours;
        //mesh.normals = normals;
        mesh.RecalculateNormals();
        mesh.UploadMeshData(false);
    }
}
