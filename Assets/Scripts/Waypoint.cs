using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var enemyComp = other.GetComponent<Enemies.Enemy>();
        if(enemyComp)
        {
            enemyComp.WaypointReached();
        }
    }
}
