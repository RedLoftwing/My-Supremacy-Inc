using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathDriver : MonoBehaviour
{
    [SerializeField] protected Transform[] nodeArray;
    [SerializeField] protected Transform targetNode;
    protected int nodeValue = 1;

    protected void Initialise()
    {
        AssignNodes();
        UpdateInfo();
    }

    protected void AssignNodes()
    {
        if (!this.gameObject.CompareTag("Aerial"))
        {
            //Find the Nodes gameobject and then store all it's children as a transform array.
            nodeArray = GameObject.Find("GroundNodes").GetComponentsInChildren<Transform>();

            if (this.gameObject.CompareTag("Wave"))
            {
                nodeArray.Reverse();
                nodeValue = nodeArray.Length - 1;
            }
        }
        else
        {
            //Find the Nodes gameobject and then store all it's children as a transform array.
            nodeArray = GameObject.Find("AirNodes").GetComponentsInChildren<Transform>();
        }
    }

    protected void UpdateInfo()
    {
        //Set the targetNode to the correct tranform in the array.
        targetNode = nodeArray[nodeValue];
        //
        transform.LookAt(targetNode);
    }

    public void WaypointReached()
    {
        if (this.gameObject.CompareTag("Wave"))
        {
            nodeValue--;
        }
        else
        {
            nodeValue++;
        }

        UpdateInfo();
        //Debug.Log("Node Value is: " + nodeValue + ". Target Node is: " + targetNode);
        //Debug.Log("ANTI-Ding");
    }
}
