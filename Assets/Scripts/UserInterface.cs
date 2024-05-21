using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UserInterface : MonoBehaviour
{
    private string[] potentialNames = {"Button", "Button (1)", "Button (2)", "Button (3)", "Button (4)", "Button (5)", "Button (6)"};
    [SerializeField] private GameObject[] towerPrefabs;
    [SerializeField] private GameObject[] hologramPrefabs;
    [SerializeField] private WorldInteraction worldInteractionScript;
    [SerializeField] private GameState gameStateScript;
    [SerializeField] private PlayerStats playerStatesScript;
    private Scene currentScene;
    [SerializeField] private LayerMask uiLayerMask;
    private GameObject buttonObj;
    [SerializeField] private GameObject buttonHighlightObj;

    public GameObject towerInfoPanel;
    public TextMeshProUGUI towerInfoPanelTitle;
    public TextMeshProUGUI towerInfoPanelCost;
    public TextMeshProUGUI towerInfoPanelAttackInfo;

    private float gameTime;
    [SerializeField] private TextMeshProUGUI gameTimerText;

    public GameObject activeWavePanel;
    public TextMeshProUGUI waveText;

    public GameObject endGamePanel;
    public TextMeshProUGUI endGameTextPanel;
    public TextMeshProUGUI endGameBackTextPanel;
    public TextMeshProUGUI finalWaterValue;
    public TextMeshProUGUI finalCashValue;
    public TextMeshProUGUI finalHealthValue;

    public GameObject[] panelArray;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject mapSelectPanel;
    public GameObject pauseMenu;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject enemySpawnSection;

    [SerializeField] private GameObject waveObj;
    [SerializeField] private GameObject waveSpawnPoint;
    [SerializeField] private GameObject waterCellObj;

    private void Start()
    {
        //Finds the active scene and stores it as currentScene.
        currentScene = SceneManager.GetActiveScene();
        //IF the active scene's name is NOT "MainMenu"...then proceed with setting gameplay variables.
        if (currentScene.name != "MainMenu")
        {
            //Game Timer values set to default. Calls coroutine to start timer.
            gameTime = 0;
            gameTimerText.SetText(System.TimeSpan.FromSeconds(gameTime).ToString("mm':'ss"));
            StartCoroutine(GameTimer());
            //Set active state for the "Active Wave" UI object.
            activeWavePanel.SetActive(false);
            //Set Wave text values.
            waveText.SetText("Wave: " + gameStateScript.waveNumber + "/10");
        }
        //ELSE IF the active scene's name IS "MainMenu"...set anything that needs to be disabled as such.
        else if (currentScene.name == "MainMenu")
        {
            //Gets the Button components from each of the children objects, and stores them in the childrenComponents array.
            var childrenComponents = enemySpawnSection.GetComponentsInChildren<Button>();
            //For each button in the childrenComponents array... 
            foreach (var Button in childrenComponents)
            {
                //Sets spawn buttons as not interactable.
                Button.interactable = false;
            }
        }
    }

    //Called whenever one of the Tower Menu buttons are pressed.
    public void TowerButtonUsed()
    {
        //Grabs the gameobject (button) that was selected by the player, and stores it as buttonObj.
        buttonObj = EventSystem.current.currentSelectedGameObject;

        //IF the name of the button matches with any of the values stored in the potentialNames array...proceed with the true path...
        if(buttonObj.name == potentialNames[0])
        {
            AssignTower(0);
        }
        else if(buttonObj.name == potentialNames[1])
        {
            AssignTower(1);
        }
        else if (buttonObj.name == potentialNames[2])
        {
            AssignTower(2);
        }
        else if (buttonObj.name == potentialNames[3])
        {
            AssignTower(3);
        }
        else if (buttonObj.name == potentialNames[4])
        {
            AssignTower(4);
        }
        else if (buttonObj.name == potentialNames[5])
        {
            AssignTower(5);
        }
        else if (buttonObj.name == potentialNames[6])
        {
            AssignTower(6);
        }
        //ELSE IF...do nothing.
        else
        {
            Debug.Log("No name match for TowerButtonUsed...");
        }
    }

    private void AssignTower(int towerValue)
    {
        //Sets the object's activity to true, and then moves it's position to the selected tower.
        buttonHighlightObj.SetActive(true);
        buttonHighlightObj.transform.position = buttonObj.transform.position;
        //Assign the appropriate tower prefab (defined by towerValue) to the chosenTower value.
        worldInteractionScript.chosenTower = towerPrefabs[towerValue];
        worldInteractionScript.hologramTower = hologramPrefabs[towerValue];
    }

    private IEnumerator GameTimer()
    {
        //While gameTime is greater than or equal to 0...
        while (gameTime >= 0)
        {
            //Wait 1 second...then set text to the value of gameTime (Converted from float value to minutes and seconds), then increase the value of gameTime by 1.
            yield return new WaitForSeconds(1);
            gameTimerText.SetText(System.TimeSpan.FromSeconds(gameTime).ToString("mm':'ss"));
            gameTime++;
        }
    }

    public void PlayButton()
    {
        //IF the game is currently inbetween waves...
        if(gameStateScript.isInterWave)
        {
            //Advance to next wave and set isInterWave to false.
            gameStateScript.waveNumber++;
            gameStateScript.isInterWave = false;
        }
    }

    public void WaterAbility1()
    {
        //Sell water for cash.
        //IF water supply is greater than or equal to 5...call DecreaseWaterSupply and AwardCash.
        if(playerStatesScript.water >= 5)
        {
            playerStatesScript.DecreaseWaterSupply(5);
            playerStatesScript.AwardCash(250);
        }
    }

    public void WaterAbility2()
    {
        //IF cash is greater than or equal to 70...Instantiate waveObj at spawn point AND call SpendCash.
        if(playerStatesScript.cash >= 70)
        {
            Instantiate(waveObj, waveSpawnPoint.transform.position, Quaternion.identity);
            playerStatesScript.SpendCash(70);
        }
    }

    public void WaterAbility3()
    {
        //IF cash is greater than or equal to 70...Instantiate waterCellObj at a random location on the path AND call SpendCash.
        if (playerStatesScript.cash >= 70)
        {
            var paths = GameObject.Find("Enemy Path Group").GetComponentsInChildren<Transform>();
            var randomNum = Random.Range(10, paths.Length - 10);
            Instantiate(waterCellObj, paths[randomNum].position, Quaternion.identity);
            playerStatesScript.SpendCash(70);
        }
    }

    public void SwitchPanel(string requestedPanel)
    {
        //Used to switch between UI elements.
        //Called from a function that is called by a button (e.g. GoToNewGame is called by the NewGame button. Which then calls this function.)
        //FOR each gameObject in the panel array...
        for (int i = 0; i < panelArray.Length; i++)
        {
            //IF the gameObject's name matches the inputted string...Set it to active.
            if(panelArray[i].name == requestedPanel)
            {
                panelArray[i].SetActive(true);
            }
            //ELSE set it to inactive.
            else
            {
                panelArray[i].SetActive(false);
            }
        }
    }

    public void GoToNewGame()
    {
        //Called by a NewGame button. Calls SwitchPanel with the appropriate string.
        SwitchPanel("MapSelect Panel");
    }

    public void LoadMap1()
    {
        //Called by a LoadMap1 button. Loads the Map1 Scene.
        SceneManager.LoadScene("Map1");
    }

    public void LoadMap2()
    {
        //Called by a LoadMap2 button. Loads the Map2 Scene.
        SceneManager.LoadScene("Map2");
    }

    public void LoadMap3()
    {
        //Called by a LoadMap3 button. Loads the Map3 Scene.
        SceneManager.LoadScene("Map3");
    }

    public void LoadGame()
    {
        //Called by a LoadGame button. Calls SwitchPanel with the appropriate string.
    }

    public void GoToTutorial()
    {
        //Called by a Tutorial button. Loads the Tutorial Scene.
        SceneManager.LoadScene("Tutorial");
    }

    public void GoToSettings()
    {
        //Called by a Settings button. Calls SwitchPanel with the appropriate string.
        SwitchPanel("Settings Panel");
    }

    public void GoToCheats()
    {
        //Called by a Cheats button. Calls SwitchPanel with the appropriate string.
        SwitchPanel("Cheats Panel");
    }

    public void ReturnToPauseMenu()
    {
        //Called by a Return to Pause Menu button. Calls SwitchPanel with the appropriate string.
        SwitchPanel("Pause Menu");
    }

    public void ReturnToMainMenu()
    {
        //Finds the active scene and stores it as currentScene.
        currentScene = SceneManager.GetActiveScene();
        //IF the active scene's name is NOT "MainMenu"...then proceed with setting gameplay variables.
        if (currentScene.name != "MainMenu")
        {
            //Load the MainMenu scene.
            SceneManager.LoadScene("MainMenu");
        }
        //ELSE...set the main menu panel to true and hide all other panels/menus.
        else
        {
            SwitchPanel("MainMenu Panel");
        }
    }

    public void QuitGame()
    {
        //Quit the application.
        Application.Quit();
    }
}
