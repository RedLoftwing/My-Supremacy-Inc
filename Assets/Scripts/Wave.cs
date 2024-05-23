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
    public List<MyVertices> localVertices = new List<MyVertices>();
    //public List<Vector3> localWaterMeshVertices = new List<Vector3>();
    private List<int> localVertexIndices = new List<int>();

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

        //for (int i = 0; i < waterMeshVertices.Length; i++)
        //{
        //    waterMeshVertices[i] += (Vector3.up * Time.deltaTime) * Random.Range(-valueTest, valueTest);
        //}

        //for (int i = 0; i < localWaterMeshVertices.Count; i++)
        //{
        //    localWaterMeshVertices[i] += (Vector3.up * Time.deltaTime) * Random.Range(-valueTest, valueTest);
        //}

        for (int i = 0; i < localVertices.Count; i++)
        {
            int index = localVertices[i].GetIndex();
            waterMeshVertices[index] += (Vector3.up * Time.deltaTime) * Random.Range(-valueTest, valueTest);
        }

        //for (int i = 0; i < localVertexIndices.Count; i++)
        //{
        //    int index = localVertexIndices[i];
        //    waterMeshVertices[index] = localWaterMeshVertices[i];
        //}

        waterMesh.vertices = waterMeshVertices;
        waterMesh.RecalculateBounds();

        //waterMesh.vertices = localWaterMeshVertices.ToArray();
        //waterMesh.RecalculateBounds();
    }

    private void FindLocalVertices()
    {
        localVertices.Clear();
        //localWaterMeshVertices.Clear();
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
                //localWaterMeshVertices.Add(allVertices[i]);
                localVertexIndices.Add(i); // Store the index
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
