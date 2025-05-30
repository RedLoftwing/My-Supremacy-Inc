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
    public Vector3 GetVector() { return vertexVector; }
    
    public void SetIndex(int i) { vertexIndex = i; }
    public void SetVector(Vector3 inVector) { vertexVector = inVector; }
}
