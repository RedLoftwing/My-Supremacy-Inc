using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var enemyComp = other.GetComponent<Enemy>();
        if(enemyComp)
        {
            enemyComp.WaypointReached();
        }
    }
}
