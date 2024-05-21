using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZoneChecker : MonoBehaviour
{
    [SerializeField] private GameState gameStateScript;
    private float timerValue = 0;

    private void OnTriggerStay(Collider other)
    {
        //Upon collision...Get the Enemy component of the other game object.
        var enemy = other.gameObject.GetComponent<Enemy>();
        //IF Enemy component is true...Set isSpawnOccupied to true.
        if (enemy)
        {
            gameStateScript.isSpawnZoneOccupied = true;

            //Increase timer value.
            timerValue++;
            //IF isSpawnZoneOccupied is true AND timerValue is greater than 50...Set bool to false and reset timer value.
            if(gameStateScript.isSpawnZoneOccupied && timerValue > 60)
            {
                gameStateScript.isSpawnZoneOccupied = false;
                timerValue = 0;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Upon exiting this collider...Set bool to false and reset timer value.
        gameStateScript.isSpawnZoneOccupied = false;
        timerValue = 0;
    }
}
