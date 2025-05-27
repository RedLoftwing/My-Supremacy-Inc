using System;
using System.Collections;
using System.Globalization;
using Player;
using UnityEngine;

public class GameState : MonoBehaviour {
    [Serializable] public class WaveComposition { public int[] waveComposition; }

    public static GameState Instance { get; private set; }
    public int WaveNumber { get; private set; }
    public bool IsInterWave { get; private set; }
    public bool isSpawningActive;
    public bool isSpawnZoneOccupied;
    public int totalWaveEnemyCount;
    public int numberOfPumpingStations;

    [SerializeField] private GameObject[] unitPrefabs;
    //Composition of enemies for each of the 10 waves. First number = Infantry. Second number = Rover. Third number = APC. Fourth number = Tank. Fifth number = Jet.
    [Tooltip("0 = Infantry, 1 = Rover, 2 = APC, 3 = Tank, 4 = Jet")]
    public WaveComposition[] waveCompositions;
    
    private const float Difference = 0.001f;
    
    private void Awake() {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    
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
        
        // Set the number of enemies present in this wave.
        totalWaveEnemyCount = thisWaveComposition[0] + thisWaveComposition[1] + thisWaveComposition[2] + thisWaveComposition[3] + thisWaveComposition[4];

        // Switch to an active wave state from an inter-wave state.
        GoToActiveWaveState(true);

        // Goes through each unit class type within the requested wave composition.
        for (var unitType = 0; unitType < thisWaveComposition.Length; unitType++) {
            // IF the quantity of this unit class type reaches 0...skip the following loop, and move onto the next unit class type.
            if (thisWaveComposition[unitType] <= 0) continue;
            // Goes through the number of units for this unit class type.
            for (var unitQuantity = 0; unitQuantity < thisWaveComposition[unitType]; unitQuantity++) {
                //Spawn is prevented until the spawn zone's collider is empty.
                yield return new WaitUntil(() => !isSpawnZoneOccupied);
                Instantiate(unitPrefabs[unitType], Vector3.zero, Quaternion.identity);
                yield return new WaitForSeconds(0.25f);
            }
        }
        // 123456789
        // C--S-BO-R

        // Wait until the number of enemies in this wave is equal to or less than 0...
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
        // Check for Victory Type.
        switch (PlayerStats.Instance.health) {
            case <= 0:
                DeclareEndGameText((PlayerStats.Instance.water >= 50) ? "Close Defeat" : "Decisive Defeat");
                break;
            case <= 10 when PlayerStats.Instance.water <= 10:
                DeclareEndGameText("Heroic Victory");
                break;
            case <= 40 when PlayerStats.Instance.water <= 20:
                DeclareEndGameText("Pyrrhic Victory");
                break;
            default:
                DeclareEndGameText((PlayerStats.Instance.water <= 65) ? "Close Victory" : "Decisive Victory");
                break;
        }

        //Set the final stat fields.
        UserInterface.Instance.finalWaterValue.SetText($"Water - {PlayerStats.Instance.water.ToString(CultureInfo.InvariantCulture)}%");
        UserInterface.Instance.finalCashValue.SetText($"Cash - {PlayerStats.Instance.cash.ToString()}");
        UserInterface.Instance.finalHealthValue.SetText($"Health - {PlayerStats.Instance.health.ToString(CultureInfo.InvariantCulture)}");
        UserInterface.Instance.endGamePanel.SetActive(true);
    }

    private static void DeclareEndGameText(string victoryDefeatType) {
        UserInterface.Instance.endGameTextPanel.SetText(victoryDefeatType);
        UserInterface.Instance.endGameBackTextPanel.SetText(victoryDefeatType);
    }

    public void ManualSpawnUnit(int requestedUnit) {
        // Called from the Cheats script. Will spawn a requested unit.
        Instantiate(unitPrefabs[requestedUnit], Vector3.zero, Quaternion.identity);
    }

    public void PauseGame() {
        // When called, swaps between the timescale being 0 (Paused) and 1 (Unpaused), and showing the pause menu and hiding it. 
        Time.timeScale = (Time.timeScale != 0) ? 0 : 1;
        UserInterface.Instance.pauseMenu.SetActive(!UserInterface.Instance.pauseMenu.activeSelf);
    }

    public void AlterGameSpeed() {
        // Switches the timescale between fast and normal.
        switch (Mathf.Abs(Time.timeScale - 3.0f)) {
            case > Difference:
                Time.timeScale = 3;
                break;
            case <= Difference:
                Time.timeScale = 1;
                UserInterface.Instance.pauseMenu.SetActive(false);
                break;
        }
    }
}