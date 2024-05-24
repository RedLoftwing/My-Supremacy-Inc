using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaterGrid : MonoBehaviour
{
    [Range(0f, 1000f)] public float width = 10f;
    [Range(0f, 1000f)] public float length = 10f;

    [Range(1f, 1000f)] public int gridSizeX = 10;
    [Range(1f, 1000f)] public int gridSizeZ = 10;
    public float gridHeight;

    private Mesh mesh;

    [SerializeField] private MyVertices[] vertices;

    private void Awake()
    {
        GenerateMesh();
    }

    private void OnValidate()
    {
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        float cellWidth = width / gridSizeX;
        float cellLength = length / gridSizeZ;

        vertices = new MyVertices[(gridSizeX + 1) * (gridSizeZ + 1)];
        int[] triangles = new int[gridSizeX * gridSizeZ * 6];
        Vector2[] uv = new Vector2[(gridSizeX + 1) * (gridSizeZ + 1)];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z <= gridSizeZ; z++)
        {
            for (int x = 0; x <= gridSizeX; x++)
            {
                vertices[vert] = new MyVertices(vert, new Vector3(x * cellWidth, gridHeight, z * cellLength));

                uv[vert] = new Vector2((float)x / gridSizeX, (float)z / gridSizeZ);

                if (x < gridSizeX && z < gridSizeZ)
                {
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + gridSizeX + 1;
                    triangles[tris + 2] = vert + 1;
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + gridSizeX + 1;
                    triangles[tris + 5] = vert + gridSizeX + 2;

                    tris += 6;
                }

                vert++;
            }
        }

        Vector3[] vertexVectors = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertexVectors[i] = vertices[i].vertexVector;
        }

        mesh.Clear();
        mesh.vertices = vertexVectors;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
    }
}
