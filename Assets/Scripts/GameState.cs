using System;
using System.Collections;
using System.Globalization;
using Player;
using UnityEngine;

public class GameState : MonoBehaviour {
    [Serializable] public class WaveComposition { public int[] waveComposition; }
    enum UnitTypes {
        Infantry = 0,
        Rover = 1,
        APC = 2,
        Tank = 3,
        Jet = 4
    };
    private UnitTypes _unitTypes;

    public static GameState Instance { get; private set; }
    public int WaveNumber { get; private set; }
    public bool IsInterWave { get; private set; }
    public bool isSpawningActive;
    public bool isSpawnZoneOccupied;
    public int totalWaveEnemyCount;
    [SerializeField] private GameObject spawnPoint;
    public int numberOfPumpingStations;

    [SerializeField] private GameObject[] unitPrefabs;
    //Composition of enemies for each of the 10 waves. First number = Infantry. Second number = Rover. Third number = APC. Fourth number = Tank. Fifth number = Jet.
    [Tooltip("0 = Infantry, 1 = Rover, 2 = APC, 3 = Tank, 4 = Jet")]
    public WaveComposition[] waveCompositions;
    
    private void Awake() {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    
    // Start is called before the first frame update
    private void Start() {
        //Set default values.
        WaveNumber = 0;
        totalWaveEnemyCount = 0;
        IsInterWave = true;
        isSpawningActive = false;
        isSpawnZoneOccupied = false;
    }

    public IEnumerator Wave(int[] thisWaveComposition) {
        // Show active wave UI elements and update wave # tracker.
        WaveNumber++;
        UserInterface.Instance.waveText.SetText($"Wave: {WaveNumber}/{waveCompositions.Length}");
        
        // Switch to an active wave state from an inter-wave state.
        GoToActiveWaveState(true);

        // Set the number of enemies present in this wave.
        totalWaveEnemyCount = thisWaveComposition[0] + thisWaveComposition[1] + thisWaveComposition[2] + thisWaveComposition[3] + thisWaveComposition[4];

        // Goes through each unit class type within the requested wave composition.
        for (var unitType = 0; unitType < thisWaveComposition.Length; unitType++) {
            // IF the quantity of this unit class type reaches 0...skip the following loop, and move onto the next unit class type.
            if (thisWaveComposition[unitType] <= 0) continue;
            // Goes through the number of units for this unit class type.
            for (var unitQuantity = 0; unitQuantity < thisWaveComposition[unitType]; unitQuantity++) {
                //Spawn is prevented until the spawn zone's collider is empty.
                yield return new WaitUntil(() => !isSpawnZoneOccupied);
        
                // Set the spawn height for the instantiation of the new unit based upon its class type.
                if (unitType != (int)UnitTypes.Jet) {
                    Instantiate(unitPrefabs[unitType], spawnPoint.transform.position, Quaternion.identity);
                }
                else {
                    var airSpawnPoint = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y + 50, spawnPoint.transform.position.z);
                    Instantiate(unitPrefabs[unitType], airSpawnPoint, Quaternion.identity);
                }
        
                yield return new WaitForSeconds(0.25f);
            }
        }
        // C----B

        // Wait until the number of enemies this wave is equal to or less than 0...
        yield return new WaitUntil(() => totalWaveEnemyCount <= 0);
        // Switch to an inter-wave state from an active wave state.
        GoToActiveWaveState(false);
        // Call the cheering animation function for the Super Villain.
        SuperVillain.Instance.CheerState();

        //IF waveNumber is 10 or greater...
        if (WaveNumber >= 10) {
            DeclareEndGame();
        }

        //Award Cash and Water.
        PlayerStats.Instance.AwardCash(480);
        PlayerStats.Instance.IncreaseWaterSupply(4 + (4 * numberOfPumpingStations));
        yield return null;
    }

    private void GoToActiveWaveState(bool inActive) {
        // Marks the interWave as active/inactive.
        IsInterWave = !inActive;
        // Prevents/enables another wave from/to activate(ing).
        isSpawningActive = inActive;
        // Shows/hides the "Active Wave" panel.
        UserInterface.Instance.activeWavePanel.SetActive(inActive);
    }

    public void DeclareEndGame() {
        UserInterface.Instance.endGamePanel.SetActive(true);

        //Check for Victory Type.
        //IF health is less than or equal to 0...
        if(PlayerStats.Instance.health <= 0) {
            //IF water is greater than or equal to 50...Call the method with the string depending on value.
            DeclareEndGameText((PlayerStats.Instance.water >= 50) ? "Close Defeat" : "Decisive Defeat");
        }
        //ELSE IF health is less than or equal to 10 AND water is less than or equal to 10...call method with string.
        else if (PlayerStats.Instance.health <= 10 && PlayerStats.Instance.water <= 10) {
            DeclareEndGameText("Heroic Victory");
        }
        //ELSE IF health is less than or equal to 40 AND water is less than or equal to 20...call method with string.
        else if (PlayerStats.Instance.health <= 40 && PlayerStats.Instance.water <= 20) {
            DeclareEndGameText("Pyrrhic Victory");
        }
        //ELSE...
        else {
            //IF water is less than or equal to 65...Call the method with the string depending on value
            DeclareEndGameText((PlayerStats.Instance.water <= 65) ? "Close Victory" : "Decisive Victory");
        }

        //Set the final stat fields.
        UserInterface.Instance.finalWaterValue.SetText("Water - " + PlayerStats.Instance.water.ToString(CultureInfo.InvariantCulture) + "%");
        UserInterface.Instance.finalCashValue.SetText("Cash - " + PlayerStats.Instance.cash.ToString());
        UserInterface.Instance.finalHealthValue.SetText("Health - " + PlayerStats.Instance.health.ToString(CultureInfo.InvariantCulture));
    }

    private void DeclareEndGameText(string victoryDefeatType) {
        //Sets the text in both text fields.
        UserInterface.Instance.endGameTextPanel.SetText(victoryDefeatType);
        UserInterface.Instance.endGameBackTextPanel.SetText(victoryDefeatType);
    }

    public void ManualSpawnUnit(int requestedUnit) {
        //Called from the Cheats script. Will spawn a requested unit.
        //IF the int value is not 4 (4 represents the 4th entry in the unit array which is a jet)...Spawn the requested unit at the ground spawn point.
        if(requestedUnit != 4) {
            Instantiate(unitPrefabs[requestedUnit], spawnPoint.transform.position, Quaternion.identity);
        }
        //ELSE if it does match...spawn the requested unit at the air spawn point. Air spawn point is calculated as the ground spawn point with a modified y value.
        else {
            var airSpawnPoint = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y + 10, spawnPoint.transform.position.z);
            Instantiate(unitPrefabs[requestedUnit], airSpawnPoint, Quaternion.identity);
        }
    }

    public void PauseGame() {
        //IF the timescale is anything BUT 0...set timescale to 0 and show the pause menu object.
        if (Time.timeScale != 0) {
            Time.timeScale = 0;
            UserInterface.Instance.pauseMenu.SetActive(true);
        }
        //ELSE IF the timescale IS 0...set timescale to 1 and hide the pause menu object.
        else if (Time.timeScale == 0) {
            Time.timeScale = 1;
            UserInterface.Instance.pauseMenu.SetActive(false);
        }
    }

    public void AlterGameSpeed() {
        const float difference = 0.001f;
        
        //IF the absolute value of the time below is greater than the difference...Speed up the timescale.
        //E.g. timeScale (1f) - 3.0f = 2.0f. 2.0f is greater than the difference...Speed up the timescale
        if (Mathf.Abs(Time.timeScale - 3.0f) > difference) {
            Time.timeScale = 3;
        }
        //ELSE IF the absolute value of the time below is less than or equal to the difference...Set timescale to 1.
        //E.g. timeScale (3f) - 3.0f = 0.0f. 0.0f is less than the difference...Set timescale to 1.
        else if (Mathf.Abs(Time.timeScale - 3.0f) <= difference) {
            Time.timeScale = 1;
            UserInterface.Instance.pauseMenu.SetActive(false);
        }
    }
}