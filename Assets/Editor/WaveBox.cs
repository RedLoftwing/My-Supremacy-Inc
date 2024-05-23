using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Wave))]
public class WaveBox : Editor
{
    private void OnSceneGUI()
    {
        // Get the target script
        Wave wave = (Wave)target;

        // Calculate the vertices of the cube
        Vector3[] vertices = new Vector3[8];
        Vector3 halfSize = wave.size / 2;
        vertices[0] = wave.transform.position + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
        vertices[1] = wave.transform.position + new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
        vertices[2] = wave.transform.position + new Vector3(halfSize.x, -halfSize.y, halfSize.z);
        vertices[3] = wave.transform.position + new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
        vertices[4] = wave.transform.position + new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
        vertices[5] = wave.transform.position + new Vector3(halfSize.x, halfSize.y, -halfSize.z);
        vertices[6] = wave.transform.position + new Vector3(halfSize.x, halfSize.y, halfSize.z);
        vertices[7] = wave.transform.position + new Vector3(-halfSize.x, halfSize.y, halfSize.z);

        // Draw the edges of the cube
        Handles.color = Color.white;
        Handles.DrawLine(vertices[0], vertices[1]);
        Handles.DrawLine(vertices[1], vertices[2]);
        Handles.DrawLine(vertices[2], vertices[3]);
        Handles.DrawLine(vertices[3], vertices[0]);

        Handles.DrawLine(vertices[4], vertices[5]);
        Handles.DrawLine(vertices[5], vertices[6]);
        Handles.DrawLine(vertices[6], vertices[7]);
        Handles.DrawLine(vertices[7], vertices[4]);

        Handles.DrawLine(vertices[0], vertices[4]);
        Handles.DrawLine(vertices[1], vertices[5]);
        Handles.DrawLine(vertices[2], vertices[6]);
        Handles.DrawLine(vertices[3], vertices[7]);
    }
}
