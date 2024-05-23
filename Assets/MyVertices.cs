using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MyVertices
{
    public int vertexIndex;
    public Vector3 vertexVector;

    public MyVertices (int vertexIndex, Vector3 vertexVector)
    {
        this.vertexIndex = vertexIndex;
        this.vertexVector = vertexVector;
    }

    public void DisplayValues()
    {
        Debug.Log("Integer: " + vertexIndex + "- Vector3: " + vertexVector);
    }

    public int GetIndex() { return vertexIndex; }
}
