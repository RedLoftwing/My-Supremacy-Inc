using UnityEngine;

namespace Towers
{
    public class PumpingStation : Tower
    {
        private void Start()
        {
            // Add 1 to the numberOfPumpingStations value.
            GameState.Instance.numberOfPumpingStations++;
        }
    }
}