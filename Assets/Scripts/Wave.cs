using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [SerializeField] private Transform[] nodeArray;
    public Vector3 size;

    [SerializeField] private GameObject water;
    [SerializeField] private Mesh waterMesh;
    [SerializeField] private MeshFilter waterMeshFilter;
    private Vector3[] waterMeshVertices;
    [SerializeField] private List<Vector3> localWaterMeshVertices = new List<Vector3>();

    [SerializeField] private float valueTest;

    // Start is called before the first frame update
    void Start()
    {
        //Find the Nodes gameobject and then store all it's children as a transform array.
        nodeArray = GameObject.Find("GroundNodes").GetComponentsInChildren<Transform>();
        //TODO: Create alternative to the A* Package pathfinding.
        ////Set target to first main entry in the nodeArray.
        //this.gameObject.GetComponent<AIDestinationSetter>().target = nodeArray[1];

        waterMeshFilter = water.GetComponent<MeshFilter>();
        waterMesh = waterMeshFilter.mesh;
        waterMeshVertices = waterMesh.vertices;
    }

    private void Update()
    {
        FindLocalVertices();

        //IF the distance between this gameobject and the first main node in the array is less than 1...destroy this gameobject.
        if (Vector3.Distance(transform.position, nodeArray[1].position) < 1)
        {
            Destroy(this.gameObject);
        }

        for(int i = 0; i < waterMeshVertices.Length; i++)
        {
            waterMeshVertices[i] += (Vector3.up * Time.deltaTime) * Random.Range(-valueTest, valueTest);
        }

        waterMesh.vertices = waterMeshVertices;
        waterMesh.RecalculateBounds();
    }

    private void FindLocalVertices()
    {
        localWaterMeshVertices.Clear();
        Vector3 halfSize = size * 0.5f;
        Vector3 minBounds = transform.position - halfSize;
        Vector3 maxBounds = transform.position + halfSize;

        foreach(Vector3 vertex in waterMesh.vertices)
        {
            Vector3 worldVertex = waterMeshFilter.transform.TransformPoint(vertex); // Convert to world space
            if (IsWithinBounds(worldVertex, minBounds, maxBounds))
            {
                localWaterMeshVertices.Add(worldVertex);
            }
        }


        //for(int i = 0;i < waterMeshVertices.Length;i++)
        //{
        //    var distance = Vector3.Distance(waterMeshVertices[i], transform.position);

        //    var sphereRadius = 10;

        //    if(distance < sphereRadius)
        //    {
        //        localWaterMeshVertices.Add(waterMeshVertices[i]);
        //    }
        //}
    }

    private bool IsWithinBounds(Vector3 point, Vector3 minBounds, Vector3 maxBounds)
    {
        return (point.x >= minBounds.x && point.x <= maxBounds.x &&
                point.y >= minBounds.y && point.y <= maxBounds.y &&
                point.z >= minBounds.z && point.z <= maxBounds.z);
    }
}
