using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Wave : MonoBehaviour
{
    public Vector3 size;
    [SerializeField] private float waterLowerSpeed;
    [SerializeField] private float waterRiseSpeed;
    [SerializeField] private GameObject water;
    private Mesh waterMesh;
    private MeshFilter waterMeshFilter;
    private Vector3[] waterMeshVertices;
    private List<MyVertices> localVertices = new List<MyVertices>();
    private List<int> localVertexIndices = new List<int>();
    private SplineAnimate splineAnimate;

    void Awake()
    {
        water = FindAnyObjectByType<Water>().gameObject;
        waterMeshFilter = water.GetComponent<MeshFilter>();
        waterMesh = waterMeshFilter.mesh;
        waterMeshVertices = waterMesh.vertices;
        splineAnimate = this.GetComponent<SplineAnimate>();
        splineAnimate.Container = GameObject.FindGameObjectWithTag("Spline3").GetComponent<SplineContainer>();
    }

    private void Update()
    {
        FindLocalVertices();

        ////IF the distance between this gameobject and the first main node in the array is less than 1...destroy this gameobject.
        //if (Vector3.Distance(transform.position, nodeArray[1].position) < 1)
        //{
        //    Destroy(this.gameObject);
        //}

        for (int i = 0; i < localVertices.Count; i++)
        {
            int index = localVertices[i].GetIndex();
            waterMeshVertices[index] = (new Vector3(waterMeshVertices[index].x, waterMeshVertices[index].y + waterRiseSpeed, waterMeshVertices[index].z));
        }

        for(int i = 0;i < waterMeshVertices.Length; i++)
        {
            if (waterMeshVertices[i].y > 2)
            {
                waterMeshVertices[i] = (new Vector3(waterMeshVertices[i].x, waterMeshVertices[i].y - waterLowerSpeed, waterMeshVertices[i].z));
            }
        }

        waterMesh.vertices = waterMeshVertices;
        waterMesh.RecalculateBounds();
    }

    private void FindLocalVertices()
    {
        localVertices.Clear();
        localVertexIndices.Clear();
        Vector3 halfSize = size * 0.5f;
        Vector3 minBounds = transform.position - halfSize;
        Vector3 maxBounds = transform.position + halfSize;

        Vector3[] allVertices = waterMesh.vertices;

        for (int i = 0; i < allVertices.Length; i++)
        {
            Vector3 worldVertex = waterMeshFilter.transform.TransformPoint(allVertices[i]); // Convert to world space
            if (IsWithinBounds(worldVertex, minBounds, maxBounds))
            {
                localVertices.Add(new MyVertices(i, allVertices[i]));
                localVertexIndices.Add(i);
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
