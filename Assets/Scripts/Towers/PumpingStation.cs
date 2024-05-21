using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpingStation : Tower
{
    private GameState gameStateScript;
    private void Start()
    {
        //Finds the GameState component and stores it as gameStateScript.
        gameStateScript = GameObject.Find("Manager").GetComponent<GameState>();
        //Add 1 to the numberOfPumpingStations value.
        gameStateScript.numberOfPumpingStations++;
    }
}
