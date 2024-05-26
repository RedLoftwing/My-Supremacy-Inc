using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] [Min(10)] private float gridFrequency = 10f;
    [SerializeField] [Min(30)] private int gridSize = 30;

    public Vector3 GetNearestPointOnGrid(Vector3 pos)
    {
        //Divides the X, Y and Z values of the position by the grid frequency value, and rounds it to the nearest int.
        int xCount = Mathf.RoundToInt(pos.x / gridFrequency);
        int yCount = Mathf.RoundToInt(pos.y / gridFrequency);
        int zCount = Mathf.RoundToInt(pos.z / gridFrequency);
        //Creates a new vector, result, using the rounded X, Y and Z values and multiplying them by the grid frequency. 
        Vector3 result = new Vector3(xCount * gridFrequency, yCount * gridFrequency, zCount * gridFrequency);
        //Returns the centered position to caller.
        return result;
    }

    private void OnDrawGizmos()
    {
        //[DEV] Sets colour of "gizmos" to yellow.
        Gizmos.color = Color.yellow;
        //[DEV] Goes through each coordinate on the grid and draws a sphere on the centred position of each coordinate.
        for (float x = 0; x < gridSize; x += gridFrequency)
        {
            for (float z = 0; z < gridSize; z += gridFrequency)
            {
                var point = GetNearestPointOnGrid(new Vector3(x, 0f, z));
                Gizmos.DrawSphere(point, 0.25f);
            }
        }
    }
}
