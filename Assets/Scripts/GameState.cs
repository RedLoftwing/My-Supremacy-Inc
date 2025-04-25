using System.Collections;
using System.Globalization;
using Player;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }
    public int waveNumber;
    public bool isInterWave;
    public bool isSpawningActive;
    public bool isSpawnZoneOccupied;
    public int thisWavesEnemies;
    [SerializeField] private GameObject spawnPoint;
    public int numberOfPumpingStations;

    [SerializeField] private GameObject[] unitPrefabs;
    //Composition of enemies for each of the 10 waves. First number = Infantry. Second number = Rover. Third number = APC. Fourth number = Tank. Fifth number = Jet.
    private readonly int[] _wave1Composition = new int[] { 5, 0, 0, 0, 0 };
    private readonly int[] _wave2Composition = new int[] { 8, 3, 0, 0, 0 };
    private readonly int[] _wave3Composition = new int[] { 10, 3, 0, 0, 0 };
    private readonly int[] _wave4Composition = new int[] { 12, 5, 2, 0, 0 };
    private readonly int[] _wave5Composition = new int[] { 15, 5, 4, 0, 0 };
    private readonly int[] _wave6Composition = new int[] { 18, 10, 6, 1, 0 };
    private readonly int[] _wave7Composition = new int[] { 20, 14, 8, 3, 0 };
    private readonly int[] _wave8Composition = new int[] { 20, 14, 10, 4, 0 };
    private readonly int[] _wave9Composition = new int[] { 22, 16, 10, 6, 2 };
    private readonly int[] _wave10Composition = new int[] { 22, 16, 14, 8, 4 };

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    
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
            //Checks the value of waveNumber...THEN starts the coroutine with the array of the corresponding value.
            switch (waveNumber)
            {
                case 1:
                    StartCoroutine(Wave(_wave1Composition));
                    break;
                case 2:
                    StartCoroutine(Wave(_wave2Composition));
                    break;
                case 3:
                    StartCoroutine(Wave(_wave3Composition));
                    break;
                case 4:
                    StartCoroutine(Wave(_wave4Composition));
                    break;
                case 5:
                    StartCoroutine(Wave(_wave5Composition));
                    break;
                case 6:
                    StartCoroutine(Wave(_wave6Composition));
                    break;
                case 7:
                    StartCoroutine(Wave(_wave7Composition));
                    break;
                case 8:
                    StartCoroutine(Wave(_wave8Composition));
                    break;
                case 9:
                    StartCoroutine(Wave(_wave9Composition));
                    break;
                case 10:
                    StartCoroutine(Wave(_wave10Composition));
                    break;
            }
        }
    }

    private IEnumerator Wave(int[] thisWaveComposition)
    {
        UserInterface.Instance.activeWavePanel.SetActive(true);
        UserInterface.Instance.waveText.SetText("Wave: " + waveNumber + "/10");
        //Set to true to prevent calling coroutine again.
        isSpawningActive = true;

        thisWavesEnemies = thisWaveComposition[0] + thisWaveComposition[1] + thisWaveComposition[2] +
                           thisWaveComposition[3] + thisWaveComposition[4];

        //Goes through each unit class within the requested wave composition.
        for (int unitType = 0; unitType < thisWaveComposition.Length; unitType++)
        {
            if (thisWaveComposition[unitType] > 0)
            {
                for (int unitQuantity = 0; unitQuantity < thisWaveComposition[unitType]; unitQuantity++)
                {
                    //Spawn is prevented until the spawn zone's collider is empty.
                    yield return new WaitUntil(() => !isSpawnZoneOccupied);
        
                    //IF the unitType value is NOT 4 (a plane), then spawn is performed at ground spawn point and the game will wait 0.25 seconds before proceeding.
                    if (unitType != 4)
                    {
                        Instantiate(unitPrefabs[unitType], spawnPoint.transform.position, Quaternion.identity);
                    }
                    //ELSE if it does match...spawn the requested unit at the air spawn point. Air spawn point is calculated as the ground spawn point with a modified y value.
                    else
                    {
                        var airSpawnPoint = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y + 10, spawnPoint.transform.position.z);
                        Instantiate(unitPrefabs[unitType], airSpawnPoint, Quaternion.identity);
                    }
        
                    yield return new WaitForSeconds(0.25f);
                }
            }
        }

        //Wait until the number of enemies this wave is equal to or less than 0...
        yield return new WaitUntil(() => thisWavesEnemies <= 0);
        //Set isInterWave to true, isSpawningActive to false. Turn off the Active Wave panel object, and call the cheering animation function for the Super Villain.
        isInterWave = true;
        isSpawningActive = false;
        UserInterface.Instance.activeWavePanel.SetActive(false);
        SuperVillain.Instance.CheerState();

        //IF waveNumber is 10 or greater...
        if (waveNumber >= 10)
        {
            DeclareEndGame();
        }

        //Award Cash and Water.
        PlayerStats.Instance.AwardCash(480);
        PlayerStats.Instance.IncreaseWaterSupply(4 + (4 * numberOfPumpingStations));
        yield return null;
    }

    public void DeclareEndGame()
    {
        UserInterface.Instance.endGamePanel.SetActive(true);

        //Check for Victory Type.
        //IF health is less than or equal to 0...
        if(PlayerStats.Instance.health <= 0)
        {
            //IF water is greater than or equal to 50...Call the method with the string depending on value.
            DeclareEndGameText((PlayerStats.Instance.water >= 50) ? "Close Defeat" : "Decisive Defeat");
        }
        //ELSE IF health is less than or equal to 10 AND water is less than or equal to 10...call method with string.
        else if (PlayerStats.Instance.health <= 10 && PlayerStats.Instance.water <= 10)
        {
            DeclareEndGameText("Heroic Victory");
        }
        //ELSE IF health is less than or equal to 40 AND water is less than or equal to 20...call method with string.
        else if (PlayerStats.Instance.health <= 40 && PlayerStats.Instance.water <= 20)
        {
            DeclareEndGameText("Pyrrhic Victory");
        }
        //ELSE...
        else
        {
            //IF water is less than or equal to 65...Call the method with the string depending on value
            DeclareEndGameText((PlayerStats.Instance.water <= 65) ? "Close Victory" : "Decisive Victory");
        }

        //Set the final stat fields.
        UserInterface.Instance.finalWaterValue.SetText("Water - " + PlayerStats.Instance.water.ToString(CultureInfo.InvariantCulture) + "%");
        UserInterface.Instance.finalCashValue.SetText("Cash - " + PlayerStats.Instance.cash.ToString());
        UserInterface.Instance.finalHealthValue.SetText("Health - " + PlayerStats.Instance.health.ToString(CultureInfo.InvariantCulture));
    }

    private void DeclareEndGameText(string victoryDefeatType)
    {
        //Sets the text in both text fields.
        UserInterface.Instance.endGameTextPanel.SetText(victoryDefeatType);
        UserInterface.Instance.endGameBackTextPanel.SetText(victoryDefeatType);
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
            Instantiate(unitPrefabs[requestedUnit], airSpawnPoint, Quaternion.identity);
        }
    }

    public void PauseGame()
    {
        //IF the timescale is anything BUT 0...set timescale to 0 and show the pause menu object.
        if (Time.timeScale != 0)
        {
            Time.timeScale = 0;
            UserInterface.Instance.pauseMenu.SetActive(true);
        }
        //ELSE IF the timescale IS 0...set timescale to 1 and hide the pause menu object.
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            UserInterface.Instance.pauseMenu.SetActive(false);
        }
    }

    public void AlterGameSpeed()
    {
        const float difference = 0.001f;
        
        //IF the absolute value of the time below is greater than the difference...Speed up the timescale.
        //E.g. timeScale (1f) - 3.0f = 2.0f. 2.0f is greater than the difference...Speed up the timescale
        if (Mathf.Abs(Time.timeScale - 3.0f) > difference)
        {
            Time.timeScale = 3;
        }
        //ELSE IF the absolute value of the time below is less than or equal to the difference...Set timescale to 1.
        //E.g. timeScale (3f) - 3.0f = 0.0f. 0.0f is less than the difference...Set timescale to 1.
        else if (Mathf.Abs(Time.timeScale - 3.0f) <= difference)
        {
            Time.timeScale = 1;
            UserInterface.Instance.pauseMenu.SetActive(false);
        }
    }
}
