using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [SerializeField] private Transform[] nodeArray;

    // Start is called before the first frame update
    void Start()
    {
        //Find the Nodes gameobject and then store all it's children as a transform array.
        nodeArray = GameObject.Find("GroundNodes").GetComponentsInChildren<Transform>();
        //TODO: Create alternative to the A* Package pathfinding.
        ////Set target to first main entry in the nodeArray.
        //this.gameObject.GetComponent<AIDestinationSetter>().target = nodeArray[1];
    }

    private void Update()
    {
        //IF the distance between this gameobject and the first main node in the array is less than 1...destroy this gameobject.
        if(Vector3.Distance(transform.position, nodeArray[1].position) < 1)
        {
            Destroy(this.gameObject);
        }
    }
}
