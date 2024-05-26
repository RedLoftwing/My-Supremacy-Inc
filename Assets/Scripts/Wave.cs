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
    private readonly List<MyVertices> _localVertices = new List<MyVertices>();
    //private List<int> localVertexIndices = new List<int>();
    private SplineAnimate _splineAnimate;

    void Awake()
    {
        water = FindAnyObjectByType<Water>().gameObject;
        _waterMeshFilter = water.GetComponent<MeshFilter>();
        _waterMesh = _waterMeshFilter.mesh;
        _waterMeshVertices = _waterMesh.vertices;
        _splineAnimate = this.GetComponent<SplineAnimate>();
        _splineAnimate.Container = GameObject.FindGameObjectWithTag("Spline3").GetComponent<SplineContainer>();
    }

    private void Update()
    {
        FindLocalVertices();

        ////IF the distance between this gameobject and the first main node in the array is less than 1...destroy this gameobject.
        //if (Vector3.Distance(transform.position, nodeArray[1].position) < 1)
        //{
        //    Destroy(this.gameObject);
        //}

        foreach (var myVertex in _localVertices)
        {
            int index = myVertex.GetIndex();
            _waterMeshVertices[index] = (new Vector3(_waterMeshVertices[index].x, _waterMeshVertices[index].y + waterRiseSpeed, _waterMeshVertices[index].z));
        }

        for(int i = 0;i < _waterMeshVertices.Length; i++)
        {
            if (_waterMeshVertices[i].y > 2)
            {
                _waterMeshVertices[i] = (new Vector3(_waterMeshVertices[i].x, _waterMeshVertices[i].y - waterLowerSpeed, _waterMeshVertices[i].z));
            }
        }

        _waterMesh.vertices = _waterMeshVertices;
        _waterMesh.RecalculateBounds();
    }

    private void FindLocalVertices()
    {
        _localVertices.Clear();
        //localVertexIndices.Clear();
        Vector3 halfSize = size * 0.5f;
        Vector3 minBounds = transform.position - halfSize;
        Vector3 maxBounds = transform.position + halfSize;

        Vector3[] allVertices = _waterMesh.vertices;

        for (int i = 0; i < allVertices.Length; i++)
        {
            Vector3 worldVertex = _waterMeshFilter.transform.TransformPoint(allVertices[i]); // Convert to world space
            if (IsWithinBounds(worldVertex, minBounds, maxBounds))
            {
                _localVertices.Add(new MyVertices(i, allVertices[i]));
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
}
