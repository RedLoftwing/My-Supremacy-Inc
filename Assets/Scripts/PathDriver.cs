using UnityEngine;

public class PathDriver : MonoBehaviour
{
    [SerializeField] protected Transform[] nodeArray;
    [SerializeField] protected Transform targetNode;
    private int _nodeValue = 1;
    
    
    protected void Initialise()
    {
        //AssignNodes();
        //UpdateInfo();

    }

    private void AssignNodes()
    {
        //Find the Nodes gameobject and then store all it's children as a transform array.
        nodeArray = GameObject.Find(this.gameObject.CompareTag("Aerial") ? "AirNodes" : "GroundNodes").GetComponentsInChildren<Transform>();
    }

    private void UpdateInfo()
    {
        //Set the targetNode to the correct transform in the array.
        targetNode = nodeArray[_nodeValue];
        //
        transform.LookAt(targetNode);
    }

    public void WaypointReached()
    {
        if (this.gameObject.CompareTag("Wave"))
        {
            _nodeValue--;
        }
        else
        {
            _nodeValue++;
        }

        UpdateInfo();
    }
}
