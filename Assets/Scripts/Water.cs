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

    private MyVertices[] _myVertices;
    private Vector3[] _verticesVectors;
    //private Color[] colours;
    private Vector2[] _uvs;
    //private Vector3[] normals;
    private int[] _triangles;

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

        _myVertices = new MyVertices[verticesCount];
        _verticesVectors = new Vector3[verticesCount];
        _uvs = new Vector2[verticesCount];
        //colours = new Color[verticesCount];
        //normals = new Vector3[verticesCount];
        _triangles = new int[trianglesCount];

        // Set the vertices of the mesh
        int vertexIndex = 0;
        for (int z = 0; z <= cellCount; ++z)
        {
            float percentageZ = (float)z / cellCount;
            float startZ = percentageZ * meshDepth;

            for (int x = 0; x <= cellCount; ++x)
            {
                float percentageX = (float)x / cellCount;
                float startX = percentageX * meshWidth;
                float height = maxMeshHeight;

                _myVertices[vertexIndex] = new MyVertices(vertexIndex, new Vector3(startX, height, startZ));
                _verticesVectors[vertexIndex] = new Vector3(startX, height, startZ);
                _uvs[vertexIndex] = new Vector2(percentageX, percentageZ);
                //normals[vertexIndex] = Vector3.up;
                ++vertexIndex;
            }
        }

        // Set up the indexes, so they are in the correct order and will render correctly
        int trianglesIndex = 0;
        for (int z = 0; z < cellCount; ++z)
        {
            for (int x = 0; x < cellCount; ++x)
            {
                vertexIndex = x + (verticesRowCount * z);

                _triangles[trianglesIndex++] = vertexIndex;
                _triangles[trianglesIndex++] = vertexIndex + verticesRowCount;
                _triangles[trianglesIndex++] = (vertexIndex + 1) + verticesRowCount;
                _triangles[trianglesIndex++] = (vertexIndex + 1) + verticesRowCount;
                _triangles[trianglesIndex++] = vertexIndex + 1;
                _triangles[trianglesIndex++] = vertexIndex;
            }
        }

        mesh.Clear();
        mesh.vertices = _verticesVectors;
        mesh.uv = _uvs;
        mesh.triangles = _triangles;
        //mesh.colors = colours;
        //mesh.normals = normals;
        mesh.RecalculateNormals();
        mesh.UploadMeshData(false);
    }
}
