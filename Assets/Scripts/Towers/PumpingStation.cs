using UnityEngine;

namespace Towers
{
    public class PumpingStation : Tower
    {
        private GameState _gameStateScript;

        private void Start()
        {
            //Finds the GameState component and stores it as gameStateScript.
            _gameStateScript = GameObject.Find("Manager").GetComponent<GameState>();
            //Add 1 to the numberOfPumpingStations value.
            _gameStateScript.numberOfPumpingStations++;
        }
    }
}