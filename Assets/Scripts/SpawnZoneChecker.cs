using UnityEngine;

public class SpawnZoneChecker : MonoBehaviour
{
    private float _timerValue;

    private void OnTriggerStay(Collider other)
    {
        //Upon collision...Get the Enemy component of the other game object.
        var enemy = other.gameObject.GetComponent<Enemies.Enemy>();
        //IF Enemy component is true...Set isSpawnOccupied to true.
        if (enemy)
        {
            GameState.Instance.isSpawnZoneOccupied = true;

            //Increase timer value.
            _timerValue++;
            //IF isSpawnZoneOccupied is true AND timerValue is greater than 50...Set bool to false and reset timer value.
            if(GameState.Instance.isSpawnZoneOccupied && _timerValue > 60)
            {
                GameState.Instance.isSpawnZoneOccupied = false;
                _timerValue = 0;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Upon exiting this collider...Set bool to false and reset timer value.
        GameState.Instance.isSpawnZoneOccupied = false;
        _timerValue = 0;
    }
}
