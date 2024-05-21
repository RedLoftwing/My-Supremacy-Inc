using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    [SerializeField] private UserInterface userInterfaceScript;
    [SerializeField] private PlayerStats playerStatsScript;
    [SerializeField] private SuperVillain superVillainScript;

    public int waveNumber;
    public bool isInterWave;
    public bool isSpawningActive;
    public bool isSpawnZoneOccupied;
    public int thisWavesEnemies;
    [SerializeField] private GameObject spawnPoint;
    public int numberOfPumpingStations;

    [SerializeField] private GameObject[] unitPrefabs;
    //Composition of enemies for each of the 10 waves. First number = Infantry. Second number = Rover. Third number = APC. Fourth number = Tank. Fifth number = Jet.
    private int[] wave1Composition = new int[5] { 5, 0, 0, 0, 0 };
    private int[] wave2Composition = new int[5] { 8, 3, 0, 0, 0 };
    private int[] wave3Composition = new int[5] { 10, 3, 0, 0, 0 };
    private int[] wave4Composition = new int[5] { 12, 5, 2, 0, 0 };
    private int[] wave5Composition = new int[5] { 15, 5, 4, 0, 0 };
    private int[] wave6Composition = new int[5] { 18, 10, 6, 1, 0 };
    private int[] wave7Composition = new int[5] { 20, 14, 8, 3, 0 };
    private int[] wave8Composition = new int[5] { 20, 14, 10, 4, 0 };
    private int[] wave9Composition = new int[5] { 22, 16, 10, 6, 2 };
    private int[] wave10Composition = new int[5] { 22, 16, 14, 8, 4 };

    // Start is called before the first frame update
    private void Start()
    {
        //Set default values.
        waveNumber = 0;
        thisWavesEnemies = 0;
        isInterWave = true;
        isSpawningActive = false;
        isSpawnZoneOccupied = false;
    }

    private void Update()
    {
        //IF a wave is ongoing AND spawning is not active...
        if(!isInterWave && !isSpawningActive)
        {
            //Checks the value of waveNumber...THEN starts the coroutine with the array of the coresponding value.
            switch (waveNumber)
            {
                case 1:
                    StartCoroutine(Wave(wave1Composition));
                    break;
                case 2:
                    StartCoroutine(Wave(wave2Composition));
                    break;
                case 3:
                    StartCoroutine(Wave(wave3Composition));
                    break;
                case 4:
                    StartCoroutine(Wave(wave4Composition));
                    break;
                case 5:
                    StartCoroutine(Wave(wave5Composition));
                    break;
                case 6:
                    StartCoroutine(Wave(wave6Composition));
                    break;
                case 7:
                    StartCoroutine(Wave(wave7Composition));
                    break;
                case 8:
                    StartCoroutine(Wave(wave8Composition));
                    break;
                case 9:
                    StartCoroutine(Wave(wave9Composition));
                    break;
                case 10:
                    StartCoroutine(Wave(wave10Composition));
                    break;
            }
        }
    }

    private IEnumerator Wave(int[] thisWaveComposition)
    {
        userInterfaceScript.activeWavePanel.SetActive(true);
        userInterfaceScript.waveText.SetText("Wave: " + waveNumber + "/10");
        //Set to true to prevent calling coroutine again.
        isSpawningActive = true;

        //Set a to 0. Allowing 
        int a = 0;

        thisWavesEnemies = thisWaveComposition[0] + thisWaveComposition[1] + thisWaveComposition[2] + thisWaveComposition[3] + thisWaveComposition[4];

        //Go through each unit for the wave.
        for (int i = 0; i < thisWaveComposition.Length; i++)
        {
            //If the coresponding unit value is greater than 0...Spawn said unit a number of times that coresponds with the value.
            if(thisWaveComposition[a] > 0)
            {
                for (int j = 0; j < thisWaveComposition[a]; j++)
                {
                    //Spawn is prevented until the spawn zone's collider is empty.
                    yield return new WaitUntil(() => !isSpawnZoneOccupied);
                    //IF the unit value (a) is NOT 4/a plane, then spawn is performed at ground spawn point and the game will wait 0.25 seconds before proceeding.
                    if (a != 4)
                    {
                        var newUnit = Instantiate(unitPrefabs[a], spawnPoint.transform.position, Quaternion.identity);
                    }
                    //ELSE if it does match...spawn the requested unit at the air spawn point. Air spawn point is calculated as the ground spawn point with a modified y value.
                    else
                    {
                        var airSpawnPoint = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y + 10, spawnPoint.transform.position.z);
                        var newUnit = Instantiate(unitPrefabs[a], airSpawnPoint, Quaternion.identity);
                    }
                    yield return new WaitForSeconds(0.25f);
                }
            }
            //Increase value to check the next unit value.
            a++;
        }

        //Wait until the number of enemies this wave is equal to or less than 0...
        yield return new WaitUntil(() => thisWavesEnemies <= 0);
        //Set isInterWave to true, isSpawningActive to false, turn off the Active Wave panel object, and call the cheering animation function for the Super Villain.
        isInterWave = true;
        isSpawningActive = false;
        userInterfaceScript.activeWavePanel.SetActive(false);
        superVillainScript.cheerState();

        //IF waveNumber is 10 or greater...
        if(waveNumber >= 10)
        {
            DeclareEndGame();
        }

        //Award Cash and Water.
        playerStatsScript.AwardCash(480);
        playerStatsScript.IncreaseWaterSupply(4 + (4 * numberOfPumpingStations));
        yield return null;
    }

    public void DeclareEndGame()
    {
        userInterfaceScript.endGamePanel.SetActive(true);

        //Check for Victory Type.
        //IF health is less than or equal to 0...
        if(playerStatsScript.health <= 0)
        {
            //IF water is greater than or equal to 50...call method with string.
            if (playerStatsScript.water >= 50)
            {
                DeclareEndGameText("Close Defeat");
            }
            //ELSE...call method with string.
            else
            {
                DeclareEndGameText("Decisive Defeat");
            }
        }
        //ELSE IF health is less than or equal to 10 AND water is less than or equal to 10...call method with string.
        else if (playerStatsScript.health <= 10 && playerStatsScript.water <= 10)
        {
            DeclareEndGameText("Heroic Victory");
        }
        //ELSE IF health is less than or equal to 40 AND water is less than or equal to 20...call method with string.
        else if (playerStatsScript.health <= 40 && playerStatsScript.water <= 20)
        {
            DeclareEndGameText("Pyrrhic Victory");
        }
        //ELSE...
        else
        {
            //IF water is less than or equal to 65...call method with string.
            if (playerStatsScript.water <= 65)
            {
                DeclareEndGameText("Close Victory");
            }
            //ELSE...call method with string.
            else
            {
                DeclareEndGameText("Decisive Victory");
            }
        }

        //Set the final stat fields.
        userInterfaceScript.finalWaterValue.SetText("Water - " + playerStatsScript.water.ToString() + "%");
        userInterfaceScript.finalCashValue.SetText("Cash - " + playerStatsScript.cash.ToString());
        userInterfaceScript.finalHealthValue.SetText("Health - " + playerStatsScript.health.ToString());
    }

    private void DeclareEndGameText(string victoryDefeatType)
    {
        //Sets the text in both text fields.
        userInterfaceScript.endGameTextPanel.SetText(victoryDefeatType);
        userInterfaceScript.endGameBackTextPanel.SetText(victoryDefeatType);
    }

    public void ManualSpawnUnit(int requestedUnit)
    {
        //Called from the Cheats script. Will spawn a requested unit.
        //IF the int value is not 4 (4 represents the 4th entry in the unit array which is a jet)...Spawn the requested unit at the ground spawn point.
        if(requestedUnit != 4)
        {
            Instantiate(unitPrefabs[requestedUnit], spawnPoint.transform.position, Quaternion.identity);
        }
        //ELSE if it does match...spawn the requested unit at the air spawn point. Air spawn point is calculated as the ground spawn point with a modified y value.
        else
        {
            var airSpawnPoint = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y + 10, spawnPoint.transform.position.z);
            var newUnit = Instantiate(unitPrefabs[requestedUnit], airSpawnPoint, Quaternion.identity);
        }
    }

    public void PauseGame()
    {
        //IF the timescale is anything BUT 0...set timescale to 0 and show the pause menu object.
        if (Time.timeScale != 0)
        {
            Time.timeScale = 0;
            userInterfaceScript.pauseMenu.SetActive(true);
        }
        //ELSE IF the timescale IS 0...set timescale to 1 and hide the pause menu object.
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            userInterfaceScript.pauseMenu.SetActive(false);
        }
    }

    public void AlterGameSpeed()
    {
        //IF the timescale is anything BUT 3...set the timescale to 3.
        if (Time.timeScale != 3)
        {
            Time.timeScale = 3;
        }
        //ELSE IF the timescale IS 3...set timescale to 1 and ensure the pause menu object is hidden.
        else if (Time.timeScale == 3)
        {
            Time.timeScale = 1;
            userInterfaceScript.pauseMenu.SetActive(false);
        }
    }
}
